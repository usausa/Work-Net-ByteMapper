﻿namespace Smart.IO.ByteMapper
{
    using System;
    using System.Linq;

    using Smart.IO.ByteMapper.Mappers;

    internal class TypeMapper<T> : ITypeMapper<T>
    {
        private readonly IMapper[] readableMappers;

        private readonly IMapper[] writableMappers;

        public Type TargetType { get; }

        public int Size { get; }

        public TypeMapper(Type targetType, int size, IMapper[] mappers)
        {
            TargetType = targetType;
            Size = size;
            readableMappers = mappers.Where(x => x.CanRead).ToArray();
            writableMappers = mappers.Where(x => x.CanWrite).ToArray();
        }

        public void FromByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < readableMappers.Length; i++)
            {
                readableMappers[i].Read(buffer, index, target);
            }
        }

        public void ToByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < writableMappers.Length; i++)
            {
                writableMappers[i].Write(buffer, index, target);
            }
        }
    }
}