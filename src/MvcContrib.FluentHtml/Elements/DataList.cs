using System.Collections.Generic;
using MvcContrib.FluentHtml.Behaviors;

namespace MvcContrib.FluentHtml.Elements
{
	/// <summary>
	/// A datalist element.
	/// </summary>
	public class DataList : DataListBase<DataList>
	{
		public DataList() {}

		public DataList(IEnumerable<IBehaviorMarker> behaviors)
			: base(behaviors) { }
	}
}