﻿namespace Smart.IO.Mapper
{
    using System;
    using Smart.IO.Mapper.Expressions;

    public static class ByteMapperConfigExpressionExtensions
    {
        public static ITypeConfigSyntax<T> MapByExpression<T>(this ByteMapperConfig config)
        {
            var builder = new MapBuilder<T>(typeof(T));
            config.AddMapping(builder);
            return builder;
        }

        public static ITypeConfigSyntax<object> MapByExpression(this ByteMapperConfig config, Type type)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            config.AddMapping(builder);
            return builder;
        }

        public static IByteMapperConfig MapByExpression<T>(this ByteMapperConfig config, Action<ITypeConfigSyntax<T>> action)
        {
            var builder = new MapBuilder<T>(typeof(T));
            action(builder);
            config.AddMapping(builder);
            return config;
        }

        public static IByteMapperConfig MapByExpression(this ByteMapperConfig config, Type type, Action<ITypeConfigSyntax<object>> action)
        {
            // TODO ?
            var builder = new MapBuilder<object>(type);
            action(builder);
            config.AddMapping(builder);
            return config;
        }

        // TODO for TypeMap

        // Const
        // Filler

        // TODO expressionが必要か？、引数のバージョンで済まないか？

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax)
        {
            // TODO
            //var builder = new FillerMapBuilder(length);
            //syntax.AddMapper()
            return syntax;
        }

        public static ITypeConfigSyntax<T> Filler<T>(this ITypeConfigSyntax<T> syntax, int offset)
        {
            // TODO
            //var builder = new FillerMapBuilder(length);
            //syntax.AddMapper()
            return syntax;
        }

        // TODO for PropetyMap/Property

        // Array
        // Binary
        // Bool
        // Byte
        // Bytes
        // DateTime
        // Number
        // String
    }
}
