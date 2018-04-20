﻿namespace Smart.IO.ByteMapper.Benchmark
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using BenchmarkDotNet.Attributes;

    using Smart.IO.ByteMapper;
    using Smart.IO.ByteMapper.Builders;
    using Smart.IO.ByteMapper.Converters;
    using Smart.Text.Japanese;

    [Config(typeof(BenchmarkConfig))]
    public class ConverterBenchmark
    {
        private const string Text20Single10 = "abcdefghijklmnoqrstu";
        private const string Text20Wide5 = "あいうえお";
        private const string Text20Empty = "";

        private const string Ascii13 = "123456789012X";

        private const short ZeroShort = 0;
        private const int ZeroInteger = 0;
        private const long ZeroLong = 0L;
        private const decimal ZeroDecimal = 0m;

        private const short Length4Integer = 9999;
        private const int Length8Integer = 99999999;
        private const long Length18Integer = 999999999999999999;

        private const decimal Length8Decimal = 999999.99m;
        private const decimal Length18Decimal = 999999999999999.999m;

        private static readonly byte[] Bytes10 = new byte[10];

        private static readonly byte[] Bytes20 = new byte[20];

        private static readonly DateTime DateTime8 = new DateTime(2000, 12, 31);
        private static readonly DateTime DateTime14 = new DateTime(2000, 12, 31, 23, 59, 59);
        private static readonly DateTime DateTime17 = new DateTime(2000, 12, 31, 23, 59, 59, 999);

        // Default

        private IMapConverter intBinaryConverter;
        private byte[] intBinaryBuffer;

        private IMapConverter booleanConverter;
        private byte[] booleanBuffer;

        private IMapConverter bytes10Converter;
        private byte[] bytes10Buffer;
        private IMapConverter bytes20Converter;
        private byte[] bytes20Buffer;

        private IMapConverter text20Converter;
        private byte[] text20Single20Buffer;
        private byte[] text20Wide10Buffer;
        private byte[] text20EmptyBuffer;

        private IMapConverter text13Converter;
        private IMapConverter ascii13Converter;
        private byte[] ascii13Buffer;

        private IMapConverter short4Converter;
        private byte[] short4ZeroBuffer;
        private byte[] short4MaxBuffer;

        private IMapConverter int8Converter;
        private byte[] int8ZeroBuffer;
        private byte[] int8MaxBuffer;

        private IMapConverter long18Converter;
        private byte[] long18ZeroBuffer;
        private byte[] long18MaxBuffer;

        private IMapConverter decimal8Converter;
        private byte[] decimal8ZeroBuffer;
        private byte[] decimal8MaxBuffer;

        private IMapConverter decimal18Converter;
        private byte[] decimal18ZeroBuffer;
        private byte[] decimal18MaxBuffer;

        private IMapConverter dateTime8Converter;
        private byte[] dateTime8Buffer;

        private IMapConverter dateTime14Converter;
        private byte[] dateTime14Buffer;

        private IMapConverter dateTime17Converter;
        private byte[] dateTime17Buffer;

        // Options

        private IMapConverter numberText8Converter;
        private byte[] numberText8ZeroBuffer;
        private byte[] numberText8MaxBuffer;

        private IMapConverter dateTimeText14Converter;
        private byte[] dateTimeText14Buffer;

        private static IBuilderContext CreateBuilderContext()
        {
            var config = new MapperFactoryConfig()
                .UseOptionsDefault();
            config.DefaultEncoding(SjisEncoding.Instance);

            return new BuilderContext(
                ((IMapperFactoryConfig)config).ResolveComponents(),
                ((IMapperFactoryConfig)config).ResolveParameters(),
                new Dictionary<string, object>());
        }

        [GlobalSetup]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var context = CreateBuilderContext();

            // Default

            // Binary
            var intBinaryBuilder = new BinaryConverterBuilder();
            intBinaryConverter = intBinaryBuilder.CreateConverter(context, typeof(int));
            intBinaryBuffer = new byte[intBinaryBuilder.CalcSize(typeof(int))];

            // Boolean
            var booleanBuilder = new BooleanConverterBuilder();
            booleanConverter = booleanBuilder.CreateConverter(context, typeof(bool));
            booleanBuffer = new byte[] { 0x30 };

            // bytes
            var bytes10Builder = new BytesConverterBuilder { Length = 10 };
            bytes10Converter = bytes10Builder.CreateConverter(context, typeof(byte[]));
            bytes10Buffer = new byte[bytes10Builder.CalcSize(typeof(byte[]))];

            var bytes20Builder = new BytesConverterBuilder { Length = 20 };
            bytes20Converter = bytes20Builder.CreateConverter(context, typeof(byte[]));
            bytes20Buffer = new byte[bytes20Builder.CalcSize(typeof(byte[]))];

            // Text
            var text20Builder = new TextConverterBuilder { Length = 20 };
            text20Converter = text20Builder.CreateConverter(context, typeof(string));
            text20Single20Buffer = SjisEncoding.GetFixedBytes(Text20Single10, 20);
            text20Wide10Buffer = SjisEncoding.GetFixedBytes(Text20Wide5, 20);
            text20EmptyBuffer = SjisEncoding.GetFixedBytes(Text20Empty, 20);

            // ASCII
            var text13Builder = new TextConverterBuilder { Length = 13, Encoding = Encoding.ASCII };
            text13Converter = text13Builder.CreateConverter(context, typeof(string));
            var ascii13Builder = new AsciiConverterBuilder { Length = 13 };
            ascii13Converter = ascii13Builder.CreateConverter(context, typeof(string));
            ascii13Buffer = new byte[ascii13Builder.CalcSize(typeof(string))];

            // Integer
            var short4Builder = new IntegerConverterBuilder { Length = 4 };
            short4Converter = short4Builder.CreateConverter(context, typeof(short));
            short4MaxBuffer = SjisEncoding.GetFixedBytes(Length4Integer.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);
            short4ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroShort.ToString(CultureInfo.InvariantCulture), 4, FixedAlignment.Right);

            var int8Builder = new IntegerConverterBuilder { Length = 8 };
            int8Converter = int8Builder.CreateConverter(context, typeof(int));
            int8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            int8ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            var long18Builder = new IntegerConverterBuilder { Length = 18 };
            long18Converter = long18Builder.CreateConverter(context, typeof(long));
            long18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Integer.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);
            long18ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroLong.ToString(CultureInfo.InvariantCulture), 18, FixedAlignment.Right);

            // Decimal
            var decimal8Builder = new DecimalConverterBuilder { Length = 10, Scale = 2 };
            decimal8Converter = decimal8Builder.CreateConverter(context, typeof(decimal));
            decimal8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Decimal.ToString(CultureInfo.InvariantCulture), 10, FixedAlignment.Right);
            decimal8ZeroBuffer = SjisEncoding.GetFixedBytes("0.00", 10, FixedAlignment.Right);

            var decimal18Builder = new DecimalConverterBuilder { Length = 20, Scale = 3 };
            decimal18Converter = decimal18Builder.CreateConverter(context, typeof(decimal));
            decimal18MaxBuffer = SjisEncoding.GetFixedBytes(Length18Decimal.ToString(CultureInfo.InvariantCulture), 20, FixedAlignment.Right);
            decimal18ZeroBuffer = SjisEncoding.GetFixedBytes("0.000", 20, FixedAlignment.Right);

            // DateTime
            var dateTime8Builder = new DateTimeConverterBuilder { Format = "yyyyMMdd" };
            dateTime8Converter = dateTime8Builder.CreateConverter(context, typeof(DateTime));
            dateTime8Buffer = SjisEncoding.GetFixedBytes(DateTime8.ToString("yyyyMMdd"), 8);

            var dateTime14Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmss" };
            dateTime14Converter = dateTime14Builder.CreateConverter(context, typeof(DateTime));
            dateTime14Buffer = SjisEncoding.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 14);

            var dateTime17Builder = new DateTimeConverterBuilder { Format = "yyyyMMddHHmmssfff" };
            dateTime17Converter = dateTime17Builder.CreateConverter(context, typeof(DateTime));
            dateTime17Buffer = SjisEncoding.GetFixedBytes(DateTime17.ToString("yyyyMMddHHmmssfff"), 17);

            // Options

            // Number
            var numberText8Builder = new NumberTextConverterBuilder { Length = 8 };
            numberText8Converter = numberText8Builder.CreateConverter(context, typeof(int));
            numberText8MaxBuffer = SjisEncoding.GetFixedBytes(Length8Integer.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);
            numberText8ZeroBuffer = SjisEncoding.GetFixedBytes(ZeroInteger.ToString(CultureInfo.InvariantCulture), 8, FixedAlignment.Right);

            // DateTime
            var dateTimeText14Builder = new DateTimeTextConverterBuilder { Length = 14 };
            dateTimeText14Converter = dateTimeText14Builder.CreateConverter(context, typeof(DateTime));
            dateTimeText14Buffer = SjisEncoding.GetFixedBytes(DateTime14.ToString("yyyyMMddHHmmss"), 14);
        }

        //--------------------------------------------------------------------------------
        // Read
        //--------------------------------------------------------------------------------

        // Binary

        [Benchmark]

        public void ReadIntBinary()
        {
            intBinaryConverter.Read(intBinaryBuffer, 0);
        }

        // Boolean

        [Benchmark]

        public void ReadBoolean()
        {
            booleanConverter.Read(booleanBuffer, 0);
        }

        // Bytes

        [Benchmark]
        public void ReadBytes10()
        {
            bytes10Converter.Read(bytes10Buffer, 0);
        }

        [Benchmark]
        public void ReadBytes20()
        {
            bytes20Converter.Read(bytes20Buffer, 0);
        }

        // Text

        [Benchmark]
        public void ReadSjisText20Single20()
        {
            text20Converter.Read(text20Single20Buffer, 0);
        }

        [Benchmark]
        public void ReadSjisText20Wide5()
        {
            text20Converter.Read(text20Wide10Buffer, 0);
        }

        [Benchmark]
        public void ReadSjisText20Empty()
        {
            text20Converter.Read(text20EmptyBuffer, 0);
        }

        // ASCII

        [Benchmark]
        public void ReadText13Code()
        {
            text13Converter.Read(ascii13Buffer, 0);
        }

        [Benchmark]
        public void ReadAscii13Code()
        {
            ascii13Converter.Read(ascii13Buffer, 0);
        }

        // Integer

        [Benchmark]
        public void ReadIntegerShort4Zero()
        {
            short4Converter.Read(short4ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadIntegerShort4Max()
        {
            short4Converter.Read(short4MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadInteger8Zero()
        {
            int8Converter.Read(int8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadInteger8Max()
        {
            int8Converter.Read(int8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadLong18Zero()
        {
            long18Converter.Read(long18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadLong18Max()
        {
            long18Converter.Read(long18MaxBuffer, 0);
        }

        // Decimal

        [Benchmark]
        public void ReadDecimal8Zero()
        {
            decimal8Converter.Read(decimal8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal8Max()
        {
            decimal8Converter.Read(decimal8MaxBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal18Zero()
        {
            decimal18Converter.Read(decimal18ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadDecimal18Max()
        {
            decimal18Converter.Read(decimal18MaxBuffer, 0);
        }

        // DateTime

        [Benchmark]
        public void ReadDateTime8()
        {
            dateTime8Converter.Read(dateTime8Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTime14()
        {
            dateTime14Converter.Read(dateTime14Buffer, 0);
        }

        [Benchmark]
        public void ReadDateTime17()
        {
            dateTime17Converter.Read(dateTime17Buffer, 0);
        }

        // Number

        [Benchmark]
        public void ReadNumberText8Zero()
        {
            numberText8Converter.Read(numberText8ZeroBuffer, 0);
        }

        [Benchmark]
        public void ReadNumberText8Max()
        {
            numberText8Converter.Read(numberText8MaxBuffer, 0);
        }

        // DateTime

        [Benchmark]
        public void ReadDateTimeText14()
        {
            dateTimeText14Converter.Read(dateTimeText14Buffer, 0);
        }

        //--------------------------------------------------------------------------------
        // Write
        //--------------------------------------------------------------------------------

        // Binary

        [Benchmark]

        public void WriteIntBinary()
        {
            intBinaryConverter.Write(intBinaryBuffer, 0, 0);
        }

        // Boolean

        [Benchmark]

        public void WriteBoolean()
        {
            booleanConverter.Write(booleanBuffer, 0, false);
        }

        // Bytes

        [Benchmark]
        public void WriteBytes10()
        {
            bytes10Converter.Write(bytes10Buffer, 0, Bytes10);
        }

        [Benchmark]
        public void WriteBytes20()
        {
            bytes20Converter.Write(bytes20Buffer, 0, Bytes20);
        }

        // Text

        [Benchmark]
        public void WriteSjisText20Single20()
        {
            text20Converter.Write(text20Single20Buffer, 0, Text20Single10);
        }

        [Benchmark]
        public void WriteSjisText20Wide5()
        {
            text20Converter.Write(text20Wide10Buffer, 0, Text20Wide5);
        }

        [Benchmark]
        public void WriteSjisText20Empty()
        {
            text20Converter.Write(text20EmptyBuffer, 0, Text20Empty);
        }

        // ASCII

        [Benchmark]
        public void WriteText13Code()
        {
            text13Converter.Write(ascii13Buffer, 0, Ascii13);
        }

        [Benchmark]
        public void WriteAscii13Code()
        {
            ascii13Converter.Write(ascii13Buffer, 0, Ascii13);
        }

        // Integer

        [Benchmark]
        public void WriteIntegerShort4Zero()
        {
            short4Converter.Write(short4ZeroBuffer, 0, ZeroShort);
        }

        [Benchmark]
        public void WriteIntegerShort4Max()
        {
            short4Converter.Write(short4MaxBuffer, 0, Length4Integer);
        }

        [Benchmark]
        public void WriteInteger8Zero()
        {
            int8Converter.Write(int8ZeroBuffer, 0, ZeroInteger);
        }

        [Benchmark]
        public void WriteInteger8Max()
        {
            int8Converter.Write(int8MaxBuffer, 0, Length8Integer);
        }

        [Benchmark]
        public void WriteLong18Zero()
        {
            long18Converter.Write(long18ZeroBuffer, 0, ZeroLong);
        }

        [Benchmark]
        public void WriteLong18Max()
        {
            long18Converter.Write(long18MaxBuffer, 0, Length18Integer);
        }

        // Decimal

        [Benchmark]
        public void WriteDecimal8Zero()
        {
            decimal8Converter.Write(decimal8ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteDecimal8Max()
        {
            decimal8Converter.Write(decimal8MaxBuffer, 0, Length8Decimal);
        }

        [Benchmark]
        public void WriteDecimal18Zero()
        {
            decimal18Converter.Write(decimal18ZeroBuffer, 0, ZeroDecimal);
        }

        [Benchmark]
        public void WriteDecimal18Max()
        {
            decimal18Converter.Write(decimal18MaxBuffer, 0, Length18Decimal);
        }

        // DateTime

        [Benchmark]
        public void WriteDateTime8()
        {
            dateTime8Converter.Write(dateTime8Buffer, 0, DateTime8);
        }

        [Benchmark]
        public void WriteDateTime14()
        {
            dateTime14Converter.Write(dateTime14Buffer, 0, DateTime14);
        }

        [Benchmark]
        public void WriteDateTime17()
        {
            dateTime17Converter.Write(dateTime17Buffer, 0, DateTime17);
        }

        // Number

        [Benchmark]
        public void WriteNumberText8Zero()
        {
            numberText8Converter.Write(numberText8ZeroBuffer, 0, ZeroInteger);
        }

        [Benchmark]
        public void WriteNumberText8Max()
        {
            numberText8Converter.Write(numberText8MaxBuffer, 0, Length8Integer);
        }

        // DateTime

        [Benchmark]
        public void WriteDateTimeText14()
        {
            dateTimeText14Converter.Write(dateTimeText14Buffer, 0, DateTime14);
        }
    }
}
