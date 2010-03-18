using System;
using System.Reflection;
using System.Web.Mvc;

namespace MvcContrib.TestHelper
{
    public static class MethodInfoExtensions
    {
        public static string ActionName(this MethodInfo method)
        {
            if (method.IsDecoratedWith<ActionNameAttribute>()) return method.GetAttribute<ActionNameAttribute>().Name;

            return method.Name;
        }
    }

    public static class AttributeExtensions
    {
        public static bool IsDecoratedWith<T>(this ICustomAttributeProvider attributeTarget) where T : Attribute
        {
            return attributeTarget.GetCustomAttributes(typeof(T), false).Length > 0;
        }

        public static T GetAttribute<T>(this ICustomAttributeProvider attributeTarget) where T : Attribute
        {
            return (T)attributeTarget.GetCustomAttributes(typeof(T), false)[0];
        }
        
    }
}