using System;
namespace AudioChat
{
	public interface IDefferedMethod
	{
		void Call();
	}

	public class DefferedMethod : IDefferedMethod
	{
		Action _method;

		public DefferedMethod(Action method)
		{ _method = method; }

		void IDefferedMethod.Call()
		{ _method(); }
	}

	public class DefferedMethod<T> : IDefferedMethod
	{
		Action<T> _method;
		T _val;

		public DefferedMethod(Action<T> method, T val)
		{
			_method = method;
			_val = val;
		}

		void IDefferedMethod.Call()
		{ _method(_val); }
	}

	public class DefferedMethod<T1, T2> : IDefferedMethod
	{
		Action<T1, T2> _method;
		T1 _val1;
		T2 _val2;

		public DefferedMethod(Action<T1, T2> method, T1 val_1, T2 val_2)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2); }
	}

	public class DefferedMethod<T1, T2, T3> : IDefferedMethod
	{
		Action<T1, T2, T3> _method;
		T1 _val1;
		T2 _val2;
		T3 _val3;

		public DefferedMethod(Action<T1, T2, T3> method, T1 val_1, T2 val_2, T3 val_3)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
			_val3 = val_3;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2, _val3); }
	}

	public class DefferedMethod<T1, T2, T3, T4> : IDefferedMethod
	{
		Action<T1, T2, T3, T4> _method;
		T1 _val1;
		T2 _val2;
		T3 _val3;
		T4 _val4;

		public DefferedMethod(Action<T1, T2, T3, T4> method, T1 val_1, T2 val_2, T3 val_3, T4 val_4)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
			_val3 = val_3;
			_val4 = val_4;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2, _val3, _val4); }
	}

	public class DefferedMethod<T1, T2, T3, T4, T5> : IDefferedMethod
	{
		public delegate void DifferedAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
		DifferedAction _method;
		T1 _val1;
		T2 _val2;
		T3 _val3;
		T4 _val4;
		T5 _val5;

		public DefferedMethod(DifferedAction method, T1 val_1, T2 val_2, T3 val_3, T4 val_4, T5 val_5)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
			_val3 = val_3;
			_val4 = val_4;
			_val5 = val_5;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2, _val3, _val4, _val5); }
	}

	public class DefferedMethod<T1, T2, T3, T4, T5, T6> : IDefferedMethod
	{
		public delegate void DifferedAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
		DifferedAction _method;
		T1 _val1;
		T2 _val2;
		T3 _val3;
		T4 _val4;
		T5 _val5;
		T6 _val6;

		public DefferedMethod(DifferedAction method, T1 val_1, T2 val_2, T3 val_3, T4 val_4, T5 val_5, T6 val_6)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
			_val3 = val_3;
			_val4 = val_4;
			_val5 = val_5;
			_val6 = val_6;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2, _val3, _val4, _val5, _val6); }
	}

	public class DefferedMethod<T1, T2, T3, T4, T5, T6, T7> : IDefferedMethod
	{
		public delegate void DifferedAction(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
		DifferedAction _method;
		T1 _val1;
		T2 _val2;
		T3 _val3;
		T4 _val4;
		T5 _val5;
		T6 _val6;
		T7 _val7;

		public DefferedMethod(DifferedAction method, T1 val_1, T2 val_2, T3 val_3, T4 val_4, T5 val_5, T6 val_6, T7 val_7)
		{
			_method = method;
			_val1 = val_1;
			_val2 = val_2;
			_val3 = val_3;
			_val4 = val_4;
			_val5 = val_5;
			_val6 = val_6;
			_val7 = val_7;
		}

		void IDefferedMethod.Call()
		{ _method(_val1, _val2, _val3, _val4, _val5, _val6, _val7); }
	}

	public interface IDefferedCall
	{
		void PushLocker(string id);
		void PopLocker(string id);
	}

	public interface IDefferedCallInternal
	{
		IDefferedCall GetIIDefferedCall();
		bool IsLock();
		void Push(IDefferedMethod callBack);
	}
}
