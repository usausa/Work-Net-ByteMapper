﻿namespace Smart.IO.Mapper.Expressions
{
    using System;
    using System.Linq.Expressions;

    public interface ITypeConfigSyntax<T>
    {
        // Type default

        ITypeConfigSyntax<T> TypeDefault(string key, object value);

        // Type setting

        ITypeConfigSyntax<T> UseFiller(bool value);

        ITypeConfigSyntax<T> UseDelimitter(bool value);

        // Mapper

        ITypeConfigSyntax<T> AddTypeMapFactory(ITypeMapFactory factory);

        ITypeConfigSyntax<T> AddTypeMapFactory(int offset, ITypeMapFactory factory);

        // ForMember

        ITypeConfigSyntax<T> ForMember(string name, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(string name, int offset, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, Action<IMemberConfigSyntax> config);

        ITypeConfigSyntax<T> ForMember(Expression<Func<T, object>> expr, int offset, Action<IMemberConfigSyntax> config);
    }
}