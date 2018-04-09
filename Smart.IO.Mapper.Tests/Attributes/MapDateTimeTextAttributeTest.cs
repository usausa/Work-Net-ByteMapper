﻿namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class MapDateTimeTextAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByDateTimeTextAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.ASCII)
                .DefaultDateTimeProvider(CultureInfo.InvariantCulture)
                .DefaultTrim(true)
                .DefaultTextFiller(0x20)
                .DefaultDateTimeStyle(DateTimeStyles.None)
                .CreateMapByAttribute<DateTimeTextAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<DateTimeTextAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new DateTimeTextAttributeObject
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
            var attribute = new MapDateTimeTextAttribute(0, 0, string.Empty);

            Assert.Throws<NotSupportedException>(() => attribute.CodePage);
            Assert.Throws<NotSupportedException>(() => attribute.EncodingName);
            Assert.Throws<NotSupportedException>(() => attribute.Filler);
            Assert.Throws<NotSupportedException>(() => attribute.Style);
            Assert.Throws<NotSupportedException>(() => attribute.Culture);

            Assert.Null(attribute.CreateConverter(null, null, typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(60)]
        internal class DateTimeTextAttributeObject
        {
            [MapDateTimeText(0, 8, "yyyyMMdd")]
            public DateTime DateTimeValue { get; set; }

            [MapDateTimeText(8, 8, "yyyyMMdd")]
            public DateTime? NullableDateTimeValue { get; set; }

            [MapDateTimeText(16, 14, "yyyyMMddHHmmss", EncodingName = "ASCII", Filler = (byte)'_', Style = DateTimeStyles.None, Culture = Culture.Invaliant)]
            public DateTime? CustomDateTimeValue { get; set; }

            [MapDateTimeText(30, 8, "yyyyMMdd")]
            public DateTimeOffset DateTimeOffsetValue { get; set; }

            [MapDateTimeText(38, 8, "yyyyMMdd")]
            public DateTimeOffset? NullableDateTimeOffsetValue { get; set; }

            [MapDateTimeText(46, 14, "yyyyMMddHHmmss", CodePage = 20127, Filler = (byte)'_', Style = DateTimeStyles.None, Culture = Culture.Invaliant)]
            public DateTimeOffset? CustomDateTimeOffsetValue { get; set; }
        }
    }
}
