﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public abstract class AbstractTypeMapAttribute : Attribute
    {
        public abstract ITypeMapperBuilder GetTypeMapperBuilder();
    }
}