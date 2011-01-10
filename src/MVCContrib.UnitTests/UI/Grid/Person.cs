using System;
using System.ComponentModel;

namespace MvcContrib.UnitTests.UI.Grid
{
	public class Person
	{
		public string Name { get; set; }
		[DisplayName("Name2")]
		public string NameWithAttribute { get; set; }

		public DateTime DateOfBirth { get; set; }

		public int Id { get; set; }
	}
}