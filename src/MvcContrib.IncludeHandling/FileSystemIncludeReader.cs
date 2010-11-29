using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace MvcContrib.IncludeHandling
{
	public class FileSystemIncludeReader : IIncludeReader
	{
		private readonly string _applicationRoot;
		private readonly string _fileSystemRoot;

		public FileSystemIncludeReader(string applicationRoot, string fileSystemRoot)
		{
			_applicationRoot = applicationRoot;
			_fileSystemRoot = fileSystemRoot;
		}

		public FileSystemIncludeReader(IHttpContextProvider http)
		{
			_applicationRoot = http.Request.ApplicationPath;
			_fileSystemRoot = http.Request.MapPath("~/");
		}

		#region IIncludeReader Members

		public string ToAbsolute(string source)
		{
			if (source.StartsWith("~/"))
			{
				return VirtualPathUtility.AppendTrailingSlash(_applicationRoot) + source.Substring(2);
			}
			return source;
		}

		public Include Read(string source, IncludeType type)
		{
			if (String.IsNullOrEmpty(source))
			{
				throw new ArgumentException("source must have a value", source);
			}

			var abs = ToFileSystem(source);
			var file = new FileInfo(abs);
			if (!file.Exists)
			{
				throw new InvalidOperationException(string.Format("{0} does not exist", source));
			}

			var content = File.ReadAllText(abs);
			var lastModifiedAt = File.GetLastWriteTimeUtc(abs);

            if (type == IncludeType.Css)
            {
                content = ReplaceCssUrls(source, content);
            }

			return new Include(type, source, content, lastModifiedAt);
		}

		#endregion

		protected string ToFileSystem(string source)
		{
			if (source.StartsWith("~/"))
			{
				var fsSource = source.Substring(2).Replace('/', '\\');
				return _fileSystemRoot + fsSource;
			}
			// assume absolute path already
			return source;
		}

        private const string UrlMatch = "url\\(\\s*[\\'\"]?(?![a-z]+:|/+)([^\\'\")]+)[\\'\"]?\\s*\\)";
        private const string UrlContentMatch = @"(^|/)(?!\.\./)([^\/]+)/\.\./";

        readonly Regex _regexUrl = new Regex(UrlMatch, RegexOptions.Multiline | RegexOptions.Compiled);
        readonly Regex _regexContent = new Regex(UrlContentMatch, RegexOptions.Compiled);

        internal string ReplaceCssUrls(string source, string content)
        {
            var relPath = VirtualPathUtility.GetDirectory(source);
            var absPath = VirtualPathUtility.ToAbsolute(relPath, this._applicationRoot);

            var newContent = _regexUrl.Replace(content, new MatchEvaluator(match =>
            {
                var g = match.Groups[1].Value;

                var path = absPath + g;
                var last = string.Empty;

                while (path != last)
                {
                    last = path;
                    path = _regexContent.Replace(path, "$1");
                }

                return "url('" + path + "')";
            }));

            return newContent;
        }
	}
}