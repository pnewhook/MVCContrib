using System;
using System.Web.Mvc;
using MvcContrib.UI.InputBuilder.ViewEngine;

namespace MvcContrib.PortableAreas
{
	public abstract class PortableAreaRegistration:AreaRegistration
	{
		public abstract void RegisterArea(AreaRegistrationContext context,IApplicationBus bus);
		
		public override void RegisterArea(AreaRegistrationContext context)
		{
			RegisterArea(context,Bus.Instance);
		}

        public void RegisterAreaEmbeddedResources()
        {
            var areaType = this.GetType();
            var resourceStore = new AssemblyResourceStore(areaType, "/areas/" + AreaName.ToLower(), areaType.Namespace);
            AssemblyResourceManager.RegisterAreaResources(resourceStore);
        }
	}
}