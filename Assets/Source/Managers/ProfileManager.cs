using System.Linq;
using UnityEngine;

namespace AudioChat
{
	public interface IProfileManager
	{
		string GetNickname();
		void SetNickname(string nickname);

		string GetCurrentMicrophone();
		void SetCurrentMicrophone(string name);

		VolumeCalculationMode GetVolumeCalculationMode();
		void SetVolumeCalculationMode(VolumeCalculationMode mode);
	}

	public class ProfileManager : IProfileManager
	{
		string IProfileManager.GetCurrentMicrophone()
		{
			string[] microphones = Microphone.devices;
			string cachedMicrophone = PlayerPrefs.GetString("cachedMicrophone");
			if (cachedMicrophone == null || !microphones.Contains(cachedMicrophone))
				cachedMicrophone = microphones[0];
			return cachedMicrophone;
		}

		void IProfileManager.SetCurrentMicrophone(string name)
		{
			PlayerPrefs.SetString("cachedMicrophone", name);
		}

		string IProfileManager.GetNickname()
		{
			string nickname = null;
			if (PlayerPrefs.HasKey("cachedNickname"))
				nickname = PlayerPrefs.GetString("cachedNickname");
			else
				nickname = "Player" + Random.Range(0, 1000);
			return nickname;
		}

		void IProfileManager.SetNickname(string nickname)
		{
			PlayerPrefs.SetString("cachedNickname", nickname);
		}

		VolumeCalculationMode IProfileManager.GetVolumeCalculationMode()
		{
			if (!PlayerPrefs.HasKey("cachedVolumeCalculationMode"))
				return VolumeCalculationMode.None;

			return (VolumeCalculationMode)PlayerPrefs.GetInt("cachedVolumeCalculationMode");
		}

		void IProfileManager.SetVolumeCalculationMode(VolumeCalculationMode mode)
		{
			PlayerPrefs.SetInt("cachedVolumeCalculationMode", (int)mode);
		}
	}
}
