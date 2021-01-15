namespace Smart.IO.ByteMapper.Helpers
{
    using System;

    public static class EnumHelper
    {
        public static Type GetConvertEnumType(Type type)
        {
            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return (targetType?.IsEnum ?? false) ? targetType : null;
        }
    }
}
