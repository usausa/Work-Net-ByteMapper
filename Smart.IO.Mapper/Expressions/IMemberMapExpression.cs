﻿namespace Smart.IO.Mapper.Expressions
{
    using Smart.IO.Mapper.Builders;

    public interface IMemberMapExpression
    {
        IMapConverterBuilder GetMapConverterBuilder();
    }
}
