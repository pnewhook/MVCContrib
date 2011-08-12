using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using MvcContrib.FluentHtml.Behaviors;
using MvcContrib.FluentHtml.Html;

namespace MvcContrib.FluentHtml.Elements
{
	/// <summary>
	/// Base class for select elements.
	/// </summary>
	/// <typeparam name="T">The derived type.</typeparam>
	public abstract class SelectBase<T> : OptionsElementBase<T> where T : SelectBase<T>
	{
		protected Option _firstOption;
		protected bool _hideFirstOption;
		protected Action<Option, object, int> _optionModifier;
		protected Func<object, object> _groupSelectory;

		protected SelectBase(string name) : base(HtmlTag.Select, name) {}

		protected SelectBase(string name, MemberExpression forMember, IEnumerable<IBehaviorMarker> behaviors)
			: base(HtmlTag.Select, name, forMember, behaviors) {}

		/// <summary>
		/// Set the 'size' attribute.
		/// </summary>
		/// <param name="value">The value of the 'size' attribute.</param>
		/// <returns></returns>
		public virtual T Size(int value)
		{
			Attr(HtmlAttribute.Size, value);
			return (T)this;
		}

		/// <summary>
		/// Uses the specified open as the first option in the select.
		/// </summary>
		/// <param name="firstOption">The first option</param>
		/// <returns></returns>
		public virtual T FirstOption(Option firstOption)
		{
			_firstOption = firstOption;
			return (T)this;
		}

		/// <summary>
		/// Specifies the text for the first option. The value for the first option will be an empty string.
		/// </summary>
		/// <param name="text">The text for the first option</param>
		/// <returns></returns>
		public virtual T FirstOption(string text)
		{
			FirstOption(null, text);
			return (T)this;
		}

		/// <summary>
		/// Specifies the value and text for the first option.
		/// </summary>
		/// <param name="value">The value for the first option. If ommitted, will default to an empty string.</param>
		/// <param name="text">The text for the first option. If ommitted, will the default to an empty string.</param>
		/// <returns></returns>
		public virtual T FirstOption(string value, string text)
		{
			_firstOption = new Option().Text(text ?? string.Empty).Value(value ?? string.Empty).Selected(false);
			return (T)this;
		}

		[Obsolete("Use the 'FirstOption' method instead.")]
		public virtual T FirstOptionText(string firstOptionText)
		{
			FirstOption(firstOptionText);
			return (T)this;
		}

		/// <summary>
		/// Hides the first option when the value passed in is true. 
		/// </summary>
		/// <param name="hideFirstOption">True to hide the first option, otherwise false.</param>
		/// <returns></returns>
		public virtual T HideFirstOptionWhen(bool hideFirstOption)
		{
			_hideFirstOption = hideFirstOption;
			return (T)this;
		}

		/// <summary>
		/// An action performed after each Option element has been created.  This is useful to
		/// modify the element before is rendered.
		/// </summary>
		/// <param name="action">The action to perform. The parameters to the action are the Option element, 
		/// the option item, and the position of the option.</param>
		public virtual T EachOption(Action<Option, object, int> action)
		{
			_optionModifier = action;
			return (T)this;
		}

		public virtual T Options<TSource, TKey>(IEnumerable<TSource> values, string valueField, string textField, Func<TSource, TKey> groupSelector)
		{
			_groupSelectory = x => groupSelector((TSource)x);
			return Options(values, valueField, textField);
		}

		public virtual T Options<TSource>(IEnumerable<TSource> values, Func<TSource, object> valueFieldSelector, Func<TSource, object> textFieldSelector, Func<TSource, object> groupSelector)
		{
			_groupSelectory = x => groupSelector((TSource)x);
			return Options(values, valueFieldSelector, textFieldSelector);
		}

		protected override TagRenderMode TagRenderMode
		{
			get { return TagRenderMode.Normal; }
		}

		protected override string RenderOptions()
		{
			var sb = new StringBuilder();

			var i = 0;

			if(_firstOption != null && _hideFirstOption == false)
			{
				var option = GetFirstOption();
				if (_optionModifier != null)
				{
					_optionModifier(option, _firstOption, i);
				}
				sb.Append(option);
				i++;
			}

			if (_groupSelectory != null)
			{
				var options = _options.Cast<object>();
				foreach (var group in options.GroupBy(_groupSelectory))
				{
					sb.AppendFormat("<optgroup label=\"{0}\">", group.Key);
					sb.Append(RenderOptions(group.AsEnumerable(), i));
					sb.Append("</optgroup>");
				}
			}
			else
			{
				sb.Append(RenderOptions(_options, i));
			}

			return sb.ToString();
		}

		private StringBuilder RenderOptions(IEnumerable options, int i)
		{
			var sb = new StringBuilder();
			foreach ( var opt in options )
			{
				var option = GetOption( opt );
				if ( _optionModifier != null )
				{
					_optionModifier( option, opt, i );
				}
				sb.Append( option );
				i++;
			}
			return sb;
		}

		protected virtual Option GetFirstOption()
		{
			return _firstOption;
		}

		protected virtual Option GetOption(object option)
		{
			var value = _valueFieldSelector(option);
			var text = _textFieldSelector(option);

			return new Option()
				.Value(value == null ? string.Empty : value.ToString())
				.Text(text == null ? string.Empty : text.ToString())
				.Selected(IsSelectedValue(value));
		}
	}
}