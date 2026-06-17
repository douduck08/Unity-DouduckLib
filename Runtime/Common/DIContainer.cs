using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DouduckLib
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectAttribute : Attribute { }

    public class DIContainer
    {
        readonly Dictionary<Type, Type> _types = new Dictionary<Type, Type>();
        readonly Dictionary<Type, object> _typeInstances = new Dictionary<Type, object>();
        readonly Dictionary<Type, (ConstructorInfo constructor, ParameterInfo[] parameters)> _cachedConstructors = new Dictionary<Type, (ConstructorInfo, ParameterInfo[])>();

        readonly HashSet<Type> _resolvingTypes = new HashSet<Type>();
        int _resolveDepth = 0;

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
            if (_resolveDepth == 0)
            {
                _resolvingTypes.Clear();
            }

            if (_resolvingTypes.Contains(contractType))
            {
                throw new InvalidOperationException($"Circular dependency detected for type: {contractType}");
            }

            _resolvingTypes.Add(contractType);
            _resolveDepth++;

            try
            {
                if (_typeInstances.ContainsKey(contractType))
                {
                    return _typeInstances[contractType];
                }

                if (!_types.ContainsKey(contractType))
                {
                    throw new KeyNotFoundException($"Type {contractType} is not registered in the container.");
                }

                Type implementation = _types[contractType];
                var (constructor, constructorParameters) = GetConstructorInfo(implementation);

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
            finally
            {
                _resolveDepth--;
                _resolvingTypes.Remove(contractType);
            }
        }

        private (ConstructorInfo constructor, ParameterInfo[] parameters) GetConstructorInfo(Type implementation)
        {
            if (_cachedConstructors.TryGetValue(implementation, out var cached))
            {
                return cached;
            }

            var constructors = implementation.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"No public constructors found for type: {implementation}");
            }

            ConstructorInfo selectedConstructor = null;
            foreach (var ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                {
                    selectedConstructor = ctor;
                    break;
                }
            }

            selectedConstructor ??= constructors[0];

            var parameters = selectedConstructor.GetParameters();
            var result = (selectedConstructor, parameters);
            _cachedConstructors.Add(implementation, result);
            return result;
        }
    }
}
