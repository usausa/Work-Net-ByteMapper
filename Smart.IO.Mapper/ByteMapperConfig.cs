﻿namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using Smart.ComponentModel;
    using Smart.Reflection;

    public class ByteMapperConfig : IByteMapperConfig
    {
        private readonly ComponentConfig config = new ComponentConfig();

        private readonly Dictionary<string, object> parameters = new Dictionary<string, object>();

        private readonly Dictionary<MappingKey, MappingEntry> mappings = new Dictionary<MappingKey, MappingEntry>();

        public ByteMapperConfig()
        {
            config.Add<IDelegateFactory>(DelegateFactory.Default);

            this.DefaultDelimiter(new byte[] { 0x0d, 0x0a });
            this.DefaultTextEncoding(Encoding.ASCII);
            this.DefaultNumberEncoding(Encoding.ASCII);
            this.DefaultNumberFormat(NumberFormatInfo.InvariantInfo);
            this.DefaultNumberStyle(NumberStyles.Integer);
            this.DefaultDecimalStyle(NumberStyles.Number);
            this.DefaultDateTimeFormat(DateTimeFormatInfo.InvariantInfo);
            this.DefaultDateTimeStyle(DateTimeStyles.None);
            this.DefaultTrim(true);
            this.DefaultTextPadding(Padding.Right);
            this.DefaultNumberPadding(Padding.Left);
            this.DefaultTextFiller(0x20);
            this.DefaultNumberFiller(0x20);
            this.DefaultEndian(Endian.Big);
            this.DefaultTrueValue(0x31);
            this.DefaultFalseValue(0x30);
        }

        public ByteMapperConfig Configure(Action<ComponentConfig> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(config);

            return this;
        }

        public ByteMapperConfig AddParameter<T>(T parameter)
        {
            parameters[typeof(T).Name] = parameter;

            return this;
        }

        public ByteMapperConfig AddParameter<T>(string name, T parameter)
        {
            parameters[name] = parameter;

            return this;
        }

        public ByteMapperConfig AddMapping(MappingEntry entry)
        {
            return AddMapping(string.Empty, entry);
        }

        public ByteMapperConfig AddMapping(string profile, MappingEntry entry)
        {
            mappings.Add(new MappingKey(entry.TargetType, profile ?? string.Empty), entry);

            return this;
        }

        IComponentContainer IByteMapperConfig.ResolveComponents()
        {
            return config.ToContainer();
        }

        IDictionary<string, object> IByteMapperConfig.ResolveParameters()
        {
            return new Dictionary<string, object>(parameters);
        }

        IDictionary<MappingKey, MappingEntry> IByteMapperConfig.ResolveMappings()
        {
            return new Dictionary<MappingKey, MappingEntry>(mappings);
        }
    }
}