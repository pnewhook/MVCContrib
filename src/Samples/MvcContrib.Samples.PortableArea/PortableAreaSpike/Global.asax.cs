﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.PortableAreas;
using MvcContrib.UI.InputBuilder;

namespace PortableAreaSpike
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new {controller = "Home", action = "Index", id = ""} // Parameter defaults
				);
		}

		protected void Application_Start()
		{
			//wire up a sample observer
			PortableArea.Bus.Add(typeof (LogAllMessagesObserver));
			//wire up a handler that validates a login for the Login Area.
			PortableArea.Bus.Add(typeof (LoginHandler));

			//Default MVC2 registration syntax
			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);
			InputBuilder.BootStrap();
		}
	}
}