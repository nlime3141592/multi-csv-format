using System;
using System.Reflection;
using System.Text;

namespace nl
{
    public static class Log
    {
        public static string LogFields<T>(this T obj, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            StringBuilder builder = new StringBuilder();
            Type type = typeof(T);

            builder.Append($"{type.Name}");

            foreach (FieldInfo fieldInfo in type.GetFields(flags))
            {
                TypeCode tCode = Type.GetTypeCode(fieldInfo.FieldType);

                switch (tCode)
                {
                    case TypeCode.Empty:
                    case TypeCode.Object:
                    case TypeCode.DBNull:
                        continue;
                    default:
                        builder.Append($",  {fieldInfo.Name} == {fieldInfo.GetValue(obj).ToString()}");
                        break;
                }
            }

            return builder.ToString();
        }
    }
}