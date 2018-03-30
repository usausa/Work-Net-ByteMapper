﻿namespace Smart.IO.Mapper.Converters
{
    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class NullableBoolConverterTest
    {
        private const int Offset = 1;

        private const byte TrueByte = 0x31;

        private const byte FalseByte = 0x30;

        private const byte NullByte = 0x00;

        private static readonly byte[] TrueBytes = TestBytes.Offset(Offset, new[] { TrueByte });

        private static readonly byte[] FalseBytes = TestBytes.Offset(Offset, new[] { FalseByte });

        private static readonly byte[] NullBytes = TestBytes.Offset(Offset, new[] { NullByte });

        private readonly NullableBoolConverter converter;

        public NullableBoolConverterTest()
        {
            converter = new NullableBoolConverter(TrueByte, FalseByte, NullByte);
        }

        //--------------------------------------------------------------------------------
        // bool
        //--------------------------------------------------------------------------------

        [Fact]
        public void ReadToNullableBool()
        {
            // True
            Assert.True((bool?)converter.Read(TrueBytes, Offset));

            // False
            Assert.False((bool?)converter.Read(FalseBytes, Offset));

            // Null
            Assert.Null(converter.Read(NullBytes, Offset));
        }

        [Fact]
        public void WriteNullableBoolToBuffer()
        {
            var buffer = new byte[1 + Offset];

            // True
            converter.Write(buffer, Offset, true);
            Assert.Equal(TrueBytes, buffer);

            // False
            converter.Write(buffer, Offset, false);
            Assert.Equal(FalseBytes, buffer);

            // Null
            converter.Write(buffer, Offset, null);
            Assert.Equal(NullBytes, buffer);
        }
    }
}
