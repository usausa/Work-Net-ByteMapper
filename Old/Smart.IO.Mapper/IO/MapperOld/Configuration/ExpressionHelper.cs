﻿namespace Smart.IO.MapperOld.Configuration
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetMemberName(Expression expr)
        {
            var mi = GetMemberInfo(expr);
            if (mi == null)
            {
                throw new ArgumentException("Expression is invalid.", nameof(expr));
            }

            return mi.Name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo(Expression expr)
        {
            while (true)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Convert:
                        expr = ((UnaryExpression)expr).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expr = ((LambdaExpression)expr).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = (MemberExpression)expr;
                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException("Expression is invalid.", nameof(expr));
                        }

                        var member = memberExpression.Member;
                        return member;
                    default:
                        return null;
                }
            }
        }
    }
}