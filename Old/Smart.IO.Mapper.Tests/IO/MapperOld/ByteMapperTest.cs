﻿namespace Smart.IO.MapperOld
{
    using System;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// ByteMapperTest の概要の説明
    /// </summary>
    [TestClass]
    public class ByteMapperTest
    {
        private static readonly Encoding SjisEncoding = Encoding.GetEncoding("Shift_JIS");

        // ------------------------------------------------------------
        // Basic
        // ------------------------------------------------------------

        protected class BasicEntity
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        protected class BasicProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultFiller(0x20)
                    .DefaultPadding(0x20)
                    .DefaultNullIfEmpty(true)
                    .DefaultDelimiter(new byte[] { 0x0D, 0x0A })
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<BasicEntity>(17)
                    .ForMember(x => x.Id, 5)
                    .ForMember(x => x.Name, 10);
            }
        }

        [TestMethod]
        public void TestBasic()
        {
            var builder = new MapperConfigBuilder(new BasicProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new BasicEntity { Id = 1, Name = "うさうさ" };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("    1うさうさ  \r\n"), bytes);

            var entity2 = mapper.FromByte<BasicEntity>(bytes);

            Assert.AreEqual(entity.Id, entity2.Id);
            Assert.AreEqual(entity.Name, entity2.Name);
        }

        [TestMethod]
        public void TestBasicNull()
        {
            var builder = new MapperConfigBuilder(new BasicProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = mapper.FromByte<BasicEntity>(SjisEncoding.GetBytes("               \r\n"));

            Assert.AreEqual(0, entity.Id);
            Assert.AreEqual(null, entity.Name);
        }

        // ------------------------------------------------------------
        // Nullable
        // ------------------------------------------------------------

        protected class NullableEntity
        {
            public int? Value { get; set; }
        }

        protected class NullableProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<NullableEntity>(5)
                    .ForMember(x => x.Value, 5);
            }
        }

        [TestMethod]
        public void TestNullableNull()
        {
            var builder = new MapperConfigBuilder(new NullableProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new NullableEntity { Value = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("     "), bytes);

            var entity2 = mapper.FromByte<NullableEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        [TestMethod]
        public void TestNullableHasValue()
        {
            var builder = new MapperConfigBuilder(new NullableProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new NullableEntity { Value = 123 };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("  123"), bytes);

            var entity2 = mapper.FromByte<NullableEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        // ------------------------------------------------------------
        // NullValue
        // ------------------------------------------------------------

        protected class NullValueEntity
        {
            public DateTime? Value { get; set; }
        }

        protected class NullValueProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding);

                config.CreateMap<NullValueEntity>(8)
                    .ForMember(x => x.Value, 8, c => c.DateTime("yyyyMMdd", null).NullValue(SjisEncoding.GetBytes("--------")));
            }
        }

        [TestMethod]
        public void TestNullValue()
        {
            var builder = new MapperConfigBuilder(new NullValueProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new NullValueEntity { Value = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("--------"), bytes);

            var entity2 = mapper.FromByte<NullValueEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        // ------------------------------------------------------------
        // Format
        // ------------------------------------------------------------

        protected class FormatEntity
        {
            public float Value { get; set; }
        }

        protected class FormatProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultPadding(typeof(float), Padding.Left);

                config.CreateMap<FormatEntity>(5)
                    .ForMember(x => x.Value, 5, c => c.Formatter("F2"));
            }
        }

        [TestMethod]
        public void TestFormat()
        {
            var builder = new MapperConfigBuilder(new FormatProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new FormatEntity { Value = 1.23f };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes(" 1.23"), bytes);

            var entity2 = mapper.FromByte<FormatEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        protected class ParseEntity
        {
            public int Value { get; set; }
        }

        protected class ParseProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIParseProvider", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20)
                    .DefaultPadding(typeof(int), Padding.Left);

                config.CreateMap<ParseEntity>(6)
                    .ForMember(x => x.Value, 6, c => c.Formatter("#,0"));
            }
        }

        [TestMethod]
        public void TestParse()
        {
            var builder = new MapperConfigBuilder(new ParseProfile());
            var mapper = new ByteMapper(builder.Build());

            var bytes = SjisEncoding.GetBytes(" 1,000");

            var entity = mapper.FromByte<ParseEntity>(bytes);

            Assert.AreEqual(1000, entity.Value);

            var bytes2 = mapper.ToByte(entity);

            CollectionAssert.AreEqual(bytes, bytes2);
        }

        // ------------------------------------------------------------
        // Enum
        // ------------------------------------------------------------

        protected enum EnumType
        {
            Zero,
            One,
            Two
        }

        protected class EnumEntity
        {
            public EnumType Value { get; set; }
        }

        protected class EnumProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20);

                config.CreateMap<EnumEntity>(1)
                    .ForMember(x => x.Value, 1);
            }
        }

        [TestMethod]
        public void TestEnum()
        {
            var builder = new MapperConfigBuilder(new EnumProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new EnumEntity { Value = EnumType.One };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("1"), bytes);

            var entity2 = mapper.FromByte<EnumEntity>(bytes);

            Assert.AreEqual(entity.Value, entity2.Value);
        }

        // ------------------------------------------------------------
        // DateTime
        // ------------------------------------------------------------

        protected class DateTimeEntity
        {
            public DateTime Value1 { get; set; }

            public DateTime? Value2 { get; set; }
        }

        protected class DateTimeProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20);

                config.CreateMap<DateTimeEntity>(28)
                    .ForMember(x => x.Value1, 14, c => c.DateTime("yyyyMMddHHmmss"))
                    .ForMember(x => x.Value2, 14, c => c.DateTime("yyyyMMddHHmmss"));
            }
        }

        [TestMethod]
        public void TestDateTime()
        {
            var builder = new MapperConfigBuilder(new DateTimeProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new DateTimeEntity { Value1 = new DateTime(2000, 12, 31, 23, 59, 59), Value2 = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("20001231235959              "), bytes);

            var entity2 = mapper.FromByte<DateTimeEntity>(bytes);

            Assert.AreEqual(entity.Value1, entity2.Value1);
            Assert.AreEqual(entity.Value2, entity2.Value2);
        }

        // ------------------------------------------------------------
        // Boolean
        // ------------------------------------------------------------

        protected class BooleanEntity
        {
            public bool Value1 { get; set; }

            public bool? Value2 { get; set; }
        }

        protected class BooleanProfile : MapperProfile
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Ignore")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Ignore")]
            public override void Configure(IMapperConfigurationExpresion config)
            {
                config
                    .DefaultEncording(SjisEncoding)
                    .DefaultPadding(0x20);

                config.CreateMap<BooleanEntity>(2)
                    .ForMember(x => x.Value1, 1, c => c.Boolean((byte)'1', (byte)'0'))
                    .ForMember(x => x.Value2, 1, c => c.Boolean((byte)'1', (byte)'0'));
            }
        }

        [TestMethod]
        public void TestBoolean()
        {
            var builder = new MapperConfigBuilder(new BooleanProfile());
            var mapper = new ByteMapper(builder.Build());

            var entity = new BooleanEntity { Value1 = true, Value2 = null };
            var bytes = mapper.ToByte(entity);

            CollectionAssert.AreEqual(SjisEncoding.GetBytes("1 "), bytes);

            var entity2 = mapper.FromByte<BooleanEntity>(bytes);

            Assert.AreEqual(entity.Value1, entity2.Value1);
            Assert.AreEqual(entity.Value2, entity2.Value2);
        }
    }
}