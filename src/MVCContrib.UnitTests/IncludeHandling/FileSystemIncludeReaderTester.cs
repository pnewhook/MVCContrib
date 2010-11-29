using System;
using MvcContrib.IncludeHandling;
using NUnit.Framework;

namespace MvcContrib.UnitTests.IncludeHandling
{
	[TestFixture]
	public class FileSystemIncludeReaderTester
	{
		private IIncludeReader _reader;

		[SetUp]
		public void TestSetup()
		{
			_reader = new FileSystemIncludeReader("/", "c:\\");
		}

		[Test]
		public void ToAbsolute_ConvertsPathWhenSourceIsRelative()
		{
			var result = _reader.ToAbsolute("~/foo.css");
			Assert.AreEqual("/foo.css", result);
		}

		[Datapoint]
		public string nothing = null;

		[Datapoint] 
		public string blank = "";

		[Theory]
		[ExpectedException(typeof(ArgumentException))]
		public void WhenSourceMissing_WillThrow(string source)
		{
			_reader.Read(source, IncludeType.Css);
		}

        [Test]
        public void ReplaceCss_WithAbsolutePath_DoesNothing()
        {
            var css = "body { background-image: url('/images/background.png'); }";

            var result = ((FileSystemIncludeReader)_reader).ReplaceCssUrls("~/Content/Css/file.css", css);

            Assert.That(result, Is.EqualTo(css));
        }

        [Test]
        public void ReplaceCss_WithRelaviteDottedPathToRoot_ReplacesWithBase()
        {
            var css = "body { background-image: url('../../images/background.png'); }";

            var result = ((FileSystemIncludeReader)_reader).ReplaceCssUrls("~/Content/Css/file.css", css);

            Assert.That(result, Is.EqualTo("body { background-image: url('/images/background.png'); }"));
        }

        [Test]
        public void ReplaceCss_WithRelaviteDottedPath_ReplacesWithBase()
        {
            var css = "body { background-image: url('../images/background.png'); }";

            var result = ((FileSystemIncludeReader)_reader).ReplaceCssUrls("~/Content/Css/file.css", css);

            Assert.That(result, Is.EqualTo("body { background-image: url('/Content/images/background.png'); }"));
        }
	}

	public class FileSystemIncludeReaderIntegrationTester
	{
		private IIncludeReader _reader;

		[SetUp]
		public void TestSetup()
		{
			_reader = new FileSystemIncludeReader("/", Environment.CurrentDirectory);
		}

		[Test]
		public void WhenFileExists_WillReadIt()
		{
			Include include = _reader.Read("IncludeHandling\\exists.txt", IncludeType.Js);
			Assert.AreEqual("hello world, i exist!", include.Content);
		}

		[Test]
		public void WhenFileNotFound_WillThrow()
		{
			Assert.Throws<InvalidOperationException>(() => _reader.Read("c:\\doesNotExist.txt", IncludeType.Css));
		}
	}
}