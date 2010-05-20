using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MvcContrib.PortableAreas
{
	public class PortableAreaMap
	{
		public const string MasterPageTemplate = @"MasterPageFile=""{0}""";
		public const string ContentPlaceHolderTemplate = @"ContentPlaceHolderID=""{0}""";
		public const string ContentPlaceHolderPattern = @"<asp:Content .*ContentPlaceHolderID=""{0}.*>";

		public string DefaultMasterPageLocation { get; set; }
		public string DefaultTitleID { get; set; }
		public string DefaultBodyID { get; set; }

		public string MasterPageLocation { get; set; }

		protected Dictionary<string, string> _mappings = new Dictionary<string, string>();

		public Stream Transform(Stream stream)
		{
			StreamReader reader = new StreamReader(stream);
			string result = TransformMarkup(reader.ReadToEnd());

			Stream newStream = new MemoryStream(result.Length);
			StreamWriter writer = new StreamWriter(newStream);
			writer.Write(result, 0, result.Length);
			writer.Flush();
			newStream.Position = 0;
			
			return newStream;
		}

		protected string TransformMarkup(string input)
		{
			string newLocation = string.Format(MasterPageTemplate, MasterPageLocation);
			string oldLocation = string.Format(MasterPageTemplate, DefaultMasterPageLocation);

			string result = string.Empty;
			
			if (string.IsNullOrEmpty(MasterPageLocation))
				result = input;
			else
				result = input.Replace(oldLocation, newLocation);

			foreach (var pair in _mappings)
			{
				string pattern = string.Format(ContentPlaceHolderPattern, pair.Key);
				
				result = Regex.Replace(result, pattern, m =>
				{
					return m.Value.Replace(pair.Key, pair.Value);
				});
			}

			return result;
		}

		public void Add(string defaultID, string newID)
		{
			if (_mappings.ContainsKey(defaultID))
				_mappings[defaultID] = newID;
			else
				_mappings.Add(defaultID, newID);
		}
	}
}
