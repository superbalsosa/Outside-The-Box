using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace DependencyInjection
{
    public class InterfaceDependencyInjector : Singleton<InterfaceDependencyInjector>
    {
        private Dictionary<(Type, object), Func<object>> factories = new();
        private Dictionary<(Type, object), object> instances = new();

        public void Register<T>(Func<T> factory, object id = null)
        {
            if (factory == null)
            {
                Debug.LogWarning($"[Injector] Tried to register null for {typeof(T)}");
                return;
            }
            factories[(typeof(T), id)] = () => factory();
#if UNITY_EDITOR
            //Debug.Log($"<color=cyan>[DI]</color> Registering: {(typeof(T), id)} with ID: {id ?? "NULL"}");
#endif
        }
        public T Resolve<T>(object id = null)
        {
            var type = (typeof(T), id);
#if UNITY_EDITOR
            //Debug.Log($"<color=yellow>[DI]</color> Searching: {type.Item1} with ID: {id ?? "NULL"}");
#endif
            if (!instances.TryGetValue(type, out var instance) || IsUnityObjectDestroyed(instance))
            {
                if (!factories.TryGetValue(type, out var factory))
                {
                    throw new Exception($"No service registered for {type}");
                }
                instance = factory();
                instances[type] = instance;
            }
            return (T)instance;
        }
        private bool IsUnityObjectDestroyed(object obj)
        {
            if (obj is UnityEngine.Object unityObj)
            {
                return unityObj == null;
            }
            return obj == null;
        }
        public void ClearInstances()
        {
            instances.Clear();
        }
    }
}