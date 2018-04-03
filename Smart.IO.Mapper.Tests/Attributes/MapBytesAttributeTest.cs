﻿namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class MapBytesAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBytesAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(0x30)
                .MapByAttribute<BytesAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<BytesAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new BytesAttributeObject
            {
                BytesValue = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04, 0x30, 0x30, 0x30, 0x30 }, buffer);

            // Read
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0xff;
            }

            mapper.FromByte(buffer, 0, obj);

            Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0xff }, obj.BytesValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapBytesAttribute(0, 0);

            Assert.Throws<NotSupportedException>(() => attribute.Filler);

            Assert.Null(attribute.CreateConverter(new MockMappingCreateContext(), typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(8)]
        internal class BytesAttributeObject
        {
            [MapBytes(0, 4)]
            public byte[] BytesValue { get; set; }

            [MapBytes(4, 4, Filler = 0x30)]
            public byte[] CustomBytesValue { get; set; }
        }
    }
}
