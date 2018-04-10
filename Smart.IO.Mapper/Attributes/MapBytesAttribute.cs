﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    public sealed class MapBytesAttribute : AbstractMemberMapAttribute
    {
        private readonly BytesConverterBuilder builder = new BytesConverterBuilder();

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public MapBytesAttribute(int offset, int length)
            : base(offset)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public override IMapConverterBuilder GetConverterBuilder()
        {
            return builder;
        }
    }
}
