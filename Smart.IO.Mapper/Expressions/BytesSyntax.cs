﻿namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    public interface IByteSyntax
    {
        // TODO
    }

    public sealed class ByteMapBuilder : IPropertyMapFactory, IByteSyntax
    {
        public int Offset { get; set; } // TODO

        public int CalcSize(Type type)
        {
            throw new NotImplementedException();
        }

        public IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }

    public static class ByteMapExtensions
    {
        // TODO
    }
}
