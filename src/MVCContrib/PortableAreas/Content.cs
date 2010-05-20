using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcContrib.PortableAreas
{
	public class Content
	{
		private static Dictionary<Type, PortableAreaMap> _maps;

		private static object _lockObject = new object();

		public static Dictionary<Type, PortableAreaMap> Maps
		{
			get
			{
				if (_maps == null)
				{
					lock (_lockObject)
					{
						if (_maps == null)
						{
							_maps = new Dictionary<Type, PortableAreaMap>();
						}
					}
				}
				return _maps;
			}
			set
			{
				_maps = value;
			}
		}

		public static T Map<T>() where T : PortableAreaMap
		{
			PortableAreaMap map = null;

			if (!Maps.TryGetValue(typeof(T), out map))
			{
				map = Activator.CreateInstance<T>();
				Maps.Add(typeof(T), map);
			}

			return map as T;
		}
	}
}
