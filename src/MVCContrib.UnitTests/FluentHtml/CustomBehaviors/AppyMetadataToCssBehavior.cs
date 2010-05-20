using System;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml.Elements;

namespace MvcContrib.UnitTests.FluentHtml.CustomBehaviors
{
	public class AppyMetadataToCssBehavior : IBehavior<IElement>
	{
		public void Execute(IElement element)
		{
			if (element.Metadata.Any())
			{
				var jsSerializer = new JavaScriptSerializer();
				var serializedMetadata = jsSerializer.Serialize(element.Metadata);
				var classToAdd = String.Format(CultureInfo.CurrentCulture, "{0}", serializedMetadata.Replace('\"', '\''));
				element.AddClass(classToAdd);
			}
		}
	}
}