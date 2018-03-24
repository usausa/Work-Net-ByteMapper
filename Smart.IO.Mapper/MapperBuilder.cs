﻿namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public class MapperBuilder
    {
        private readonly IDictionary<string, object> parameters;

        private readonly IDictionary<MappingKey, MappingEntry> mappings;

        // TODO cache

        public IComponentContainer Components { get; }

        public MapperBuilder(IMapperBuilderConfig config)
        {
            Components = config.ResolveComponents();
            parameters = config.ResolveParameters();
            mappings = config.ResolveMappings();
        }

        public ITypeMapper<T> Create<T>()
        {
            // TODO
            return null;
        }
    }
}
