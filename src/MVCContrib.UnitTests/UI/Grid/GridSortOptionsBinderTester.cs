using System.Web.Mvc;
using MvcContrib.UI.Grid;
using NUnit.Framework;

namespace MvcContrib.UnitTests.UI.Grid
{
	[TestFixture]
	public class GridSortOptionsBinderTester
	{
		[Test]
		public void Binds_sort_order_ascending()
		{
			var form = new FormCollection
			{
				{"SortDirection", "Ascending"},
			};

			var binder = new GridSortOptionsBinder();
			var result = (GridSortOptions)binder.BindModel(new ControllerContext(), CreateBindingContext(form));
			result.SortDirection.ShouldEqual(SortDirection.Ascending);
		}


		[Test]
		public void Binds_sort_order_descending()
		{
			var form = new FormCollection
			{
				{"SortDirection", "Descending"},
			};

			var binder = new GridSortOptionsBinder();
			var result = (GridSortOptions)binder.BindModel(new ControllerContext(), CreateBindingContext(form));
			result.SortDirection.ShouldEqual(SortDirection.Descending);
		}

		[Test]
		public void Binds_sort_column()
		{
			var form = new FormCollection
			{
				{"Column", "Surname"}
			};

			var binder = new GridSortOptionsBinder();
			var result = (GridSortOptions)binder.BindModel(new ControllerContext(), CreateBindingContext(form));
			result.Column.ShouldEqual("Surname");
		}

		[Test]
		public void Binds_with_prefix()
		{
			var form = new FormCollection 
			{
				{ "grid.Column", "Surname" },
				{ "grid.SortDirection", "Descending" }
			};

			var binder = new GridSortOptionsBinder();
			var context = CreateBindingContext(form);
			context.ModelName = "grid";

			var result = (GridSortOptions)binder.BindModel(new ControllerContext(), CreateBindingContext(form));
			result.Column.ShouldEqual("Surname");
			result.SortDirection.ShouldEqual(SortDirection.Descending);

		}

		private ModelBindingContext CreateBindingContext(FormCollection form)
		{
			var context = new ModelBindingContext
			{
				ValueProvider = form.ToValueProvider(),
				ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(GridSortOptions)),
				ModelState = new ModelStateDictionary()
			};

			return context;
		}
	}
}