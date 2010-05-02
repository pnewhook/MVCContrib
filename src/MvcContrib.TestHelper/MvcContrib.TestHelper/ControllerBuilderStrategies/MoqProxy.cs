using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Linq;

namespace MvcContrib.TestHelper.ControllerBuilderStrategies
{
	//Derived from StructureMap's MoqFactory.
	public class MoqProxy
	{
		private Type _mockOpenType;

		public MoqProxy()
		{
			Assembly Moq = Assembly.Load("Moq");
			_mockOpenType = Moq.GetType("Moq.Mock`1");

			if (_mockOpenType == null)
			{
				throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + Moq.Location);
			}
		}


		public MockProxy<T> DynamicMock<T>()
		{
			return new MockProxy<T>(_mockOpenType.MakeGenericType(typeof(T)));
		}
	}

	public class MockProxy<T>
	{
		private readonly Type _mockType;
		private PropertyInfo _objectProperty;
		private object _instance;

		public T Object 
		{ 
			get
			{
				return (T)_objectProperty.GetValue(_instance, null);

			}
		}

		public MockProxy(Type mockType)
		{
			_mockType = mockType;
			_instance = Activator.CreateInstance(_mockType);
			_objectProperty = mockType.GetProperty("Object", _mockType);
		}

		private MethodInfo GetSetupMethod<TResult>() {
			var openSetupMethod = _mockType.GetMethods().First(m => m.IsGenericMethod && m.Name == "Setup");
			return openSetupMethod.MakeGenericMethod(typeof(TResult));
		}

		public void ReturnFor<TResult>(Expression<Func<T, TResult>> expression, TResult result)
		{
			var setupMethod = GetSetupMethod<TResult>();
			var setup = setupMethod.Invoke(_instance, new object[] { expression });
			var returnsMethod = setup.GetType().GetMethod("Returns", new [] {typeof(TResult)});
			returnsMethod.Invoke(setup, new object[] { result});
		}

		public void CallbackFor<TResult>(Expression<Func<T, TResult>> expression, Func<TResult> callback)
		{
			var setupMethod = GetSetupMethod<TResult>();
			var setup = setupMethod.Invoke(_instance, new object[] { expression });
			var returnsMethod = setup.GetType().GetMethod("Returns", new[] { typeof(Func<TResult>) });
			returnsMethod.Invoke(setup, new object[] {callback});
		}

		public void SetupProperty<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var openSetupMethod = _mockType.GetMethods().First(m => m.Name == "SetupProperty" && m.GetParameters().Length == 1);
			var setupMethod = openSetupMethod.MakeGenericMethod(typeof(TProperty));
			setupMethod.Invoke(_instance, new object[] {expression});
		}
	}
}