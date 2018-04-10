﻿namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Xunit;

    public class MapArrayAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByArrayAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(0x00)
                .DefaultEndian(Endian.Big)
                .CreateMapByAttribute<ArrayAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<ArrayAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new ArrayAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
                },
                buffer);

            // Write
            obj.ArrayValue = new[] { 1, 2, 3 };
            obj.ByteArrayValue = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07 };

            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x01,
                    0x00, 0x00, 0x00, 0x02,
                    0x00, 0x00, 0x00, 0x03,
                    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07
                },
                buffer);

            // Read
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] > 0)
                {
                    buffer[i]++;
                }
            }

            mapper.FromByte(buffer, 0, obj);

            Assert.Equal(new[] { 2, 3, 4 }, obj.ArrayValue);
            Assert.Equal(new byte[] { 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 }, obj.ByteArrayValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapArrayAttribute(0);

            Assert.Throws<NotSupportedException>(() => attribute.Filler);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(19)]
        internal class ArrayAttributeObject
        {
            [MapArray(3)]
            [MapBinary(0)]
            public int[] ArrayValue { get; set; }

            [MapArray(7, Filler = 0xFF)]
            [MapByte(12)]
            public byte[] ByteArrayValue { get; set; }
        }
    }
}
