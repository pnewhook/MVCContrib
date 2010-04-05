using System;
using MvcContrib.FluentHtml.Elements;

namespace MvcContrib.FluentHtml.Behaviors
{
	public class AutoLabelBehavior : IBehavior<IElement>
	{
		private readonly Func<IElement, bool> isQualifiedFunc;
		private readonly Func<IElement, bool> renderLabelAfterFunc;

		public AutoLabelBehavior() : this(null, null) { }

		public AutoLabelBehavior(Func<IElement, bool> isQualifiedFunc, Func<IElement, bool> renderLabelAfterFunc)
		{
			this.isQualifiedFunc = isQualifiedFunc ?? (e => !(e is Hidden) && !(e is Button) && !(e is SubmitButton));			this.renderLabelAfterFunc = renderLabelAfterFunc;
		}

		public void Execute(IElement element)
		{
			if(!isQualifiedFunc(element))
			{
				return;
			}
			if(renderLabelAfterFunc == null || !renderLabelAfterFunc(element))
			{
				element.SetAutoLabel();
			}
			else
			{
				element.SetAutoLabelAfter();
			}
		}
	}
}