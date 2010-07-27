using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core;
using NUnit.Framework;
using Castle.Windsor;
using MvcContrib.Castle;


namespace MvcContrib.UnitTests.IoC
{
	public class WindsorExtensionTester
	{

		public class WindsorExtensionTestBase
		{
			protected IWindsorContainer _container;

			[SetUp]
			public virtual void Setup()
			{
				_container = new WindsorContainer();
			}			
		}

		[TestFixture]
		public class When_RegisterController_generic_is_invoked : WindsorExtensionTestBase
		{
			public override void Setup()
			{
				base.Setup();
				_container.RegisterController<WindsorSimpleController>();
			}

			[Test]
			public void Then_the_type_should_be_registered()
			{
				Assert.That(_container.Kernel.HasComponent("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller"));
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.Implementation, Is.EqualTo(typeof(WindsorSimpleController)));
			}

			[Test]
			public void Then_the_lifestyle_should_be_transient()
			{
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.LifestyleType, Is.EqualTo(LifestyleType.Transient));
			}
		}

		[TestFixture]
		public class When_RegisterControllers_is_invoked_with_types : WindsorExtensionTestBase
		{
			public override void Setup()
			{
				base.Setup();
				_container.RegisterControllers(typeof(WindsorSimpleController));
			}

			[Test]
			public void Then_the_type_should_be_registered()
			{
				Assert.That(_container.Kernel.HasComponent("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller"));
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.Implementation, Is.EqualTo(typeof(WindsorSimpleController)));
			}

			[Test]
			public void Then_the_lifestyle_should_be_transient()
			{
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.LifestyleType, Is.EqualTo(LifestyleType.Transient));
			}
		}

		[TestFixture]
		public class When_RegisterControllers_is_invoked_with_assemblies : WindsorExtensionTestBase
		{
			public override void Setup()
			{
				base.Setup();
				_container.RegisterControllers(typeof(WindsorExtensionTester).Assembly);
			}

			[Test]
			public void Then_all_controllers_in_the_assembly_should_be_registered()
			{
				Assert.That(_container.Kernel.HasComponent("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller"));
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.Implementation, Is.EqualTo(typeof(WindsorSimpleController)));
				//etc
			}

			[Test]
			public void Then_lifestyles_should_be_set_to_transient()
			{
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.castle.windsorcontrollerfactorytester+windsorsimplecontroller").ComponentModel.LifestyleType, Is.EqualTo(LifestyleType.Transient));
				Assert.That(_container.Kernel.GetHandler("mvccontrib.unittests.controllerfactories.ioccontrollerfactorytester.ioctestcontroller").ComponentModel.LifestyleType, Is.EqualTo(LifestyleType.Transient));
			}
		}

		public class WindsorDisposableController : IDisposable, IController {
			public bool IsDisposed;

			public WindsorDisposableController() {
				IsDisposed = false;
			}

			public void Dispose() {
				IsDisposed = true;
			}

			public void Execute(RequestContext controllerContext) {
			}
		}

		public class MockApplication : HttpApplication, IContainerAccessor {
			private readonly IWindsorContainer _container;

			public MockApplication(IWindsorContainer container) {
				_container = container;
			}

			public IWindsorContainer Container {
				get { return _container; }
			}
		}

		public class WindsorSimpleController : IController {
			public void Execute(RequestContext controllerContext) {
				throw new NotImplementedException();
			}
		}

		public class WindsorDependencyController : IController {
			public IDependency _dependency;

			public WindsorDependencyController(IDependency dependency) {
				_dependency = dependency;
			}

			public void Execute(RequestContext controllerContext) {
				throw new NotImplementedException();
			}
		}

		public interface IDependency {
		}

		public class StubDependency : IDependency {
		}
	}
}