using System.Collections.Generic;
using System.Linq.Expressions;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml.Html;

namespace MvcContrib.FluentHtml.Elements
{
	/// <summary>
	/// Base type for date picker elements.
	/// </summary>
	public abstract class QuantityInputBase<T> : Input<T> where T : QuantityInputBase<T>
	{
		protected QuantityInputBase(string type, string name) : base(type, name) {}

		protected QuantityInputBase(string type, string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors) 
			: base(type, name, forMember, behaviors) {}

		/// <summary>
		/// Limit what input values are presented.
		/// </summary>
		public virtual T Limit(object min, object max, long? step)
		{
			if (min != null)
			{
				Attr(HtmlAttribute.Min, min);
			}
			if (max != null)
			{
				Attr(HtmlAttribute.Max, max);
			}
			if (step != null)
			{
				Attr(HtmlAttribute.Step, step);
			}
			return (T)this;
		}
	}
}