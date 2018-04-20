﻿namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class MapDateTimeExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByDateTimeExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .UseOptionsDefault()
                .DefaultDelimiter(null)
                .DefaultTextFiller(0x20)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<DateTimeExpressionObject>(60)
                        .ForMember(
                            x => x.DateTimeValue,
                            m => m.DateTime("yyyyMMdd"))
                        .ForMember(
                            x => x.NullableDateTimeValue,
                            m => m.DateTime("yyyyMMdd"))
                        .ForMember(
                            x => x.CustomDateTimeValue,
                            m => m.DateTime("yyyyMMddHHmmss").Filler((byte)'_'))
                        .ForMember(
                            x => x.DateTimeOffsetValue,
                            m => m.DateTime("yyyyMMdd"))
                        .ForMember(
                            x => x.NullableDateTimeOffsetValue,
                            m => m.DateTime("yyyyMMdd"))
                        .ForMember(
                            x => x.CustomDateTimeOffsetValue,
                            m => m.DateTime("yyyyMMddHHmmss").Filler((byte)'_'));
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<DateTimeExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new DateTimeExpressionObject
            {
                DateTimeValue = new DateTime(2000, 12, 31, 0, 0, 0),
                DateTimeOffsetValue = new DateTimeOffset(new DateTime(2000, 12, 31, 0, 0, 0))
            };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(
                Encoding.ASCII.GetBytes(
                    "20001231" +
                    "        " +
                    "______________" +
                    "20001231" +
                    "        " +
                    "______________"),
                buffer);

            // Read
            mapper.FromByte(buffer, obj);

            mapper.FromByte(
                Encoding.ASCII.GetBytes(
                    "20010101" +
                    "20010101" +
                    "20001231235959" +
                    "20010101" +
                    "20010101" +
                    "20001231235959"),
                obj);

            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0), obj.DateTimeValue);
            Assert.Equal(new DateTime(2001, 1, 1, 0, 0, 0), obj.NullableDateTimeValue);
            Assert.Equal(new DateTime(2000, 12, 31, 23, 59, 59), obj.CustomDateTimeValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2001, 1, 1, 0, 0, 0)), obj.DateTimeOffsetValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2001, 1, 1, 0, 0, 0)), obj.NullableDateTimeOffsetValue);
            Assert.Equal(new DateTimeOffset(new DateTime(2000, 12, 31, 23, 59, 59)), obj.CustomDateTimeOffsetValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentException>(() => new MapDateTimeExpression(null));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class DateTimeExpressionObject
        {
            public DateTime DateTimeValue { get; set; }

            public DateTime? NullableDateTimeValue { get; set; }

            public DateTime? CustomDateTimeValue { get; set; }

            public DateTimeOffset DateTimeOffsetValue { get; set; }

            public DateTimeOffset? NullableDateTimeOffsetValue { get; set; }

            public DateTimeOffset? CustomDateTimeOffsetValue { get; set; }
        }
    }
}