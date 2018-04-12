﻿namespace Smart.IO.ByteMapper.Expressions
{
    using System;
    using System.Text;

    using Smart.Functional;

    using Xunit;

    public class FillerExpressionTest
    {
        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByConstExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller((byte)' ')
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<FillerExpressionObject>(4)
                        .AutoFiller(true)
                        .Filler(0, 1)
                        .Filler(1, 1, (byte)'0')
                        .Filler(1)
                        .Filler(1, (byte)'_');
                })
                .ToMapperFactory();
            var mapper = mapperFactory.Create<FillerExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new FillerExpressionObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes(" 0 _"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapFillerExpression(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new MapFillerExpression(-1, 0x00));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class FillerExpressionObject
        {
        }
    }
}
