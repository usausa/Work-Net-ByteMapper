﻿namespace Smart.IO.MapperOld
{
    using System;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public interface IMapperConfigurationExpresion
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultEncording(Encoding encoding);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultFiller(byte value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultDelimiter(byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="padding"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(Padding padding);

        /// <summary>
        ///
        /// </summary>
        /// <param name="padding"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(Padding padding, byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultTrim(bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultNullIfEmpty(bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultNullValue(byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultConverter(IValueConverter value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(Type type, byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="direction"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultPadding(Type type, Padding direction, byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultTrim(Type type, bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultNullIfEmpty(Type type, bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultValue(Type type, byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IMapperConfigurationExpresion DefaultConverter(Type type, IValueConverter value);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> CreateMap<T>(int length);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> CreateMap<T>(int length, byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> CreateMap<T>(int length, byte[] delimiter);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length"></param>
        /// <param name="filler"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        ITypeConfigurationExpression<T> CreateMap<T>(int length, byte filler, byte[] delimiter);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        void CreateMap(Type type);
    }
}