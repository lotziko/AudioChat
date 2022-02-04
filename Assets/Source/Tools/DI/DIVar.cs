
namespace Tools
{
	public class DIVar<T> where T : class
    {
        T _value;
        object _id;

        public DIVar(object id = null)
        { _id = id; }

        public T Value
        {
            get
            {
                if (_value == null)
                    _value = DI.Get<T>(_id);

                return _value;
            }
        }
    }
}
