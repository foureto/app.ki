using System.Reflection;

namespace App.Ki.Clickhouse.Extensions;

internal static class ObjectExtensions
{
    public static void SetProperty(this object target, PropertyInfo propertyInfo, object value, Type fieldType)
    {
        propertyInfo.SetValue(target, value.ConvertFromDbValue(propertyInfo.PropertyType));
    }

    public static object ConvertFromDbValue(this object value, Type dbType)
    {
        return value switch
        {
            DateTimeOffset offset when dbType == typeof(DateTime) => offset.DateTime,
            string srtValue when dbType.IsEnum => Enum.Parse(dbType, srtValue),
            { } _ when dbType == typeof(string) => value.ToString(),
            byte flag when dbType == typeof(bool) => flag == 1,
            DBNull => null,
            _ => value
        };
    }
}