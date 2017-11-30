using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Deskband.Core.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns description of enum item marked with "Description" attribute
        /// </summary>
        public static string ToDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        public static List<KeyValuePair<Enum, String>> EnumToList(this Enum en)
        {
            Type enumType = en.GetType();
            Array enumValArray = Enum.GetValues(enumType);
            var enumValList = new List<KeyValuePair<Enum, String>>(enumValArray.Length);
            foreach (int val in enumValArray)
            {
                Enum item = (Enum)Enum.Parse(enumType, val.ToString());
                enumValList.Add(new KeyValuePair<Enum, String>(item, item.ToDescription()));
            }
            return enumValList;
        }
    }
}