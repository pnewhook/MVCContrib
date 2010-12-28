using System.Collections.Generic;
using System.Configuration;

namespace MvcContrib.IncludeHandling.Configuration
{
	public class IncludeHandlingSectionHandler : ConfigurationSection, IIncludeHandlingSettings
	{
		private const string CSS = "css";
		private const string JS = "js";
		private IDictionary<IncludeType, IIncludeTypeSettings> _types;

		#region IIncludeHandlingSettings Members

		[ConfigurationProperty(CSS)]
		public CssTypeElement Css
		{
			get { return (CssTypeElement) this[CSS] ?? new CssTypeElement(); }
		}

		[ConfigurationProperty(JS)]
		public JsTypeElement Js
		{
			get { return (JsTypeElement) this[JS] ?? new JsTypeElement(); }
		}

		public IDictionary<IncludeType, IIncludeTypeSettings> Types
		{
            get
            {
                if (_types == null)
                {
                    _types = new Dictionary<IncludeType, IIncludeTypeSettings>
			            {
			                {IncludeType.Css, Css},
			                {IncludeType.Js, Js}
			            };
                }

                return _types;
            }
		}

		#endregion
	}
}