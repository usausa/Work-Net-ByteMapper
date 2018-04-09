﻿namespace Smart.IO.Mapper.Converters
{
    internal sealed class BigEndianIntBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new BigEndianIntBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetIntBE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntBE(buffer, index, (int)value);
        }
    }

    internal sealed class LittleEndianIntBinaryConverter : IMapConverter
    {
        public static IMapConverter Default { get; } = new LittleEndianIntBinaryConverter();

        public object Read(byte[] buffer, int index)
        {
            return ByteOrder.GetIntLE(buffer, index);
        }

        public void Write(byte[] buffer, int index, object value)
        {
            ByteOrder.PutIntLE(buffer, index, (int)value);
        }
    }
}
