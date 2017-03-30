using System;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria.Core
{
	public class TriggerEnterExitAction : GLMonoBehaviour, IOnTriggerEnter, IOnTriggerExit
	{
		public event Action OnTouch;
		public event Action OnStopTouch;

		public void OnTriggerEnter(Collider other)
		{
			if (OnTouch != null)
				OnTouch();
		}

		public void OnTriggerExit(Collider other)
		{
			if (OnStopTouch != null)
				OnStopTouch();
		}
	}
}