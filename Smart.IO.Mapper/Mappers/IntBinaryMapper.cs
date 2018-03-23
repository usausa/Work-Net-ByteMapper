﻿namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class BigEndianIntBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public int Length => 4;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public BigEndianIntBinaryMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, ByteOrder.GetIntBE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutIntBE(buffer, index + offset, (int)getter(target));
        }
    }

    public sealed class LittleEndianIntBinaryMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        public int Length => 4;

        public bool CanRead => getter != null;

        public bool CanWrite => setter != null;

        public LittleEndianIntBinaryMapper(
            int offset,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            this.offset = offset;
            this.getter = getter;
            this.setter = setter;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            setter(target, ByteOrder.GetIntLE(buffer, index + offset));
        }

        public void Write(byte[] buffer, int index, object target)
        {
            ByteOrder.PutIntLE(buffer, index + offset, (int)getter(target));
        }
    }
}
