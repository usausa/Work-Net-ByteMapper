﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    public sealed class BoolBinaryAttribute : PropertyAttributeBase
    {
        public byte? TrueValue { get; set; }

        public byte? FalseValue { get; set; }

        public BoolBinaryAttribute(int offset)
            : base(offset)
        {
        }
    }
}
