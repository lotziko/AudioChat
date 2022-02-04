using UnityEngine.EventSystems;

namespace Tools
{
	public class VRInputField : TMPro.TMP_InputField
	{
		DIVar<IVRKeyboardManager> _keyboardManager = new DIVar<IVRKeyboardManager>();

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			_keyboardManager.Value?.Open(this);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			//base.OnDeselect(eventData);
		}
	}
}
