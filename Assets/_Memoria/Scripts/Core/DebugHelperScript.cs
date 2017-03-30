using Gamelogic;
using UnityEngine;
using UnityCallbacks;
using UnityEngine.SceneManagement;

namespace Memoria.Core
{
	public class DebugHelperScript : GLMonoBehaviour, IUpdate
	{
		public KeyCode restartKey;
		public string sceneName;

		public void Update()
		{
			if (Input.GetKeyDown(restartKey))
			{
				SceneManager.LoadScene(sceneName);
			}
		}
	}
}