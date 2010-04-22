using System;
using System.Web.Mvc;
using MvcContrib.UI.InputBuilder;
using MvcContrib.UI.InputBuilder.ViewEngine;

namespace MvcContrib.PortableAreas
{
	public abstract class PortableAreaRegistration:AreaRegistration
	{
		public static Action RegisterEmbeddedViewEngine = () => { InputBuilder.BootStrap(); };
		public virtual void RegisterArea(AreaRegistrationContext context,IApplicationBus bus)
		{

			bus.Send(new PortableAreaStartupMessage(AreaName));

			RegisterDefaultRoutes(context);

			RegisterAreaEmbeddedResources();
		}

		public  void CreateStaticResourceRoute(AreaRegistrationContext context, string SubfolderName)
		{
			context.MapRoute(
			AreaName + "-" + SubfolderName,
			AreaName + "/"+SubfolderName+"/{resourceName}",
			new { controller = "EmbeddedResource", action = "Index", resourcePath = "Content."+SubfolderName },
			null,
			new string[] { "MvcContrib.PortableAreas" }
			);
		}

		public void RegisterDefaultRoutes(AreaRegistrationContext context) {
			CreateStaticResourceRoute(context, "Images");
			CreateStaticResourceRoute(context, "Styles");
			CreateStaticResourceRoute(context, "Scripts");
			context.MapRoute(AreaName + "-Default",
			                 AreaName + "/{controller}/{action}",
			                 new { controller = "default", action = "index" });
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			RegisterArea(context,Bus.Instance);
						
			RegisterEmbeddedViewEngine();
		}

        public void RegisterAreaEmbeddedResources()
        {
            var areaType = this.GetType();
            var resourceStore = new AssemblyResourceStore(areaType, "/areas/" + AreaName.ToLower(), areaType.Namespace);
            AssemblyResourceManager.RegisterAreaResources(resourceStore);
        }
	}
}