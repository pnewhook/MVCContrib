using System;
using System.Reflection;

namespace MvcContrib.TestHelper.MockFactories
{
    /// <summary>
    /// Creates mock objects using Moq.
    /// </summary>
    internal class MoqFactory : IMockFactory
    {
		private static readonly Type _mockOpenType;
    	private static readonly Exception _loadException;

		/// <summary>
		/// Grabs references to static types.
		/// </summary>
    	static MoqFactory()
		{
			try
			{
				Assembly Moq = Assembly.Load("Moq");
				_mockOpenType = Moq.GetType("Moq.Mock`1");

				if (_mockOpenType == null)
				{
					throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + Moq.Location);
				}				
			}
			catch(Exception ex)
			{
				_loadException = ex;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public MoqFactory()
		{
			if (_mockOpenType == null)
			{
				throw new InvalidOperationException("Unable to create a proxy for Moq.", _loadException);
			}
		}

		public IMockProxy<T> DynamicMock<T>() where T : class
		{
			return new MoqProxy<T>(_mockOpenType.MakeGenericType(typeof(T)));
		}
	}
}