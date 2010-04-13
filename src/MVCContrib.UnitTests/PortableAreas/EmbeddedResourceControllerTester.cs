using System;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.PortableAreas;
using NUnit.Framework;

namespace MvcContrib.UnitTests.PortableAreas
{
    [TestFixture]
    public class EmbeddedResourceControllerTester
    {
        /// <summary>
        /// Ensure area registration only happens one time.
        /// </summary>
        [TestFixtureSetUp]
        public void Embedded_resource_controller_setup()
        {
            var areaRegistration = new StubPortableAreaRegistration();
            var registrationContext = new AreaRegistrationContext("FooArea", new RouteCollection());
            areaRegistration.RegisterArea(registrationContext);          
        }


        [Test]
        public void Embedded_resource_controller_should_return_embedded_image()
        {
            // arrange
            EmbeddedResourceController controller = InitializeEmbeddedResourceController();
            
            // act
            var result = controller.Index("images.arrow.gif", null) as FileStreamResult;

            // assert
            result.FileStream.ShouldNotBeNull();
            result.ContentType.ShouldEqual("image/gif");
        }

        [Test]
        public void Embedded_resource_controller_should_return_404_for_nonexistant_resource()
        {
            // arrange
            EmbeddedResourceController controller = InitializeEmbeddedResourceController();

            // act
            var result = controller.Index("foobar.gif", null);

            // assert
            result.ShouldBeNull();
            controller.Response.StatusCode.ShouldEqual(404);
        }

        [Test]
        public void Embedded_resource_controller_should_return_embedded_image_for_custom_path()
        {
            // arrange
            EmbeddedResourceController controller = InitializeEmbeddedResourceController();

            // act
            var result = controller.Index("arrow.gif", "images") as FileStreamResult;

            // assert
            result.FileStream.ShouldNotBeNull();
            result.ContentType.ShouldEqual("image/gif");
        }

        [Test]
        public void Embedded_resource_controller_should_return_404_for_nonexistant_custom_path()
        {
            // arrange
            EmbeddedResourceController controller = InitializeEmbeddedResourceController();

            // act
            var result = controller.Index("foobar.gif", "ximages");

            // assert
            result.ShouldBeNull();
            controller.Response.StatusCode.ShouldEqual(404);
        }


        private static EmbeddedResourceController InitializeEmbeddedResourceController()
        {
            var controller = new EmbeddedResourceController();
            var routeData = new RouteData();
            routeData.DataTokens.Add("area", "FooArea");
            controller.ControllerContext = new ControllerContext(MvcMockHelpers.DynamicHttpContextBase(), routeData, controller);
            return controller;
        }
    }

    class StubPortableAreaRegistration : PortableAreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context, IApplicationBus bus)
        {
            context.MapRoute("ResourceRoute", "fooarea/resource/{resourceName}", 
                new { controller = "Resource", action = "Index" });

            
            this.RegisterAreaEmbeddedResources();
        }

        public override string AreaName
        {
            get
            {
                return "FooArea";
            }
        }
    }
}
