using System;
using System.Linq;
using MvcContrib.PortableAreas;
using System.Collections.Generic;

namespace MvcContrib
{
	public class Bus
	{
		private static IApplicationBus _instance;
		private static object _busLock = new object();

		public static IApplicationBus Instance { 
			get
			{
				InitializeTheDefaultBus();
				return _instance;
			}
			set
			{
				_instance=value;					
			}
		}

		private static void InitializeTheDefaultBus()
		{
			if(_instance==null)
			{
				lock(_busLock)
				{
					if(_instance==null)
					{
						_instance = new ApplicationBus(new MessageHandlerFactory());
						AddAllMessageHandlers();
					}
				}
			}
		}
		
		public static void Send(IEventMessage eventMessage)
		{
			Instance.Send(eventMessage);
		}

		public static void AddMessageHandler(Type type)
		{
			Instance.Add(type);
		}

		public static void AddAllMessageHandlers()
		{
			var handlers = FindAllMessageHandlers();

			foreach (var handler in handlers)
				Instance.Add(handler);
		}

		private static IEnumerable<Type> FindAllMessageHandlers()
		{
			var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());

			var types = allTypes.Where(type => IsValidType(type));

			return types;
		}

		public static bool IsValidType(Type type)
		{
			if (type.IsInterface || type.IsAbstract || type.IsNestedPrivate)
				return false;

			bool isIMessageHandler = type.GetInterface(typeof(IMessageHandler).Name) != null;

			return isIMessageHandler;
		}
	}
}