using System;
using MvcContrib.FluentHtml.Html;

namespace MvcContrib.FluentHtml.Elements
{
    public class TypeStamp : TextInput<TypeStamp>
    {
        public const string TypeStampKey = "_xTypeStampx_";

        /// <summary>
        /// Generates an HTML input element of type 'hidden' that denotes an instance type stamp
        /// </summary>
        /// <param name="name">Value of the 'name' attribute of the element.  Also used to derive the 'id' attribute.</param>
        /// <param name="instanceType">instance type information to be recorded</param>
        public TypeStamp(string name, Type instanceType) : base(HtmlInputType.Hidden, GenerateBinderContextProperty(name)) {
            Value(instanceType.FullName); 
        }

        private static string GenerateBinderContextProperty(string name)
        {
            return String.Concat(String.IsNullOrEmpty(name) ? string.Empty : name + '.', TypeStampKey);
        }
    }
}
