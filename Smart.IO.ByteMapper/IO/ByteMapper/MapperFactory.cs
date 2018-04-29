﻿namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Collections.Concurrent;
    using Smart.ComponentModel;

    public sealed class MapperFactory
    {
        private readonly IDictionary<string, object> parameters;

        private readonly IDictionary<MapperKey, IMappingFactory> mappingFactories;

        private readonly ThreadsafeHashArrayMap<MapperKey, object> cache = new ThreadsafeHashArrayMap<MapperKey, object>();

        public IComponentContainer Components { get; }

        public MapperFactory(IMapperFactoryConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Components = config.ResolveComponents();
            parameters = config.ResolveParameters();
            mappingFactories = config.ResolveMappingFactories()
                .ToDictionary(x => new MapperKey(x.Type, x.Name ?? Profile.Default), x => x);
        }

        public ITypeMapper Create(Type type)
        {
            return CallGenericCreateInternal(type, null);
        }

        public ITypeMapper Create(Type type, string profile)
        {
            return CallGenericCreateInternal(type, profile);
        }

        public ITypeMapper<T> Create<T>()
        {
            return CreateInternal<T>(null);
        }

        public ITypeMapper<T> Create<T>(string profile)
        {
            return CreateInternal<T>(profile);
        }

        private ITypeMapper CallGenericCreateInternal(Type type, string profile)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var method = GetType().GetMethod(nameof(CreateInternal), BindingFlags.Instance | BindingFlags.NonPublic);
            var genericMethod = method.MakeGenericMethod(type);
            return (ITypeMapper)genericMethod.Invoke(this, new object[] { profile });
        }

        private ITypeMapper<T> CreateInternal<T>(string profile)
        {
            var type = typeof(T);
            var key = new MapperKey(type, profile ?? Profile.Default);
            if (!cache.TryGetValue(key, out var mapper))
            {
                mapper = cache.AddIfNotExist(key, CreateMapper<T>);
            }

            return (ITypeMapper<T>)mapper;
        }

        private ITypeMapper<T> CreateMapper<T>(MapperKey key)
        {
            if (!mappingFactories.TryGetValue(key, out var mappingFactory))
            {
                throw new ByteMapperException($"Mapper entry is not exist. type=[{key.Type.FullName}], profile=[{key.Profile}]");
            }

            var mapping = mappingFactory.Create(Components, parameters);
            return new TypeMapper<T>(mapping.Type, mapping.Size, mapping.Filler, mapping.Mappers);
        }
    }
}
