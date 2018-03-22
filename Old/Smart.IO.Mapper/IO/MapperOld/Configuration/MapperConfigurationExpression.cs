﻿namespace Smart.IO.MapperOld.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Smart.IO.MapperOld.Converters;
    using Smart.IO.MapperOld.Mappers;

    /// <summary>
    ///
    /// </summary>
    internal class MapperConfigurationExpression : IMapperConfigurationExpresion, IDefaultSettings
    {
        private static readonly byte[] EmptyDelimiter = new byte[0];

        private readonly string profile;

        private readonly MapperConfig mapperConfig;

        private readonly Dictionary<Type, Padding> paddingOfType = new Dictionary<Type, Padding>();

        private readonly Dictionary<Type, byte> paddingBytesOfType = new Dictionary<Type, byte>();

        private readonly Dictionary<Type, bool> trimOfType = new Dictionary<Type, bool>();

        private readonly Dictionary<Type, bool> nullIfEmptyOfType = new Dictionary<Type, bool>();

        private readonly Dictionary<Type, byte[]> nullValueOfType = new Dictionary<Type, byte[]>();

        private readonly Dictionary<Type, IValueConverter> formatterOfType = new Dictionary<Type, IValueConverter>();

        private Padding defaultPadding = Padding.Right;

        private byte defaultPaddingByte;

        private bool defaultTrim = true;

        private bool defaultNullIfEmpty = true;

        private byte[] defaultNullValue;

        private IValueConverter defaultConverter = new DefaultConverter();

        private byte defaultFiller;

        private byte[] defaultDelimiter;

        /// <summary>
        ///
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="mapperConfig"></param>
        public MapperConfigurationExpression(string profile, MapperConfig mapperConfig)
        {
            this.profile = profile;
            this.mapperConfig = mapperConfig;
        }

        public IMapperConfigurationExpresion DefaultFiller(byte value)
        {
            defaultFiller = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultDelimiter(byte[] value)
        {
            defaultDelimiter = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultEncording(Encoding encoding)
        {
            mapperConfig.Encoding = encoding;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Padding padding)
        {
            defaultPadding = padding;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(byte filler)
        {
            defaultPaddingByte = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Padding padding, byte filler)
        {
            defaultPadding = padding;
            defaultPaddingByte = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultTrim(bool value)
        {
            defaultTrim = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullIfEmpty(bool value)
        {
            defaultNullIfEmpty = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullValue(byte[] value)
        {
            defaultNullValue = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultConverter(IValueConverter value)
        {
            defaultConverter = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, byte filler)
        {
            paddingBytesOfType[type] = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction)
        {
            paddingOfType[type] = direction;
            return this;
        }

        public IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction, byte filler)
        {
            paddingOfType[type] = direction;
            paddingBytesOfType[type] = filler;
            return this;
        }

        public IMapperConfigurationExpresion DefaultTrim(Type type, bool value)
        {
            trimOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultNullIfEmpty(Type type, bool value)
        {
            nullIfEmptyOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultValue(Type type, byte[] value)
        {
            nullValueOfType[type] = value;
            return this;
        }

        public IMapperConfigurationExpresion DefaultConverter(Type type, IValueConverter value)
        {
            formatterOfType[type] = value;
            return this;
        }

        public Encoding GetEncoding()
        {
            return mapperConfig.Encoding;
        }

        Padding IDefaultSettings.GetPadding(Type type)
        {
            Padding value;
            if (paddingOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && paddingOfType.TryGetValue(valueType, out value) ? value : defaultPadding;
        }

        byte IDefaultSettings.GetPaddingByte(Type type)
        {
            byte value;
            if (paddingBytesOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && paddingBytesOfType.TryGetValue(valueType, out value) ? value : defaultPaddingByte;
        }

        bool IDefaultSettings.GetTrim(Type type)
        {
            bool value;
            if (trimOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && trimOfType.TryGetValue(valueType, out value) ? value : defaultTrim;
        }

        bool IDefaultSettings.GetNullIfEmpty(Type type)
        {
            bool value;
            if (nullIfEmptyOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && nullIfEmptyOfType.TryGetValue(valueType, out value) ? value : defaultNullIfEmpty;
        }

        public byte[] GetNullValue(Type type)
        {
            byte[] value;
            if (nullValueOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && nullValueOfType.TryGetValue(valueType, out value) ? value : defaultNullValue;
        }

        IValueConverter IDefaultSettings.GetConverter(Type type)
        {
            IValueConverter value;
            if (formatterOfType.TryGetValue(type, out value))
            {
                return value;
            }

            var valueType = Nullable.GetUnderlyingType(type);
            return (valueType != null) && formatterOfType.TryGetValue(valueType, out value) ? value : defaultConverter;
        }

        public ITypeConfigurationExpression<T> CreateMap<T>(int length)
        {
            return CreateMap<T>(length, defaultFiller, defaultDelimiter);
        }

        public ITypeConfigurationExpression<T> CreateMap<T>(int length, byte filler)
        {
            return CreateMap<T>(length, filler, defaultDelimiter);
        }

        public ITypeConfigurationExpression<T> CreateMap<T>(int length, byte[] delimiter)
        {
            return CreateMap<T>(length, defaultFiller, delimiter);
        }

        public ITypeConfigurationExpression<T> CreateMap<T>(int length, byte filler, byte[] delimiter)
        {
            var typeMapper = new TypeMapper(length, filler, delimiter ?? EmptyDelimiter);
            mapperConfig.AddTypeMapper(profile, typeof(T), typeMapper);
            return new TypeConfigurationExpression<T>(this, typeMapper);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        public void CreateMap(Type type)
        {
            // TODO Attrbute version
            throw new NotImplementedException();
        }
    }
}