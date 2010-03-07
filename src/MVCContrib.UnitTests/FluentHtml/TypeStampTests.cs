using MvcContrib.FluentHtml.Elements;
using MvcContrib.FluentHtml.Html;
using MvcContrib.UnitTests.FluentHtml.Helpers;
using NUnit.Framework;

namespace MvcContrib.UnitTests.FluentHtml
{
    [TestFixture]
    public class TypeStampTests
    {
        [Test]
        public void basic_typestamp_renders_with_correct_tag_and_type()
        {
            new TypeStamp("x", typeof(TypeStampTests)).ToString()
                .ShouldHaveHtmlNode("x__xTypeStampx_")
                .ShouldBeNamed(HtmlTag.Input)
                .ShouldHaveAttribute(HtmlAttribute.Type).WithValue(HtmlInputType.Hidden);
        }

        [Test]
        public void basic_typestamp_renders_with_correct_value()
        {
            var value = new TypeStamp("x", typeof(TypeStampTests)).ToString()
                .ShouldHaveHtmlNode("x__xTypeStampx_")
                .ShouldHaveAttribute("Value").Value;
            Assert.That(value, Is.EqualTo(typeof(TypeStampTests).FullName));
        }

        [Test]
        public void typestamp_renders_without_binding_context()
        {
            new TypeStamp(string.Empty, typeof(TypeStampTests)).ToString()
                .ShouldHaveHtmlNode("_xTypeStampx_")
                .ShouldBeNamed(HtmlTag.Input)
                .ShouldHaveAttribute(HtmlAttribute.Type).WithValue(HtmlInputType.Hidden);
        }
    }
}
