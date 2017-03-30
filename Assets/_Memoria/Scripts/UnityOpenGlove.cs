using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Gamelogic;
using OpenGloveClass = OpenGlove.OpenGlove;

namespace Memoria
{
	public class UnityOpenGlove : GLMonoBehaviour
	{
		public bool outputLog;
		public string leftComDevice = "COM5";
		public int leftBaudRate = 57600;
		public string rightComDevice = "COM8";
		public int rightBaudRate = 57600;

		private OpenGloveClass _leftGlove;
		private OpenGloveClass _rightGlove;

		private List<int> _positivePins;
		private List<int> _negativePins;

		private List<int> _leftIndex;
		private List<int> _rightIndex;
		private List<int> _leftMiddle;
		private List<int> _rightMiddle;
		private List<int> _leftThumb;
		private List<int> _rightThumb;
		private List<int> _leftPinky;
		private List<int> _rightPinky;
		private List<int> _leftRing;
		private List<int> _rightRing;
		private List<int> _palm;

		private List<string> _valuesOff;

		private DIOManager _dioManager;

		public void Initialize(DIOManager dioManager)
		{
			_dioManager = dioManager;

			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			_positivePins = new List<int> { 11, 10, 9, 6, 5, 3 };
			_negativePins = new List<int> { 16, 15, 14, 12, 8, 2 };

			_leftPinky = new List<int> { 10 };
			_leftRing = new List<int> { 9 };
			_leftMiddle = new List<int> { 3 };
			_leftIndex = new List<int> { 5 };
			_leftThumb = new List<int> { 6 };

			_rightThumb = new List<int> { 10 };
			_rightIndex = new List<int> { 9 };
			_rightMiddle = new List<int> { 6 };
			_rightRing = new List<int> { 5 };
			_rightPinky = new List<int> { 3 };

			_palm = new List<int> { 11 };

			_valuesOff = new List<string> { "LOW", "LOW", "LOW", "LOW", "LOW", "LOW" };

			_leftGlove = new OpenGloveClass();
			_rightGlove = new OpenGloveClass();

			PrintDebug("Start Left Glove");
			_leftGlove.OpenPort(leftComDevice, leftBaudRate);
			System.Threading.Thread.Sleep(2000);
			_leftGlove.InitializeMotor(_positivePins);
			_leftGlove.InitializeMotor(_negativePins);
			_leftGlove.ActivateMotor(_negativePins, _valuesOff);
			_leftGlove.ActivateMotor(_negativePins, _valuesOff);

			PrintDebug("Start Right Glove");
			_rightGlove.OpenPort(rightComDevice, rightBaudRate);
			System.Threading.Thread.Sleep(2000);
			_rightGlove.InitializeMotor(_positivePins);
			_rightGlove.InitializeMotor(_negativePins);
			_rightGlove.ActivateMotor(_negativePins, _valuesOff);
			_rightGlove.ActivateMotor(_negativePins, _valuesOff);

			PrintDebug("Ports Open");

			DeactivateLeftHand();
			DeactivateRightHand();
		}

		public void ActivateLeftHand(string impact)
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			ActivateMotorLeftPinky(impact);
			ActivateMotorLeftRing(impact);
			ActivateMotorLeftMiddle(impact);
			ActivateMotorLeftIndex(impact);
			ActivateMotorLeftThumb(impact);
		}

		public void ActivateRightHand(string impact)
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			ActivateMotorRightPinky(impact);
			ActivateMotorRightRing(impact);
			ActivateMotorRightMiddle(impact);
			ActivateMotorRightIndex(impact);
			ActivateMotorRightThumb(impact);
		}

		public void DeactivateLeftHand()
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			DeactivateMotorLeftPinky();
			DeactivateMotorLeftRing();
			DeactivateMotorLeftMiddle();
			DeactivateMotorLeftIndex();
			DeactivateMotorLeftThumb();
		}

		public void DeactivateRightHand()
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			DeactivateMotorRightPinky();
			DeactivateMotorRightRing();
			DeactivateMotorRightMiddle();
			DeactivateMotorRightIndex();
			DeactivateMotorRightThumb();
		}

		#region Left Hand

		public void ActivateMotorLeftIndex(string impact)
		{
			PrintDebug("Left Index activated");
			_leftGlove.ActivateMotor(_leftIndex, new List<string> { impact });
		}

		public void DeactivateMotorLeftIndex()
		{
			PrintDebug("Left Index de-activated");
			_leftGlove.ActivateMotor(_leftIndex, new List<string> { "0" });
		}

		public void ActivateMotorLeftMiddle(string impact)
		{
			PrintDebug("Left Middle activated");
			_leftGlove.ActivateMotor(_leftMiddle, new List<string> { impact });
		}

		public void DeactivateMotorLeftMiddle()
		{
			PrintDebug("Left Middle de-activated");
			_leftGlove.ActivateMotor(_leftMiddle, new List<string> { "0" });
		}

		public void ActivateMotorLeftThumb(string impact)
		{
			PrintDebug("Left Thumb activated");
			_leftGlove.ActivateMotor(_leftThumb, new List<string> { impact });
		}

		public void DeactivateMotorLeftThumb()
		{
			PrintDebug("Left Thumb de-activated");
			_leftGlove.ActivateMotor(_leftThumb, new List<string> { "0" });
		}

		public void ActivateMotorLeftPinky(string impact)
		{
			PrintDebug("Left Pinky activated");
			_leftGlove.ActivateMotor(_leftPinky, new List<string> { impact });
		}

		public void DeactivateMotorLeftPinky()
		{
			PrintDebug("Left Pinky de-activated");
			_leftGlove.ActivateMotor(_leftPinky, new List<string> { "0" });
		}

		public void ActivateMotorLeftRing(string impact)
		{
			PrintDebug("Left Ring activated");
			_leftGlove.ActivateMotor(_leftRing, new List<string> { impact });
		}

		public void DeactivateMotorLeftRing()
		{
			PrintDebug("Left Ring de-activated");
			_leftGlove.ActivateMotor(_leftRing, new List<string> { "0" });
		}

		public void ActivateMotorLeftPalm(string impact)
		{
			PrintDebug("Left Palm activated");
			_leftGlove.ActivateMotor(_palm, new List<string> { impact });
		}

		public void DeactivateMotorLeftPalm()
		{
			PrintDebug("Left Palm de-activated");
			_leftGlove.ActivateMotor(_palm, new List<string> { "0" });
		}

		#endregion

		#region Right Hand

		public void ActivateMotorRightIndex(string impact)
		{
			PrintDebug("Right Index activated");
			_rightGlove.ActivateMotor(_rightIndex, new List<string> { impact });
		}

		public void DeactivateMotorRightIndex()
		{
			PrintDebug("Right Index de-activated");
			_rightGlove.ActivateMotor(_rightIndex, new List<string> { "0" });
		}

		public void ActivateMotorRightMiddle(string impact)
		{
			PrintDebug("Right Middle activated");
			_rightGlove.ActivateMotor(_rightMiddle, new List<string> { impact });
		}

		public void DeactivateMotorRightMiddle()
		{
			PrintDebug("Right Middle de-activated");
			_rightGlove.ActivateMotor(_rightMiddle, new List<string> { "0" });
		}

		public void ActivateMotorRightThumb(string impact)
		{
			PrintDebug("Right Thumb activated");
			_rightGlove.ActivateMotor(_rightThumb, new List<string> { impact });
		}

		public void DeactivateMotorRightThumb()
		{
			PrintDebug("Right Thumb de-activated");
			_rightGlove.ActivateMotor(_rightThumb, new List<string> { "0" });
		}

		public void ActivateMotorRightPinky(string impact)
		{
			PrintDebug("Right Pinky activated");
			_rightGlove.ActivateMotor(_rightPinky, new List<string> { impact });
		}

		public void DeactivateMotorRightPinky()
		{
			PrintDebug("Right Pinky de-activated");
			_rightGlove.ActivateMotor(_rightPinky, new List<string> { "0" });
		}

		public void ActivateMotorRightRing(string impact)
		{
			PrintDebug("Right Ring activated");
			_rightGlove.ActivateMotor(_rightRing, new List<string> { impact });
		}

		public void DeactivateMotorRightRing()
		{
			PrintDebug("Right Ring de-activated");
			_rightGlove.ActivateMotor(_rightRing, new List<string> { "0" });
		}

		public void ActivateMotorRightPalm(string impact)
		{
			PrintDebug("Right Palm activated");
			_rightGlove.ActivateMotor(_palm, new List<string> { impact });
		}

		public void DeactivateMotorRightPalm()
		{
			PrintDebug("Right Palm de-activated");
			_rightGlove.ActivateMotor(_palm, new List<string> { "0" });
		}

		#endregion

		public void PrintDebug(string output)
		{
			if (!outputLog)
				return;

			Debug.Log(string.Format("Open Glove: {0}", output));
		}

		public void DeactivateInSeconds(float seconds)
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			StartCoroutine(DeactivateInSecondsC(seconds));
		}

		public IEnumerator DeactivateInSecondsC(float seconds)
		{
			ActivateLeftHand("255");
			ActivateRightHand("255");
			yield return new WaitForSeconds(seconds);
			DeactivateLeftHand();
			DeactivateRightHand();
		}
	}
}