﻿namespace Smart.AspNetCore.Formatters
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Formatters;

    using Smart.Collections.Concurrent;
    using Smart.IO.ByteMapper;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Ignore")]
    public class ByteMapperInputFormatter : InputFormatter
    {
        private static readonly Type SingleReaderType = typeof(SingleInputReader<>);

        private static readonly Type ArrayReaderType = typeof(ArrayInputReader<>);

        private static readonly Type ListReaderType = typeof(ListInputReader<>);

        private readonly ThreadsafeTypeHashArrayMap<IInputReader> readerCache = new ThreadsafeTypeHashArrayMap<IInputReader>();

        private readonly ThreadsafeHashArrayMap<MapperKey, IInputReader> profiledReaderCache = new ThreadsafeHashArrayMap<MapperKey, IInputReader>();

        private readonly ByteMapperFormatterConfig config;

        public ByteMapperInputFormatter(ByteMapperFormatterConfig config)
        {
            this.config = config;
            foreach (var mediaType in config.SupportedMediaTypes)
            {
                SupportedMediaTypes.Add(mediaType);
            }
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var profile = context.HttpContext.Items.TryGetValue(Const.ProfileKey, out var value)
                ? value as string
                : null;
            var reader = String.IsNullOrEmpty(profile)
                ? GetReader(context.ModelType)
                : GetReader(context.ModelType, profile);

            var request = context.HttpContext.Request;

            var model = reader.Read(request.Body, request.ContentLength);

            return InputFormatterResult.SuccessAsync(model);
        }

        private IInputReader GetReader(Type type)
        {
            if (!readerCache.TryGetValue(type, out var reader))
            {
                reader = readerCache.AddIfNotExist(type, CreateReader);
            }

            return reader;
        }

        private IInputReader GetReader(Type type, string profile)
        {
            var key = new MapperKey(type, profile);
            if (!profiledReaderCache.TryGetValue(key, out var reader))
            {
                reader = profiledReaderCache.AddIfNotExist(key, CreateReader);
            }

            return reader;
        }

        private IInputReader CreateReader(Type type)
        {
            var readerType = ResolveReaderType(type);
            return (IInputReader)Activator.CreateInstance(readerType, config, null);
        }

        private IInputReader CreateReader(MapperKey key)
        {
            var readerType = ResolveReaderType(key.Type);
            return (IInputReader)Activator.CreateInstance(readerType, config, key.Profile);
        }

        private static Type ResolveReaderType(Type type)
        {
            if (type.IsArray)
            {
                return ArrayReaderType.MakeGenericType(type.GetElementType());
            }

            if (TypeHelper.IsEnumerableType(type))
            {
                return ListReaderType.MakeGenericType(type.GenericTypeArguments[0]);
            }

            return SingleReaderType.MakeGenericType(type);
        }

        private interface IInputReader
        {
            object Read(Stream stream, long? length);
        }

        private sealed class SingleInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            public SingleInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = mapper.Size;
            }

            public object Read(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    if (stream.Read(buffer, 0, bufferSize) != bufferSize)
                    {
                        return default;
                    }

                    var target = factory();
                    mapper.FromByte(buffer, 0, target);
                    return target;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private sealed class ArrayInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            private readonly int readSize;

            public ArrayInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = Math.Max(config.BufferSize, mapper.Size);
                readSize = (bufferSize / mapper.Size) * mapper.Size;
            }

            public object Read(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    if (length.HasValue)
                    {
                        var array = new T[length.Value / mapper.Size];

                        var index = 0;
                        int read;
                        while ((read = stream.Read(buffer, 0, readSize)) > 0)
                        {
                            var limit = read - mapper.Size;
                            for (var pos = 0; pos <= limit; pos += mapper.Size)
                            {
                                var target = factory();
                                mapper.FromByte(buffer, pos, target);
                                array[index] = target;
                                index++;
                            }
                        }

                        return array;
                    }
                    else
                    {
                        var list = new List<T>();

                        int read;
                        while ((read = stream.Read(buffer, 0, readSize)) > 0)
                        {
                            var limit = read - mapper.Size;
                            for (var pos = 0; pos <= limit; pos += mapper.Size)
                            {
                                var target = factory();
                                mapper.FromByte(buffer, pos, target);
                                list.Add(target);
                            }
                        }

                        return list.ToArray();
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private sealed class ListInputReader<T> : IInputReader
        {
            private readonly ITypeMapper<T> mapper;

            private readonly Func<T> factory;

            private readonly int bufferSize;

            private readonly int readSize;

            public ListInputReader(ByteMapperFormatterConfig config, string profile)
            {
                mapper = String.IsNullOrEmpty(profile) ? config.MapperFactory.Create<T>() : config.MapperFactory.Create<T>(profile);
                factory = config.DelegateFactory.CreateFactory<T>();
                bufferSize = Math.Max(config.BufferSize, mapper.Size);
                readSize = (bufferSize / mapper.Size) * mapper.Size;
            }

            public object Read(Stream stream, long? length)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    var list = length.HasValue ? new List<T>((int)(length.Value / mapper.Size)) : new List<T>();

                    int read;
                    while ((read = stream.Read(buffer, 0, readSize)) > 0)
                    {
                        var limit = read - mapper.Size;
                        for (var pos = 0; pos <= limit; pos += mapper.Size)
                        {
                            var target = factory();
                            mapper.FromByte(buffer, pos, target);
                            list.Add(target);
                        }
                    }

                    return list;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}