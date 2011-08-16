using MvcContrib.FluentHtml.Elements;
using MvcContrib.FluentHtml.Html;
using MvcContrib.UnitTests.FluentHtml.Helpers;
using NUnit.Framework;

namespace MvcContrib.UnitTests.FluentHtml
{
	[TestFixture]
	public class EmailBoxTests
	{
		[Test]
		public void basic_emailbox_render()
		{
			var html = new EmailBox("foo").ToString();
			html.ShouldHaveHtmlNode("foo")
				.ShouldBeNamed(HtmlTag.Input)
				.ShouldHaveAttribute(HtmlAttribute.Type).WithValue(HtmlInputType.Email);
		}

		[Test]
		public void emailbox_with_multiple_renders_multiple()
		{
			var html = new EmailBox("foo").Multiple(true).ToString();
			html.ShouldHaveHtmlNode("foo")
				.ShouldHaveAttribute(HtmlAttribute.Multiple).WithValue(HtmlAttribute.Multiple);
		}

		[Test]
		public void emailbox_without_multiple_does_not_render_multiple()
		{
			var html = new EmailBox("foo").Multiple(true).Multiple(false).ToString();
			html.ShouldHaveHtmlNode("foo")
				.ShouldNotHaveAttribute(HtmlAttribute.Multiple);
		}

		[Test]
		public void emailbox_list_renders_list()
		{
			var html = new EmailBox("foo").List("list1").ToString();
			html.ShouldHaveHtmlNode("foo")
				.ShouldHaveAttribute(HtmlAttribute.List).WithValue("list1");
		}
	}
}