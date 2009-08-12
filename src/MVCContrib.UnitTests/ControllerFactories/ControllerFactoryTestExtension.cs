using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MvcContrib.UnitTests.ControllerFactories
{
	public static class ControllerFactoryTestExtension
	{
		private static readonly PropertyInfo _typeCacheProperty;
		private static readonly FieldInfo _cacheField;

		static ControllerFactoryTestExtension()
		{
			_typeCacheProperty = typeof(DefaultControllerFactory).GetProperty("ControllerTypeCache", BindingFlags.Instance | BindingFlags.NonPublic);
			_cacheField = _typeCacheProperty.PropertyType.GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		/// <summary>
		/// Replaces the cache field of a the DefaultControllerFactory's ControllerTypeCache.
		/// This ensures that only the specified controller types will be searched when instantiating a controller.
		/// As the ControllerTypeCache is internal, this uses some reflection hackery.
		/// </summary>
		public static void InitializeWithControllerTypes(this IControllerFactory factory, params Type[] controllerTypes)
		{
			var cache = controllerTypes
				.GroupBy(t => t.Name.Substring(0, t.Name.Length - "Controller".Length), StringComparer.OrdinalIgnoreCase)
				.ToDictionary(g => g.Key, g => g.ToLookup(t => t.Namespace ?? string.Empty, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);

			var buildManager = _typeCacheProperty.GetValue(factory, null);
			_cacheField.SetValue(buildManager, cache);
		}
	}
}