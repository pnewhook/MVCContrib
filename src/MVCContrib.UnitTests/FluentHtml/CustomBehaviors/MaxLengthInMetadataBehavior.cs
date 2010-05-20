using System.ComponentModel.DataAnnotations;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml.Elements;

namespace MvcContrib.UnitTests.FluentHtml.CustomBehaviors
{
	public class MaxLengthInMetadataBehavior : IBehavior<IMemberElement>
	{
		public void Execute(IMemberElement behavee)
		{
			var helper = new MemberBehaviorHelper<RangeAttribute>();
			var attribute = helper.GetAttribute(behavee);
			if (attribute != null)
			{
				behavee.Metadata.Add("maximum", attribute.Maximum);
				behavee.Metadata.Add("minimum", attribute.Minimum);
			}
		}
	}
}