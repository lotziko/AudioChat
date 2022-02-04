using UnityEngine;

namespace Tools
{
    public interface IDIContext
    {
        T TryGet<T>(object id = null) where T : class;
    }

    // ########################################

    public abstract class DIContext : MonoBehaviour, IDIContext
    {
        IDIContextContainer _container = new DIContextContainer();

        // ======================================

        void Awake()
        {
            DI.Push(_container);
            OnBind();
        }

        void OnDestroy()
        { DI.Pop(_container); }

        // ======================================

        protected abstract void OnBind();

        protected void Bind<T>(T instance, object id = null) where T : class
        { _container.Bind<T>(instance, id); }

        // ============ IDIContext ==============

        T IDIContext.TryGet<T>(object id)
        {
            return _container.TryGet<T>(id);
        }
    }
}
