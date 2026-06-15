using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DouduckLib
{
    public class DIContainer
    {
        readonly Dictionary<Type, Type> _types = new Dictionary<Type, Type>();
        readonly Dictionary<Type, object> _typeInstances = new Dictionary<Type, object>();

        public void Register<TContract, TImplementation>()
        {
            _types[typeof(TContract)] = typeof(TImplementation);
        }

        public void Register<TContract, TImplementation>(TImplementation instance)
        {
            _typeInstances[typeof(TContract)] = instance;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type contractType)
        {
            if (_typeInstances.ContainsKey(contractType))
            {
                return _typeInstances[contractType];
            }
            else
            {
                Type implementation = _types[contractType];
                ConstructorInfo constructor = implementation.GetConstructors()[0];
                ParameterInfo[] constructorParameters = constructor.GetParameters();
                if (constructorParameters.Length == 0)
                {
                    return Activator.CreateInstance(implementation);
                }

                List<object> parameters = new List<object>(constructorParameters.Length);
                foreach (ParameterInfo parameterInfo in constructorParameters)
                {
                    parameters.Add(Resolve(parameterInfo.ParameterType));
                }

                return constructor.Invoke(parameters.ToArray());
            }
        }
    }
}
