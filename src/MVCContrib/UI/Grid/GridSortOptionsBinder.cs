using System;
using System.Web.Mvc;

namespace MvcContrib.UI.Grid
{
	public class GridSortOptionsBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var sortOptions = new GridSortOptions();
			return sortOptions;
		}

		private string CreateSubPropertyName(string prefix, string property)
		{
			if(string.IsNullOrEmpty(prefix))
			{
				return property;
			}

			return prefix + "." + property;
		}
	}
}