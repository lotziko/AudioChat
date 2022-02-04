
using System;

namespace AudioChat
{
	public interface IInputManager
	{
		bool IsLocked { get; }
		event Action OnUpdate;

		void Lock();
		void Unlock();
	}

	public class InputManager : IInputManager
	{
		private bool _isLocked;
		private Action _onUpdate;

		bool IInputManager.IsLocked => _isLocked;

		event Action IInputManager.OnUpdate
		{
			add { _onUpdate += value; }
			remove { _onUpdate -= value; }
		}

		void IInputManager.Lock()
		{
			_isLocked = true;
			_onUpdate?.Invoke();
		}

		void IInputManager.Unlock()
		{
			_isLocked = false;
			_onUpdate?.Invoke();
		}
	}
}
