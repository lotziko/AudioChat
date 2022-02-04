using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public interface IDIContextContainer
    {
        void Bind<T>(T instance, object id = null) where T : class;
        void UnBind<T>(T instance, object id = null) where T : class;
        void UnBindAll();
        T TryGet<T>(object id = null) where T : class;
    }

    // #################################

    public class DIContextContainer : IDIContextContainer
    {
        Dictionary<ContainerType, object> _instances = new Dictionary<ContainerType, object>();

        // =============== IDIContextContainer ===============

        void IDIContextContainer.Bind<T>(T instance, object id)
        {
            ContainerType t = new ContainerType(typeof(T), id);

            if (_instances.ContainsKey(t))
                Debug.Log("DIContextContainer.Bind: instance already exists");

            _instances[t] = instance;
        }

        void IDIContextContainer.UnBind<T>(T instance, object id)
        {
            ContainerType t = new ContainerType(typeof(T), id);
            _instances.Remove(t);
        }

        void IDIContextContainer.UnBindAll()
        { _instances.Clear(); }

        T IDIContextContainer.TryGet<T>(object id)
        {
            ContainerType t = new ContainerType(typeof(T), id);

            if (!_instances.TryGetValue(t, out object result))
                return null;

            return result as T;
        }
        
        // ===================================================

        class ContainerType
        {
            public ContainerType(Type obj, object id = null)
            {
                Obj = obj;
                Id = id;
            }

            public Type Obj { get; private set; }
            public object Id { get; private set; }

            public override int GetHashCode()
            { return Obj.GetHashCode(); }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                ContainerType other = (ContainerType)obj;
                return other.Obj == this.Obj && other.Id == this.Id;
            }
        }
    }
}
