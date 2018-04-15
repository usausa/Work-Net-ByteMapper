﻿namespace ByteHelperTest
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ByteHelper
    {
        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Fill(this byte[] array, int offset, int length, byte value)
        {
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            for (var i = 0; i < length; i++)
            {
                array[offset + i] = value;
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillUnsafe(this byte[] array, int offset, int length, byte value)
        {
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            fixed (byte* pSrc = &array[offset])
            {
                for (var i = 0; i < length; i++)
                {
                    var pDst = pSrc + i;
                    *pDst = value;
                }
            }

            return array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] FillMemoryCopy(this byte[] array, int offset, int length, byte value)
        {
            // few cost
            if ((length <= 0) || (array == null))
            {
                return array;
            }

            //if (length <= 32)
            //{
            //    for (var i = 0; i < length; i++)
            //    {
            //        array[offset + i] = value;
            //    }

            //    return array;
            //}

            fixed (byte* pSrc = &array[offset])
            {
                *pSrc = value;
                byte* pDst;

                int copy;
                for (copy = 1; copy <= length / 2; copy <<= 1)
                {
                    pDst = pSrc + copy;
                    Buffer.MemoryCopy(pSrc, pDst, length - copy, copy);
                }

                pDst = pSrc + copy;
                Buffer.MemoryCopy(pSrc, pDst, length - copy, length - copy);
            }

            return array;
        }

        //--------------------------------------------------------------------------------
        // Encoding
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] GetAsciiBytes(string str)
        {
            var length = str.Length;
            var bytes = new byte[length];

            fixed (char* pSrc = str)
            fixed (byte* pDst = &bytes[0])
            {
                var ps = pSrc;
                var pd = pDst;

                for (var i = 0; i < length; i++)
                {
                    *pd = (byte)*ps;
                    ps++;
                    pd++;
                }
            }

            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetAsciiString(byte[] bytes)
        {
            var length = bytes.Length;
            var str = new string('\0', length);

            fixed (byte* pSrc = &bytes[0])
            fixed (char* pDst = str)
            {
                var ps = pSrc;
                var pd = pDst;

                for (var i = 0; i < length; i++)
                {
                    *pd = (char)*ps;
                    ps++;
                    pd++;
                }
            }

            return str;
        }

        //--------------------------------------------------------------------------------
        // Integer
        //--------------------------------------------------------------------------------

        // TODO parse 1, 2, 3

        // TODO format

        //// TODO check, minus, trim ?
        //public static int ParseInteger(byte[] bytes, int index, int count)
        //{
        //    var end = index + count;

        //    // No check, simple version
        //    var value = 0;
        //    for (var i = index; i < end; i++)
        //    {
        //        value *= 10;
        //        value += bytes[i] - '0';
        //    }

        //    return value;
        //}

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        // TODO parse 1, 2, 3

        // TODO format

        //--------------------------------------------------------------------------------
        // DateTime
        //--------------------------------------------------------------------------------

        public static bool TryParse(byte[] bytes, int index, string format, out DateTime dateTime)
        {
            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            var milisecond = 0;
            var msPow = 100;

            for (var i = 0; i < format.Length; i++)
            {
                var value = bytes[index + i] - '0';
                if ((value >= 0) && (value < 10))
                {
                    switch (format[i])
                    {
                        case 'y':
                            year = (year * 10) + value;
                            break;
                        case 'M':
                            month = (month * 10) + value;
                            break;
                        case 'd':
                            day = (day * 10) + value;
                            break;
                        case 'H':
                            hour = (hour * 10) + value;
                            break;
                        case 'm':
                            minute = (minute * 10) + value;
                            break;
                        case 's':
                            second = (second * 10) + value;
                            break;
                        case 'f':
                            milisecond = milisecond + (value * msPow);
                            msPow /= 10;
                            break;
                        default:
                            dateTime = default;
                            return false;
                    }
                }
                else if (format[i] == 'f')
                {
                    msPow /= 10;
                }
            }

            try
            {
                dateTime = new DateTime(year, month, day, hour, minute, second, milisecond);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                dateTime = default;
                return false;
            }
        }

        // TODO format

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private const long InvDivisor = 0x1999999A;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Div10Signed(int dividend)
        {
            // signed only
            return (int)((InvDivisor * dividend) >> 32);
        }
    }
}
