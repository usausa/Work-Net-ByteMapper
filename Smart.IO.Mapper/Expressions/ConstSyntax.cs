﻿namespace Smart.IO.Mapper.Expressions
{
    using System;

    using Smart.ComponentModel;
    using Smart.IO.Mapper.Helpers;
    using Smart.IO.Mapper.Mappers;

    internal sealed class ConstMapBuilder : ITypeMapFactory
    {
        private readonly byte[] content;

        public ConstMapBuilder(byte[] content)
        {
            this.content = content;
        }

        public int CalcSize(Type type)
        {
            return content.Length;
        }

        public IMapper CreateMapper(IComponentContainer components, IMappingParameter parameters, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
