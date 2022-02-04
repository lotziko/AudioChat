using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AudioChat
{
	public class LoadingManager : MonoBehaviour
	{
		DIVar<IXRManager> _xrManager = new DIVar<IXRManager>();

		private void Start()
		{
			BindManagers();

			AppType type = _xrManager.Value.GetAppType();
			switch (type)
			{
				case AppType.PC:
					SceneManager.LoadScene("LobbyScenePC");
					break;
				case AppType.VR:
					SceneManager.LoadScene("LobbySceneVR");
					break;
			}
		}

		private void BindManagers()
		{
			DI.Bind<IXRManager>(new XRManager());
			DI.Bind<IProfileManager>(new ProfileManager());
			DI.Bind<ISystemInfoManager>(new SystemInfoManager());
		}
	}
}
