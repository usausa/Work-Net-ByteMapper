﻿namespace Smart.IO.Mapper.Attributes
{
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class TypeDefaultAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseTypeDefaultAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.ASCII)
                .DefaultCulture(CultureInfo.InvariantCulture)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .DefaultNumberStyle(NumberStyles.Integer)
                .MapByAttribute<TypeDefaultAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<TypeDefaultAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new TypeDefaultAttributeObject { IntValue = 1 };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("1___"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(4)]
        [TypeDefault(Parameter.NumberPadding, Padding.Right)]
        [TypeDefault(Parameter.NumberFiller, (byte)'_')]
        internal class TypeDefaultAttributeObject
        {
            [MapNumberText(0, 4)]
            public int IntValue { get; set; }
        }
    }
}