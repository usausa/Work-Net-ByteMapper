﻿namespace Smart.IO.ByteMapper.Builders
{
    using System;
    using System.Text;

    using Smart.IO.ByteMapper.Converters;

    public sealed class TextConverterBuilder : IMapConverterBuilder
    {
        public int Length { get; set; }

        public Encoding Encoding { get; set; }

        public bool? Trim { get; set; }

        public Padding? Padding { get; set; }

        public byte? Filler { get; set; }

        public int CalcSize(Type type)
        {
            return Length;
        }

        public IMapConverter CreateConverter(IBuilderContext context, Type type)
        {
            if (type == typeof(string))
            {
                return new TextConverter(
                    Length,
                    Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding),
                    Trim ?? context.GetParameter<bool>(Parameter.Trim),
                    Padding ?? context.GetParameter<Padding>(Parameter.TextPadding),
                    Filler ?? context.GetParameter<byte>(Parameter.TextFiller));
            }

            return null;
        }
    }
}