using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MvcContrib.TestHelper.MockFactories
{
    internal class MoqProxy<T> : IMockProxy<T>
    {
        private readonly Type _mockType;
        private readonly PropertyInfo _objectProperty;
        private readonly object _instance;

        public T Object 
        { 
            get
            {
                return (T)_objectProperty.GetValue(_instance, null);

            }
        }

        public MoqProxy(Type mockType)
        {
            _mockType = mockType;
            _instance = Activator.CreateInstance(_mockType);
            _objectProperty = mockType.GetProperty("Object", _mockType);
        }

        private MethodInfo GetSetupMethod<TResult>() 
        {
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