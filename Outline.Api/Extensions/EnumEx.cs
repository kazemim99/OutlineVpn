using System;
using System.ComponentModel;
using System.Reflection;

namespace Outline.Api.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
            }
            return "";
        }
    }
}