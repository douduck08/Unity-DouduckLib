using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DouduckLib
{
    public interface IFactory { }

    public interface IFactory<out TValue> : IFactory
    {
        TValue Create();
    }

    public interface IFactory<in TParam1, out TValue> : IFactory
    {
        TValue Create(TParam1 param);
    }

    public interface IFactory<in TParam1, in TParam2, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IFactory<in TParam1, in TParam2, in TParam3, in TParam4, out TValue> : IFactory
    {
        TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    public class FactoryContainer
    {
        readonly IDictionary<Type, IFactory> factories = new Dictionary<Type, IFactory>();

        public void Register<TContract>(IFactory factory)
        {
            factories[typeof(TContract)] = factory;
        }

        public T Create<T>()
        {
            return (T)Create(typeof(T), new object[0]);
        }

        public T Create<T>(IEnumerable<object> extraArgs)
        {
            return (T)Create(typeof(T), extraArgs);
        }

        public object Create(Type contractType, IEnumerable<object> extraArgs)
        {
            IFactory factory = factories[contractType];
            Type factoryType = factory.GetType();
            MethodInfo methodInfo = factoryType.GetMethod("Create");
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            return methodInfo.Invoke(factory, extraArgs.ToArray());
        }
    }
}
