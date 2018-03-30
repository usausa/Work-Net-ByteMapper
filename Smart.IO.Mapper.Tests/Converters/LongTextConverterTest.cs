namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class LongTextConverterTest
    {
        private const int Offset = 1;

        private const int Length = 10;

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes(string.Empty.PadLeft(Length, ' ')));

        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("1".PadLeft(Length, ' ')));

        private static readonly byte[] MinusBytes = TestBytes.Offset(Offset, Encoding.ASCII.GetBytes("-1".PadLeft(Length, ' ')));

        private readonly LongTextConverter longConverter;

        private readonly LongTextConverter nullableLongConverter;

        private readonly LongTextConverter enumConverter;

        private readonly LongTextConverter nullableEnumConverter;

        public LongTextConverterTest()
        {
            longConverter = CreateConverter(typeof(long));
            nullableLongConverter = CreateConverter(typeof(long?));
            enumConverter = CreateConverter(typeof(LongEnum));
            nullableEnumConverter = CreateConverter(typeof(LongEnum?));
        }

        private static LongTextConverter CreateConverter(Type type)
        {
            return new LongTextConverter(
                Length,
                Encoding.ASCII,
                true,
                Padding.Left,
                0x20,
                NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo,
                type);
        }

        //--------------------------------------------------------------------------------
        // long
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToLongIsDefault()
        {
            Assert.Equal(0L, longConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToLong()
        {
            Assert.Equal(1L, longConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteValueLongToBuffer()
        {
            var buffer = new byte[Length + Offset];
            longConverter.Write(buffer, Offset, 1L);

            Assert.Equal(ValueBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // long?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableLongIsDefault()
        {
            Assert.Null(nullableLongConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableLong()
        {
            Assert.Equal(1L, nullableLongConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void WriteNullLongToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableLongConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToEnumIsDefault()
        {
            Assert.Equal(LongEnum.Zero, enumConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToEnum()
        {
            Assert.Equal(LongEnum.One, enumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToEnum()
        {
            Assert.Equal((LongEnum)(-1L), enumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteValueEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, LongEnum.One);

            Assert.Equal(ValueBytes, buffer);
        }

        [Fact]
        public void WriteUndefinedEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            enumConverter.Write(buffer, Offset, (LongEnum)(-1));

            Assert.Equal(MinusBytes, buffer);
        }

        //--------------------------------------------------------------------------------
        // enum?
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadEmptyToNullableEnumIsDefault()
        {
            Assert.Null(nullableEnumConverter.Read(NullBytes, Offset));
        }

        [Fact]
        public void ReadValueToNullableEnum()
        {
            Assert.Equal(LongEnum.One, nullableEnumConverter.Read(ValueBytes, Offset));
        }

        [Fact]
        public void ReadUndefinedValueToNullableEnum()
        {
            Assert.Equal((LongEnum)(-1L), nullableEnumConverter.Read(MinusBytes, Offset));
        }

        [Fact]
        public void WriteNullEnumToBuffer()
        {
            var buffer = new byte[Length + Offset];
            nullableEnumConverter.Write(buffer, Offset, null);

            Assert.Equal(NullBytes, buffer);
        }
    }
}
