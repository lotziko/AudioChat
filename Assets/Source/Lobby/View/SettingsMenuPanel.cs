using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tools;
using UnityEngine;

namespace AudioChat
{
	public class SettingsMenuPanel : MonoBehaviour
	{
		DIVar<IProfileManager> _profileManager = new DIVar<IProfileManager>();
		DIVar<ISystemInfoManager> _systemInfoManager = new DIVar<ISystemInfoManager>();

		public static void Create(MenuContainer container)
		{
			GameObject prefab = Resources.Load("SettingsMenuPanel") as GameObject;
			GameObject go = Instantiate(prefab) as GameObject;
			SettingsMenuPanel me = go.GetComponent<SettingsMenuPanel>();
			go.GetComponent<MenuPanel>().Open(container, new DefferedMethod(me.Init));
		}

		// =============================================================

		[SerializeField] private TMP_Dropdown _microphoneSettings;
		[SerializeField] private TMP_Dropdown _audioTypeSettings;

		private string[] _microphoneDevices;
		private List<VolumeCalculationMode> _audioModes;

		// =============================================================

		private void Init()
		{
			_microphoneDevices = Microphone.devices;
			List<string> devices = _microphoneDevices.ToList();
			_microphoneSettings.AddOptions(devices);
			_microphoneSettings.value = devices.IndexOf(_profileManager.Value.GetCurrentMicrophone());

			_audioModes = _systemInfoManager.Value.GetAvailableVolumeCalculationModes();
			List<string> audioModesOptions = _audioModes.Select((i) => i.ToString()).ToList();
			_audioTypeSettings.AddOptions(audioModesOptions);
			_audioTypeSettings.value = _audioModes.IndexOf(_profileManager.Value.GetVolumeCalculationMode());
		}

		// =============================================================

		public void OnConfirmClick()
		{
			GetComponent<MenuPanel>().Close();
			_profileManager.Value.SetCurrentMicrophone(_microphoneDevices[_microphoneSettings.value]);
			_profileManager.Value.SetVolumeCalculationMode(_audioModes[_audioTypeSettings.value]);
		}
	}
}
