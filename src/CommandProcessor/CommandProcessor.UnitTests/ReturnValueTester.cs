using System;
using NBehave.Spec.NUnit;
using NUnit.Framework;

namespace MvcContrib.CommandProcessor.UnitTests
{
	[TestFixture]
	public class ReturnValueTester
	{
		[Test]
		public void Can_set_value()
		{
			var value = new ReturnValue();
			value.SetValue(new DateTime());

			value.Type.ShouldEqual(typeof (DateTime));
			value.Value.ShouldEqual(value.Value);
		}
	}
}