using System;
using System.Reflection;
using Rhino.Mocks;
using System.Linq;

namespace MvcContrib.TestHelper.MockFactories
{
	internal class RhinoMocksFactory : IMockFactory
	{
		internal static Assembly RhinoMocks;

		private static object _mocks;
		private static Exception _loadException;
		private static MethodInfo _dynamicMockOpen;

		static RhinoMocksFactory()
		{
			try
			{
				RhinoMocks = Assembly.Load("Rhino.Mocks");
				var repositoryType = RhinoMocks.GetType("Rhino.Mocks.MockRepository");
				_dynamicMockOpen = repositoryType.GetMethods().First(m => m.Name == "DynamicMock" && m.IsGenericMethod);
				_mocks = Activator.CreateInstance(repositoryType);
			}
			catch(Exception ex)
			{
				_loadException = ex;
			}
		}

		public RhinoMocksFactory()
		{			
			if (_mocks == null)
			{
				throw new InvalidOperationException("Unable to create a proxy for RhinoMocks.", _loadException);
			}
		}

		public IMockProxy<T> DynamicMock<T>() where T : class
		{
			var dynamicMock = _dynamicMockOpen.MakeGenericMethod(typeof(T));
			return new RhinoMocksProxy<T>((T)dynamicMock.Invoke(_mocks, new object[] { new object[0]}), _mocks);
		}
	}
}
