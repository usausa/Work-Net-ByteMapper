﻿namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    public interface IConstSyntax
    {
        // TODO
    }

    public sealed class ConstMapBuilder : ITypeMapFactory, IConstSyntax
    {
        // TODO
        public int Offset { get; set; }

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }

    public static class ConstMapExtensions
    {
        // TODO
    }
}
