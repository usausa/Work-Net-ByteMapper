﻿namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Collections.Generic;
    using Smart.ComponentModel;
    using Smart.IO.Mapper.Builders;
    using Smart.IO.Mapper.Mappers;
    using Smart.Reflection;

    internal class AttributeMapping : IMapping
    {
        private readonly MapAttribute mapAttribute;

        private readonly bool validate;

        public Type Type { get; }

        public string Profile { get; }

        public int Size => mapAttribute.Size;

        public AttributeMapping(Type type, MapAttribute mapAttribute, string profile, bool validate)
        {
            Type = type;
            this.mapAttribute = mapAttribute;
            Profile = profile;
            this.validate = validate;
        }

        public IMapper[] CreateMappers(IComponentContainer components, IDictionary<string, object> parameters)
        {
            var context = new BuilderContext(
                components,
                parameters,
                Type.GetCustomAttributes().OfType<ITypeDefaultAttribute>().ToDictionary(x => x.Key, x => x.Value));

            var list = new List<MapperPosition>();
            list.AddRange(CreateTypeEntries(context));
            list.AddRange(CreateMemberEntries(context));

            if (mapAttribute.AutoDelimitter)
            {
                var delimiter = context.GetParameter<byte[]>(Parameter.Delimiter);
                if ((delimiter != null) && (delimiter.Length > 0))
                {
                    var offset = mapAttribute.Size - delimiter.Length;
                    list.Add(new MapperPosition(offset, delimiter.Length, new ConstantMapper(offset, delimiter)));
                }
            }

            list.Sort(MapperPosition.Comparer);

            var filler = default(byte?);
            var fillers = new List<MapperPosition>();
            for (var i = 0; i < list.Count; i++)
            {
                var end = list[i].Offset + list[i].Size;
                var next = i < list.Count - 1 ? list[i + 1].Offset : mapAttribute.Size;

                if (validate && (end > next))
                {
                    throw new ByteMapperException(
                        "Range overlap. " +
                        $"type=[{Type.FullName}], " +
                        $"range=[{list[i].Offset}..{end}], " +
                        $"next=[{next}]");
                }

                if (mapAttribute.AutoFiller && (end < next))
                {
                    if (!filler.HasValue)
                    {
                        filler = context.GetParameter<byte>(Parameter.Filler);
                    }

                    var length = next - end;
                    fillers.Add(new MapperPosition(
                        end,
                        length,
                        new FillMapper(end, length, filler.Value)));
                }
            }

            if (fillers.Count > 0)
            {
                list.AddRange(fillers);
                list.Sort(MapperPosition.Comparer);
            }

            return list.Select(x => x.Mapper).ToArray();
        }

        private IEnumerable<MapperPosition> CreateTypeEntries(IBuilderContext context)
        {
            return Type.GetCustomAttributes()
                .OfType<AbstractTypeMapAttribute>()
                .Select(x =>
                {
                    var builder = x.GetTypeMapperBuilder();
                    return new MapperPosition(
                        builder.Offset,
                        builder.CalcSize(Type),
                        builder.CreateMapper(context, Type));
                });
        }

        private IEnumerable<MapperPosition> CreateMemberEntries(IBuilderContext context)
        {
            return Type.GetProperties()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.GetCustomAttributes().OfType<AbstractMemberMapAttribute>().FirstOrDefault(),
                    ArrayAttribute = x.GetCustomAttribute<MapArrayAttribute>()
                })
                .Where(x => x.Attribute != null)
                .Select(x =>
                {
                    var delegateFactory = context.Components.Get<IDelegateFactory>();

                    if (x.ArrayAttribute != null)
                    {
                        if (!x.Property.PropertyType.IsArray)
                        {
                            throw new ByteMapperException(
                                "Attribute does not match property. " +
                                $"type=[{x.Property.DeclaringType?.FullName}], " +
                                $"property=[{x.Property.Name}], " +
                                $"attribute=[{typeof(MapArrayAttribute).FullName}]");
                        }

                        var elementType = x.Property.PropertyType.GetElementType();

                        var elementBuilder = x.Attribute.GetConverterBuilder();
                        var elementConverter = elementBuilder.CreateConverter(context, elementType);
                        if (elementConverter == null)
                        {
                            throw new ByteMapperException(
                                "Attribute does not match property. " +
                                $"type=[{x.Property.DeclaringType?.FullName}], " +
                                $"property=[{x.Property.Name}], " +
                                $"attribute=[{x.Attribute.GetType().FullName}]");
                        }

                        var arrayBuilder = x.ArrayAttribute.GetArrayConverterBuilder();
                        arrayBuilder.ElementConverterBuilder = elementBuilder;

                        return new MapperPosition(
                            x.Attribute.Offset,
                            arrayBuilder.CalcSize(elementType),
                            new MemberMapper(
                                x.Attribute.Offset,
                                arrayBuilder.CreateConverter(context, elementType),
                                delegateFactory.CreateGetter(x.Property),
                                delegateFactory.CreateSetter(x.Property)));
                    }

                    var builder = x.Attribute.GetConverterBuilder();
                    var converter = builder.CreateConverter(context, x.Property.PropertyType);
                    if (converter == null)
                    {
                        throw new ByteMapperException(
                            "Attribute does not match property. " +
                            $"type=[{x.Property.DeclaringType?.FullName}], " +
                            $"property=[{x.Property.Name}], " +
                            $"attribute=[{x.Attribute.GetType().FullName}]");
                    }

                    return new MapperPosition(
                        x.Attribute.Offset,
                        builder.CalcSize(x.Property.PropertyType),
                        new MemberMapper(
                            x.Attribute.Offset,
                            converter,
                            delegateFactory.CreateGetter(x.Property),
                            delegateFactory.CreateSetter(x.Property)));
                });
        }

        internal class MapperPosition
        {
            public static IComparer<MapperPosition> Comparer => Comparers.Delegate<MapperPosition>((x, y) => x.Offset - y.Offset);

            public int Offset { get; }

            public int Size { get; }

            public IMapper Mapper { get; }

            public MapperPosition(int offset, int size, IMapper mapper)
            {
                Offset = offset;
                Size = size;
                Mapper = mapper;
            }
        }
    }
}
