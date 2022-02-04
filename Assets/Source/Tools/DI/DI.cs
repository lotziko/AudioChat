using System.Collections.Generic;

namespace Tools
{
    public static class DI
    {
        static IDIContextContainer _defaultContainer = new DIContextContainer();
        static List<IDIContextContainer> _containers = new List<IDIContextContainer>();

        // =============================

        public static void Push(IDIContextContainer context)
        {
            _containers.Add(context);
        }

        public static void Pop(IDIContextContainer context)
        {
            _containers.Remove(context);
        }

        public static void Bind<T>(T instance, object id = null) where T : class
        {
            _defaultContainer.Bind<T>(instance, id);
        }

        public static void UnBind<T>(T instance, object id = null) where T : class
        {
            _defaultContainer.UnBind<T>(instance, id);
        }

        public static void UnBindAll()
        {
            _defaultContainer.UnBindAll();
            _containers.Clear();
        }

        public static T Get<T>(object id = null) where T : class
        {
            T result = _defaultContainer.TryGet<T>(id);
            if (result != null)
                return result;

            foreach (IDIContextContainer curContainer in _containers)
            {
                result = curContainer.TryGet<T>(id);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
