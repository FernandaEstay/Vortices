using System;
using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
	public class HapticDetector : GLMonoBehaviour, IOnValidate, IOnTriggerEnter, IOnTriggerExit
	{
		public string impact = "255";
		private DIOManager _dioManager;

		public void OnValidate()
		{
			try
			{
				var impactValue = Convert.ToInt32(impact);
				impactValue = Mathf.Clamp(impactValue, 0, 255);

				impact = impactValue.ToString();
			}
			catch (Exception)
			{
				impact = "255";
			}
		}

		public void Initialize(DIOManager dioManager)
		{
			_dioManager = dioManager;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (_dioManager == null)
				return;

			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			var handMapping = other.GetComponent<HandMapping>();

			if (handMapping == null)
				return;

			var unityOpenGlove = _dioManager.unityOpenGlove;

			switch (handMapping.handMap)
			{
				case HandMap.LeftIndex:
					unityOpenGlove.ActivateMotorLeftIndex(impact);
					break;
				case HandMap.RightIndex:
					unityOpenGlove.ActivateMotorRightIndex(impact);
					break;
				case HandMap.LeftMiddle:
					unityOpenGlove.ActivateMotorLeftMiddle(impact);
					break;
				case HandMap.RightMiddle:
					unityOpenGlove.ActivateMotorRightMiddle(impact);
					break;
				case HandMap.LeftThumb:
					unityOpenGlove.ActivateMotorLeftThumb(impact);
					break;
				case HandMap.RightThumb:
					unityOpenGlove.ActivateMotorRightThumb(impact);
					break;
				case HandMap.LeftPinky:
					unityOpenGlove.ActivateMotorLeftPinky(impact);
					break;
				case HandMap.RightPinky:
					unityOpenGlove.ActivateMotorRightPinky(impact);
					break;
				case HandMap.LeftRing:
					unityOpenGlove.ActivateMotorLeftRing(impact);
					break;
				case HandMap.RightRing:
					unityOpenGlove.ActivateMotorRightRing(impact);
					break;
				case HandMap.LeftPalm:
					unityOpenGlove.ActivateMotorLeftPalm(impact);
					break;
				case HandMap.RightPalm:
					unityOpenGlove.ActivateMotorRightPalm(impact);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (!_dioManager.useLeapMotion || !_dioManager.useHapticGlove)
				return;

			var handMapping = other.GetComponent<HandMapping>();

			if (handMapping == null)
				return;

			var unityOpenGlove = _dioManager.unityOpenGlove;

			switch (handMapping.handMap)
			{
				case HandMap.LeftIndex:
					unityOpenGlove.DeactivateMotorLeftIndex();
					break;
				case HandMap.RightIndex:
					unityOpenGlove.DeactivateMotorRightIndex();
					break;
				case HandMap.LeftMiddle:
					unityOpenGlove.DeactivateMotorLeftMiddle();
					break;
				case HandMap.RightMiddle:
					unityOpenGlove.DeactivateMotorRightMiddle();
					break;
				case HandMap.LeftThumb:
					unityOpenGlove.DeactivateMotorLeftThumb();
					break;
				case HandMap.RightThumb:
					unityOpenGlove.DeactivateMotorRightThumb();
					break;
				case HandMap.LeftPinky:
					unityOpenGlove.DeactivateMotorLeftPinky();
					break;
				case HandMap.RightPinky:
					unityOpenGlove.DeactivateMotorRightPinky();
					break;
				case HandMap.LeftRing:
					unityOpenGlove.DeactivateMotorLeftRing();
					break;
				case HandMap.RightRing:
					unityOpenGlove.DeactivateMotorRightRing();
					break;
				case HandMap.LeftPalm:
					unityOpenGlove.DeactivateMotorLeftPalm();
					break;
				case HandMap.RightPalm:
					unityOpenGlove.DeactivateMotorRightPalm();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
