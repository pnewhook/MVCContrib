using System;
using System.Linq;
using MvcContrib.Attributes;
using MvcContrib.Binders;
using MvcContrib.UnitTests.UI.DerivedTypeModelBinder;
using NUnit.Framework;

namespace MvcContrib.UnitTests.Binders
{
    [TestFixture]
    [DerivedTypeBinderAware(typeof(int))]
    public class DerivedTypeModelBinderCacheTests
    {
		//[Test]
		//public void validate_declarative_registration_of_derived_types()
		//{
		//    DerivedTypeModelBinderCache.RegisterDerivedTypes(typeof(DerivedTypeModelBinderCacheTests),
		//                                                     new[] {typeof(string)});

		//    Assert.That((from p in DerivedTypeModelBinderCache.GetDerivedTypes(typeof(DerivedTypeModelBinderCacheTests))
		//                 where p.Name == typeof(string).Name
		//                 select p).FirstOrDefault(), Is.Not.Null);

		//    DerivedTypeModelBinderCache.Reset();

		//    // next, let's validate that the cache was cleared by reset
		//    Assert.That((from p in DerivedTypeModelBinderCache.GetDerivedTypes(typeof(DerivedTypeModelBinderCacheTests))
		//                 where p.Name == typeof(string).Name
		//                 select p).FirstOrDefault(), Is.Null);

		//}

		//[Test]
		//public void validate_attribute_scan_on_getDerivedTypes_call()
		//{
		//    DerivedTypeModelBinderCache.Reset();

		//    Assert.That((from p in DerivedTypeModelBinderCache.GetDerivedTypes(typeof(DerivedTypeModelBinderCacheTests))
		//                     where p.Name == typeof(int).Name
		//                     select p).FirstOrDefault(), Is.Not.Null);

		//    DerivedTypeModelBinderCache.Reset();
		//}

		[Test]
		public void verify_default_type_stamp_field_name()
		{
			Assert.That(DerivedTypeModelBinderCache.TypeStampFieldName, Is.EqualTo("_xTypeStampx_"));
		}

		[Test]
		public void GetTypeName_verify_encryption()
		{
			DerivedTypeModelBinderCache.Reset();
			DerivedTypeModelBinderCache.RegisterDerivedTypes(typeof(ITestClass), new[] {typeof(TestClass)});

			var typeName = DerivedTypeModelBinderCache.GetTypeName(typeof(TestClass));

			Assert.That(typeName, Is.StringStarting("VFxCac+"));
		}

		[Test]
		public void GetTypeName_verify_error_on_unrecognized_type()
		{
			DerivedTypeModelBinderCache.Reset();
			DerivedTypeModelBinderCache.RegisterDerivedTypes(typeof(ITestClass), new[] { typeof(TestClass) });

			var exception = Assert.Throws<InvalidOperationException>(() => DerivedTypeModelBinderCache.GetTypeName(typeof(DerivedTypeModelBinderCacheTests)));

			Assert.That(exception.Message, Is.StringContaining("DerivedTypeModelBinderCacheTests"));
			Assert.That(exception.Message, Is.StringContaining("with the DerivedTypeModelBinder"));
		}
    }
}
