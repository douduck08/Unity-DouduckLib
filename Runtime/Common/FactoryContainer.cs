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
        readonly Dictionary<Type, IFactory> _factories = new Dictionary<Type, IFactory>();

        public void Register<TContract>(IFactory factory)
        {
            _factories[typeof(TContract)] = factory;
        }

        public TValue Create<TValue>()
        {
            if (_factories.TryGetValue(typeof(TValue), out var factory))
            {
                return ((IFactory<TValue>)factory).Create();
            }
            throw new KeyNotFoundException($"Factory for {typeof(TValue)} not registered.");
        }

        public TValue Create<TParam1, TValue>(TParam1 param)
        {
            if (_factories.TryGetValue(typeof(TValue), out var factory))
            {
                return ((IFactory<TParam1, TValue>)factory).Create(param);
            }
            throw new KeyNotFoundException($"Factory for {typeof(TValue)} not registered.");
        }

        public TValue Create<TParam1, TParam2, TValue>(TParam1 param1, TParam2 param2)
        {
            if (_factories.TryGetValue(typeof(TValue), out var factory))
            {
                return ((IFactory<TParam1, TParam2, TValue>)factory).Create(param1, param2);
            }
            throw new KeyNotFoundException($"Factory for {typeof(TValue)} not registered.");
        }

        public TValue Create<TParam1, TParam2, TParam3, TValue>(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            if (_factories.TryGetValue(typeof(TValue), out var factory))
            {
                return ((IFactory<TParam1, TParam2, TParam3, TValue>)factory).Create(param1, param2, param3);
            }
            throw new KeyNotFoundException($"Factory for {typeof(TValue)} not registered.");
        }

        public TValue Create<TParam1, TParam2, TParam3, TParam4, TValue>(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            if (_factories.TryGetValue(typeof(TValue), out var factory))
            {
                return ((IFactory<TParam1, TParam2, TParam3, TParam4, TValue>)factory).Create(param1, param2, param3, param4);
            }
            throw new KeyNotFoundException($"Factory for {typeof(TValue)} not registered.");
        }

        [Obsolete("Use strongly-typed Create methods instead to avoid reflection overhead.")]
        public T Create<T>(IEnumerable<object> extraArgs)
        {
            return (T)Create(typeof(T), extraArgs);
        }

        [Obsolete("Use strongly-typed Create methods instead to avoid reflection overhead.")]
        public object Create(Type contractType, IEnumerable<object> extraArgs)
        {
            IFactory factory = _factories[contractType];
            Type factoryType = factory.GetType();
            MethodInfo methodInfo = factoryType.GetMethod("Create");
            return methodInfo.Invoke(factory, extraArgs.ToArray());
        }
    }
}
