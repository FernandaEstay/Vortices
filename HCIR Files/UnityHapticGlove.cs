using UnityEngine;
using System.Collections.Generic;

namespace HCIR
{
	public class UnityHapticGlove : MonoBehaviour
	{

		private HapticsGlove.HapticsGlove glove;
		private List<int> pins;

		public void Start()
		{
			pins = new List<int>() {10};

			glove = new HapticsGlove.HapticsGlove();

			Debug.Log("Haptics Glove Output");
			foreach (var portName in glove.GetPortNames())
			{
				Debug.Log(portName);
			}

			glove.OpenPort("COM5", 9600);
			glove.InitializeMotor(pins);

			Debug.Log("Ports Open");
		}

		public void ActivateMotor(string impact)
		{
			Debug.Log("activated");
			glove.ActivateMotor(pins, new List<string>() {impact});
		}

		public void DeactivateMotor()
		{
			Debug.Log("de-activated");
			glove.ActivateMotor(pins, new List<string>() {"0"});
		}
	}
}