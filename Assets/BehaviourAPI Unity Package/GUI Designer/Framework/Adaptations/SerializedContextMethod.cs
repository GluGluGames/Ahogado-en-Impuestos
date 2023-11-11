using System;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using System.Reflection;
    using UnityToolkit;

    /// <summary>
    /// Data class to serialize a component method call.
    /// </summary>
    [Serializable]

    public class SerializedContextMethod : ICloneable
    {
        /// <summary>
        /// The name of the component. If is empty, the used component will be the behaviour runner.
        /// </summary>
        public string componentName;

        /// <summary>
        /// The name of the method called.
        /// </summary>
        public string methodName;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Delegate GetDelegate(Component defaultComponent, Type[] arguments, Type delegateType)
        {
            if (string.IsNullOrWhiteSpace(methodName) || defaultComponent == null) return null;
            if (!delegateType.IsSubclassOf(typeof(Delegate))) return null;

            Component component = string.IsNullOrWhiteSpace(componentName) ? defaultComponent : defaultComponent.gameObject.GetComponent(componentName);

            if (component == null)
            {
                Debug.LogWarning($"BUILD ERROR: The specified component ({componentName}) does not exist or is not attached to runner.", defaultComponent.gameObject);
                return null;
            }

            MethodInfo methodInfo = component.GetType().GetMethod(methodName, arguments);

            if (methodInfo == null)
            {
                Debug.LogWarning($"BUILD ERROR: The method called {methodName} was not found. Can't create a delegate.", defaultComponent.gameObject);
                return null;
            }

            try
            {
                var del = methodInfo.CreateDelegate(delegateType, component);
                return del;
            }
            catch
            {
                Debug.LogWarning("BUILD ERROR: The method don't match the required parameters. Can't create a delegate.", defaultComponent.gameObject);
                return null;
            }
            
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(methodName)) return "";
            return $"{(string.IsNullOrEmpty(componentName) ? "$runner" : componentName)}.{methodName}";
        }
    }

    /// <summary>
    /// Serialized method data that uses their fields to generate a delegate of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the delegate that stores the method.</typeparam>
    [Serializable]
    public class SerializedContextMethod<T> : SerializedContextMethod where T : Delegate
    {
        protected virtual Type[] FunctionArgs => new Type[0];
       
        public T CreateDelegate(Component runner)
        {
            var del = GetDelegate(runner, FunctionArgs, typeof(T));
            if(del is T typedDelegate)
            {
                return typedDelegate;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Serialized method for void events.
    /// </summary>
    [Serializable]
    public class ContextualSerializedAction : SerializedContextMethod<Action>
    {
    }

    /// <summary>
    /// Serialized method for action update event.
    /// </summary>
    [Serializable]
    public class ContextualSerializedStatusFunction : SerializedContextMethod<Func<Status>>
    {
    }

    /// <summary>
    /// Serialized method for PerceptionReference check event.
    /// </summary>
    [Serializable]
    public class ContextualSerializedBoolFunction : SerializedContextMethod<Func<bool>>
    {
    }
}