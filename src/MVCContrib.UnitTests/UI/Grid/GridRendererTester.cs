using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;
using MvcContrib.UI.Grid.ActionSyntax;
using NUnit.Framework;
using Rhino.Mocks;

namespace MvcContrib.UnitTests.UI.Grid
{
	[TestFixture]
	public class GridRendererTester
	{
		private List<Person> _people;
		private GridModel<Person> _model;
		private IViewEngine _viewEngine;
		private ViewEngineCollection _engines;
		private StringWriter _writer;
		private NameValueCollection _querystring;

		[SetUp]
		public void Setup()
		{
			_model = new GridModel<Person>();
			_people = new List<Person> {new Person {Id = 1, Name = "Jeremy", DateOfBirth = new DateTime(1987, 4, 19)}};
			_viewEngine = MockRepository.GenerateMock<IViewEngine>();
			_engines = new ViewEngineCollection(new List<IViewEngine> { _viewEngine });
			_writer = new StringWriter();
			_querystring= new NameValueCollection();
			RouteTable.Routes.MapRoute("default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}

		[TearDown]
		public void Teardown()
		{
			RouteTable.Routes.Clear();
		}

		private IGridColumn<Person> ColumnFor(Expression<Func<Person, object>> expression)
		{
			return _model.Column.For(expression);
		}

		[Test]
		public void Should_render_empty_table()
		{
			string expected = ExpectedEmptyTable("There is no data available.", "grid");
			RenderGrid(null).ShouldEqual(expected);
		}

	    private string ExpectedEmptyTable(string message, string @class)
	    {
	        return "<table class=\""+ @class +"\"><thead><tr><th></th></tr></thead><tbody><tr><td>" + message +"</td></tr></tbody></table>";
	    }

	    [Test]
		public void Should_render_empty_table_when_collection_is_empty()
		{
			_people.Clear();
	        string expected = ExpectedEmptyTable("There is no data available.", "grid");
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_empty_table_with_custom_message()
		{
			_people.Clear();
		    string expected = ExpectedEmptyTable("Test", "grid");// "<table class=\"grid\"><tr><td>Test</td></tr></table>";
			_model.Empty("Test");
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Custom_html_attrs()
		{
			_people.Clear();
		    string expected = ExpectedEmptyTable("There is no data available.", "sortable grid");// "<table class=\"sortable grid\"><tr><td>There is no data available.</td></tr></table>";
			_model.Attributes(@class => "sortable grid");
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Custom_html_attributes_should_be_encoded()
		{
			_people.Clear();
			string expected = ExpectedEmptyTable("There is no data available.", "&quot;foo&quot;");
			_model.Attributes(@class => "\"foo\"");
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render()
		{
			ColumnFor(x => x.Name);
			ColumnFor(x => x.Id);
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th><th>Id</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td><td>1</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_with_custom_Header()
		{
			ColumnFor(x => x.Name).Header("<td>TEST</td>");
			ColumnFor(x => x.Id);
			string expected = "<table class=\"grid\"><thead><tr><td>TEST</td><th>Id</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td><td>1</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Header_should_be_split_pascal_case()
		{
			ColumnFor(x => x.DateOfBirth).Format("{0:dd}");
			string expected =
				"<table class=\"grid\"><thead><tr><th>Date Of Birth</th></tr></thead><tbody><tr class=\"gridrow\"><td>19</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void With_format()
		{
			ColumnFor(x => x.DateOfBirth).Format("{0:ddd}");
			var dayString = string.Format("{0:ddd}", _people[0].DateOfBirth);
			dayString = HttpUtility.HtmlEncode(dayString);
			string expected = "<table class=\"grid\"><thead><tr><th>Date Of Birth</th></tr></thead><tbody><tr class=\"gridrow\"><td>" +
			                  dayString + "</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Complicated_column()
		{
			ColumnFor(x => x.Id + "-" + x.Name).Named("Test");
			string expected =
				"<table class=\"grid\"><thead><tr><th>Test</th></tr></thead><tbody><tr class=\"gridrow\"><td>1-Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Column_heading_should_be_empty()
		{
			ColumnFor(x => x.Id + "-" + x.Name);
			string expected =
				"<table class=\"grid\"><thead><tr><th></th></tr></thead><tbody><tr class=\"gridrow\"><td>1-Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Column_should_be_rendered_using_custom_partial()
		{
			SetupViewEngine("Foo", (v, w) => {
				var model = ((Person)v.ViewData.Model);
				w.Write("<td>" + model.Name + "_TEST</td>");
			});

#pragma warning disable 612,618
			ColumnFor(x => x.Name).Partial("Foo");
#pragma warning restore 612,618
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy_TEST</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Custom_column_should_use_partial_with_same_name_as_column()
		{
			SetupViewEngine("Name", "<td>Foo</td>");
#pragma warning disable 612,618
			_model.Column.For("Name");
#pragma warning restore 612,618
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Foo</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Custom_column_with_custom_partial()
		{
			SetupViewEngine("Foo", "<td>Foo</td>");
#pragma warning disable 612,618
			_model.Column.For("Name").Partial("Foo");
#pragma warning restore 612,618
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Foo</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}


		[Test]
		public void With_cell_condition()
		{
			ColumnFor(x => x.Name);
			ColumnFor(x => x.Id).CellCondition(x => false);
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th><th>Id</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td><td></td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void With_col_condition()
		{
			ColumnFor(x => x.Name);
			ColumnFor(x => x.Id).Visible(false);
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_row_start()
		{
			ColumnFor(x => x.Id);
			_model.Sections.RowStart(x => "<tr foo=\"bar\">");
			string expected = "<table class=\"grid\"><thead><tr><th>Id</th></tr></thead><tbody><tr foo=\"bar\"><td>1</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}


		[Test]
		public void Should_render_custom_row_end()
		{
			ColumnFor(x => x.Id);
			_model.Sections.RowEnd(x => "</tr>TEST");
			string expected = "<table class=\"grid\"><thead><tr><th>Id</th></tr></thead><tbody><tr class=\"gridrow\"><td>1</td></tr>TEST</tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Alternating_rows_should_have_correct_css_class()
		{
			_people.Add(new Person {Name = "Person 2"});
			_people.Add(new Person {Name = "Person 3"});
			ColumnFor(x => x.Name);
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr><tr class=\"gridrow_alternate\"><td>Person 2</td></tr><tr class=\"gridrow\"><td>Person 3</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_row_start_with_alternate_row()
		{
			_people.Add(new Person { Name = "Person 2" });
			_people.Add(new Person { Name = "Person 3" });
			ColumnFor(x => x.Name);
			_model.Sections.RowStart(row => "<tr class=\"row " + (row.IsAlternate ? "gridrow_alternate" : "gridrow") + "\">");
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"row gridrow\"><td>Jeremy</td></tr><tr class=\"row gridrow_alternate\"><td>Person 2</td></tr><tr class=\"row gridrow\"><td>Person 3</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_row_start_with_action()
		{
			_people.Add(new Person { Name = "Person 2" });
			_people.Add(new Person { Name = "Person 3" });
			ColumnFor(x => x.Name);
			_model.Sections.RowStart(c  =>
			{
				_writer.Write("<tr class=\"gridrow\">");
			});

			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr><tr class=\"gridrow\"><td>Person 2</td></tr><tr class=\"gridrow\"><td>Person 3</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}


		[Test]
		public void Should_render_custom_row_start_with_action_alternate()
		{
			_people.Add(new Person { Name = "Person 2" });
			_people.Add(new Person { Name = "Person 3" });
			ColumnFor(x => x.Name);
			_model.Sections.RowStart((c, vd) =>
			{
				_writer.Write("<tr class=\"row " + (vd.IsAlternate ? "gridrow_alternate" : "gridrow") + "\">");
			});

			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"row gridrow\"><td>Jeremy</td></tr><tr class=\"row gridrow_alternate\"><td>Person 2</td></tr><tr class=\"row gridrow\"><td>Person 3</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_header_attributes()
		{
			ColumnFor(x => x.Name).HeaderAttributes(style => "width:100%");
			string expected = "<table class=\"grid\"><thead><tr><th style=\"width:100%\">Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_encode_header_attributes()
		{
			ColumnFor(x => x.Name).HeaderAttributes(style => "\"foo\"");
			string expected = "<table class=\"grid\"><thead><tr><th style=\"&quot;foo&quot;\">Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}
		
		[Test]
		public void Should_render_header_attributes_when_rendering_custom_row_start()
		{
			ColumnFor(x => x.Name).HeaderAttributes(style => "width:100%");
			_people.Add(new Person { Name = "Person 2" });
			_people.Add(new Person { Name = "Person 3" });
			_model.Sections.RowStart(row => "<tr class=\"row " + (row.IsAlternate ? "gridrow_alternate" : "gridrow") + "\">");
			string expected = "<table class=\"grid\"><thead><tr><th style=\"width:100%\">Name</th></tr></thead><tbody><tr class=\"row gridrow\"><td>Jeremy</td></tr><tr class=\"row gridrow_alternate\"><td>Person 2</td></tr><tr class=\"row gridrow\"><td>Person 3</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}


		[Test]
		public void Custom_item_section()
		{
			ColumnFor(x => x.Name).Action(s => _writer.Write("<td>Test</td>"));
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Test</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}


		[Test]
		public void Should_render_with_custom_header_section()
		{
			ColumnFor(p => p.Name).HeaderAction(() => _writer.Write("<td>TEST</td>"));
			ColumnFor(p => p.Id);
			string expected = "<table class=\"grid\"><thead><tr><td>TEST</td><th>Id</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td><td>1</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_attributes_in_table_cell()
		{
			ColumnFor(x => x.Name).Attributes(foo => "bar");
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td foo=\"bar\">Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_encode_custom_attributes()
		{
			ColumnFor(x => x.Name).Attributes(foo => "\"bar\"");
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td foo=\"&quot;bar&quot;\">Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
			
		}

		[Test]
		public void Should_render_custom_attributes_when_Attributes_called_multiple_times()
		{
			ColumnFor(x => x.Name).Attributes(foo => "bar").Attributes(baz => "blah");
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td foo=\"bar\" baz=\"blah\">Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_attributes_in_table_cell_with_logic()
		{
			_people.Add(new Person() { Name = "foo"});
			ColumnFor(x => x.Name).Attributes(row => new Hash(foo => row.IsAlternate ? "bar" : "baz" ));
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td foo=\"baz\">Jeremy</td></tr><tr class=\"gridrow_alternate\"><td foo=\"bar\">foo</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_attributes_for_row()
		{
			ColumnFor(x => x.Name);
			_model.Sections.RowAttributes(x => new Hash(foo => "bar"));
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr foo=\"bar\" class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_css_class_for_row()
		{
			ColumnFor(x => x.Name);
			_model.Sections.RowAttributes(x => new Hash(@class => "foo"));
			string expected = "<table class=\"grid\"><thead><tr><th>Name</th></tr></thead><tbody><tr class=\"foo\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_attributes_for_header_row()
		{
			ColumnFor(x => x.Name);
			_model.Sections.HeaderRowAttributes(new Hash(foo => "bar"));
			string expected = "<table class=\"grid\"><thead><tr foo=\"bar\"><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_grid_with_sort_links()
		{
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=Name&amp;Direction=Ascending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_grid_with_sort_links_using_prefix()
		{
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions(), "foo");
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?foo.Column=Name&amp;foo.Direction=Ascending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_grid_with_sort_direction_ascending()
		{
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions() { Column = "Name" });
			string expected = "<table class=\"grid\"><thead><tr><th class=\"sort_asc\"><a href=\"/?Column=Name&amp;Direction=Descending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Direction_heading_should_not_override_custom_class()
		{
			ColumnFor(x => x.Name).HeaderAttributes(@class => "foo");
			_model.Sort(new GridSortOptions() { Column = "Name" });
			string expected = "<table class=\"grid\"><thead><tr><th class=\"foo sort_asc\"><a href=\"/?Column=Name&amp;Direction=Descending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_grid_with_sort_direction_descending()
		{
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions() { Column = "Name", Direction = SortDirection.Descending });
			string expected = "<table class=\"grid\"><thead><tr><th class=\"sort_desc\"><a href=\"/?Column=Name&amp;Direction=Ascending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_grid_with_sort_direction_descending_as_the_default()
		{
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions() {Direction = SortDirection.Descending});
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=Name&amp;Direction=Descending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);

		}

		[Test]
		public void Sorting_Maintains_existing_querystring_parameters()
		{
			_querystring["foo"] = "bar";
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=Name&amp;Direction=Ascending&amp;foo=bar\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Sorting_does_not_maintain_null_querystring_parameters()
		{
			_querystring[(string)null] = "foo";
			ColumnFor(x => x.Name);
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=Name&amp;Direction=Ascending\">Name</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_not_render_sort_links_for_columns_tha_are_not_sortable()
		{
			ColumnFor(x => x.Id);
			ColumnFor(x => x.Name).Sortable(false);
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=Id&amp;Direction=Ascending\">Id</a></th><th>Name</th></tr></thead><tbody><tr class=\"gridrow\"><td>1</td><td>Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Uses_custom_sort_column_name()
		{
			ColumnFor(x => x.Id).SortColumnName("foo");
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=foo&amp;Direction=Ascending\">Id</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>1</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Uses_custom_sort_column_name_for_composite_expression()
		{
			ColumnFor(x => x.Id + x.Name).SortColumnName("foo").Named("bar");
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=foo&amp;Direction=Ascending\">bar</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>1Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Falls_back_to_column_name_for_composite_expression()
		{
			ColumnFor(x => x.Id + x.Name).Named("bar");
			_model.Sort(new GridSortOptions());
			string expected = "<table class=\"grid\"><thead><tr><th><a href=\"/?Column=bar&amp;Direction=Ascending\">bar</a></th></tr></thead><tbody><tr class=\"gridrow\"><td>1Jeremy</td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		//TODO: Change this to use IHtmlString when we take a dependency on .NET 4.
		[Test] 
		public void Should_not_automatically_encode_IHtmlString_instances()
		{
			ColumnFor(x => MvcHtmlString.Create("<script></script>")).Named("foo");
			string expected = "<table class=\"grid\"><thead><tr><th>foo</th></tr></thead><tbody><tr class=\"gridrow\"><td><script></script></td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Should_render_custom_name_with_DisplayNameAttribute() 
		{
			ColumnFor(x => x.NameWithAttribute);
			string expected =
				"<table class=\"grid\"><thead><tr><th>Name2</th></tr></thead><tbody><tr class=\"gridrow\"><td></td></tr></tbody></table>";
			RenderGrid().ShouldEqual(expected);
		}

		[Test]
		public void Infers_format_from_DisplayFormAttribute()
		{
			ColumnFor(x => x.DateWithAttribute);
			
			string expected =
				"<table class=\"grid\"><thead><tr><th>Date With Attribute</th></tr></thead><tbody><tr class=\"gridrow\"><td>01</td></tr></tbody></table>";

			RenderGrid().ShouldEqual(expected);
			
		}

		private string RenderGrid()
		{
			return RenderGrid(_people);
		}

		private string RenderGrid(IEnumerable<Person> dataSource)
		{
			var renderer = new HtmlTableGridRenderer<Person>(_engines);

			var viewContext = MockRepository.GenerateStub<ViewContext>();
			viewContext.Writer = _writer;
			viewContext.View = MockRepository.GenerateStub<IView>();
			viewContext.TempData = new TempDataDictionary();
			var response = MockRepository.GenerateStub<HttpResponseBase>();
			var context = MockRepository.GenerateStub<HttpContextBase>();
			var request = MockRepository.GenerateStub<HttpRequestBase>();

			viewContext.HttpContext = context;
			context.Stub(x =>x.Response).Return(response);
			context.Stub(x => x.Request).Return(request);
			response.Stub(x => x.Output).Return(_writer);
			request.Stub(x => x.ApplicationPath).Return("/");
			request.Stub(x => x.QueryString).Return(_querystring);
			response.Expect(x => x.ApplyAppPathModifier(Arg<string>.Is.Anything))
				.Do(new Func<string,string>(x => x))
				.Repeat.Any();

			renderer.Render(_model, dataSource, _writer, viewContext);
            
			return _writer.ToString();
		}

		private void SetupViewEngine(string viewName, string viewContents)
		{
			SetupViewEngine(viewName, (v, w) => w.Write(viewContents));
		}

		private void SetupViewEngine(string viewName, Action<ViewContext, TextWriter> action)
		{
			var view = MockRepository.GenerateMock<IView>();
			_viewEngine.Expect(x => x.FindPartialView(Arg<ControllerContext>.Is.Anything, Arg<string>.Is.Equal(viewName), Arg<bool>.Is.Anything)).Return(new ViewEngineResult(view, _viewEngine)).Repeat.Any();

			view.Expect(x => x.Render(null, null)).IgnoreArguments()
				.Do(action).Repeat.Any();
		}

		public static RenderingContext FakeRenderingContext() 
		{
			var engine = MockRepository.GenerateStub<IViewEngine>();
			engine.Stub(x => x.FindPartialView(null, null, true)).IgnoreArguments().Return(new ViewEngineResult(MockRepository.GenerateStub<IView>(), engine)).Repeat.Any();

			var context = new RenderingContext(
				new StringWriter(),
				new ViewContext() { View = MockRepository.GenerateStub<IView>(), TempData = new TempDataDictionary() },
				new ViewEngineCollection(new List<IViewEngine>() { engine }));

			return context;
		}
	}
}
