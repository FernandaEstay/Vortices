using System;
using System.Collections;
using Gamelogic;
using Memoria.Core;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
	public class LookPointer : GLMonoBehaviour, IAwake, IFixedUpdate
	{
		[SerializeField]
		private float _positionSteps = 1.0f;
		[SerializeField]
		private float _rotationSteps = 2.0f;
		[SerializeField]
		private float _scaleSteps = 0.2f;

		protected DIOManager dioManager;

		private float _initialAlpha;
		[HideInInspector]
		public bool zoomingIn;
		[HideInInspector]
		public bool zoomingOut;

		[HideInInspector]
		public PitchGrabObject actualPitchGrabObject;
		[HideInInspector]
		public PitchGrabObject posibleActualPitchGrabObject;

		private Vector3 _actualPitchObjectOriginalPosition;
		private Quaternion _actualPitchObjectOriginalRotation;
		private Vector3 _actualPitchObjectOriginalScale;

		public void Initialize(DIOManager fatherDioManager)
		{
			dioManager = fatherDioManager;
		}

		#region Unity Callback

		public void Awake()
		{
			zoomingIn = false;
			zoomingOut = false;
			actualPitchGrabObject = null;
			posibleActualPitchGrabObject = null;
		}

		public void FixedUpdate()
		{
			if ((ZoomOutKeyboardInput() || ZoomOutJoystickInput())
				&& actualPitchGrabObject != null && !dioManager.movingSphere)
			{
				if (!zoomingOut && !zoomingIn)
				{
					StartCoroutine(ZoomingOut(null));
				}
			}

			if ((AcceptObjectKeyboardInput() || AcceptObjectJoystickInput())
				&& posibleActualPitchGrabObject != null && !dioManager.movingSphere)
			{
				if (!zoomingOut && !zoomingIn)
				{
					AcceptObject();
				}
			}
		}

		public void LookPointerEnter(PitchGrabObject pitchGrabObject)
		{
			_initialAlpha = pitchGrabObject.dioController.sphereController.sphereAlpha;
			var objectColor = pitchGrabObject.objectMeshRender.material.color;
			objectColor.a = 1.0f;
			pitchGrabObject.objectMeshRender.material.color = objectColor;

			posibleActualPitchGrabObject = pitchGrabObject;
		}

		public void LookPointerStay(PitchGrabObject pitchGrabObject)
		{
			posibleActualPitchGrabObject = pitchGrabObject;

			if ((ZoomInKeyboardInput() || ZoomInJoystickInput()) &&
				actualPitchGrabObject == null && !dioManager.movingSphere)
			{
				if (!zoomingIn && !zoomingOut)
				{
					StartCoroutine(ZoomingIn(pitchGrabObject, null));
				}
			}
		}

		public void LookPointerExit(PitchGrabObject pitchGrabObject)
		{
			if (actualPitchGrabObject == null)
			{
				var objectColor = pitchGrabObject.objectMeshRender.material.color;
				objectColor.a = _initialAlpha;
				pitchGrabObject.objectMeshRender.material.color = objectColor;
			}

			posibleActualPitchGrabObject = null;
		}

		#endregion

		#region ZoomIn

		public void SetZoomInInitialStatus(PitchGrabObject pitchGrabObject)
		{
			actualPitchGrabObject = pitchGrabObject;
			actualPitchGrabObject.dioController.inSpherePosition = false;
			_actualPitchObjectOriginalPosition = pitchGrabObject.transform.position;
			_actualPitchObjectOriginalRotation = pitchGrabObject.transform.rotation;
			_actualPitchObjectOriginalScale = pitchGrabObject.transform.localScale;
		}

		public void DirectZoomInCall(Action finalAction)
		{
			if (!zoomingIn && !zoomingOut && actualPitchGrabObject == null && !dioManager.movingSphere)
			{
				StartCoroutine(ZoomingIn(posibleActualPitchGrabObject, finalAction));
			}
		}

		public void DirectZoomInCall(PitchGrabObject pitchGrabObject, Action finalAction)
		{
			if (!zoomingIn && !zoomingOut && actualPitchGrabObject == null && !dioManager.movingSphere)
			{
				StartCoroutine(ZoomingIn(pitchGrabObject, finalAction));
			}
		}

		private IEnumerator ZoomingIn(PitchGrabObject pitchGrabObject, Action finalAction)
		{
			zoomingIn = true;
			SetZoomInInitialStatus(pitchGrabObject);

			dioManager.csvCreator.AddLines("ZoomingIn", pitchGrabObject.idName);

			var counter = 0;
			while (true)
			{
				pitchGrabObject.transform.position =
					Vector3.MoveTowards(pitchGrabObject.transform.position,
						Vector3.zero, 0.01f);

				if (counter >= dioManager.closeRange)
				{
					break;
				}

				counter++;
				yield return new WaitForFixedUpdate();
			}

			if (finalAction != null)
				finalAction();

			zoomingIn = false;
		}

		private bool ZoomInKeyboardInput()
		{
			//if (dioManager.useKeyboard && !dioManager.useMouse)
			//{
			//	return Input.GetKeyDown(dioManager.action1Key);
			//}

			return false;
		}

		private bool ZoomInJoystickInput()
		{
			//if (dioManager.useJoystick)
			//{
			//	var zoomIn = Input.GetAxis("Action1");

			//	return zoomIn == 1.0f;
			//}

			return false;
		}

		#endregion

		#region ZoomOut

		public void DirectZoomOutCall(Action finalAction)
		{
			if (!zoomingOut && !zoomingIn && actualPitchGrabObject != null && !dioManager.movingSphere)
			{
				StartCoroutine(ZoomingOut(finalAction));
			}
		}

		private IEnumerator ZoomingOut(Action finalAction)
		{
			dioManager.csvCreator.AddLines("ZoomingOut", actualPitchGrabObject.idName);
			zoomingOut = true;

			var positionTargetReached = false;
			var rotationTargetReached = false;
			var scaleTargetReaced = false;

			while (true)
			{
				//Position
				actualPitchGrabObject.transform.position =
					Vector3.MoveTowards(actualPitchGrabObject.transform.position,
						_actualPitchObjectOriginalPosition, _positionSteps);

				if (actualPitchGrabObject.transform.position.EqualOrMayorCompareVector(_actualPitchObjectOriginalPosition, -0.0001f) && !positionTargetReached)
				{
					positionTargetReached = true;
					actualPitchGrabObject.transform.position = _actualPitchObjectOriginalPosition;
				}

				//Rotation
				actualPitchGrabObject.transform.rotation =
					Quaternion.RotateTowards(actualPitchGrabObject.transform.rotation,
						_actualPitchObjectOriginalRotation, _rotationSteps);

				if (actualPitchGrabObject.transform.rotation.EqualOrMayorCompareQuaternion(_actualPitchObjectOriginalRotation, -0.0001f) && !rotationTargetReached)
				{
					rotationTargetReached = true;
					actualPitchGrabObject.transform.rotation = _actualPitchObjectOriginalRotation;
				}

				actualPitchGrabObject.transform.parent.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				//Scale
				actualPitchGrabObject.transform.localScale =
					Vector3.MoveTowards(actualPitchGrabObject.transform.localScale,
						_actualPitchObjectOriginalScale, _scaleSteps);

				if (actualPitchGrabObject.transform.localScale.EqualOrMinorCompareVector(_actualPitchObjectOriginalScale, 0.001f) && !scaleTargetReaced)
				{
					scaleTargetReaced = true;
					actualPitchGrabObject.transform.localScale = _actualPitchObjectOriginalScale;
				}

				if (positionTargetReached && rotationTargetReached && scaleTargetReaced)
					break;

				yield return new WaitForFixedUpdate();
			}

			actualPitchGrabObject.OnUnDetect();
			actualPitchGrabObject.dioController.inSpherePosition = true;
			actualPitchGrabObject = null;
			zoomingOut = false;

			if (finalAction != null)
				finalAction();
		}

		private bool ZoomOutKeyboardInput()
		{
			//if (dioManager.useKeyboard && !dioManager.useMouse)
			//{
			//	return Input.GetKeyDown(dioManager.action2Key);
			//}

			return false;
		}

		private bool ZoomOutJoystickInput()
		{
			//if (dioManager.useJoystick)
			//{
			//	var zoomIn = Input.GetAxis("Action2");

			//	return zoomIn == 1.0f;
			//}

			return false;
		}

		#endregion

		#region Accept

		public void AcceptObject()
		{
			print("accept call");
			bool unPitchedAccept = false;

			if (actualPitchGrabObject == null)
			{
				if (posibleActualPitchGrabObject == null)
					return;

				unPitchedAccept = true;
				actualPitchGrabObject = posibleActualPitchGrabObject;
			}

			var pitchMaterial = actualPitchGrabObject.GetComponent<MeshRenderer>();

			actualPitchGrabObject.isSelected = !actualPitchGrabObject.isSelected;
			pitchMaterial.material.color = actualPitchGrabObject.isSelected ? Color.green : Color.white;

			if (actualPitchGrabObject.isSelected)
			{
				dioManager.buttonPanel.NegativeAcceptButton();
			}
			else
			{
				dioManager.buttonPanel.PositiveAcceptButton();
			}

			var action = actualPitchGrabObject.isSelected ? "Select" : "Deselect";
			dioManager.csvCreator.AddLines(action, actualPitchGrabObject.idName);

			if (unPitchedAccept)
				actualPitchGrabObject = null;
		}

		private bool AcceptObjectKeyboardInput()
		{
			if (dioManager.useKeyboard && !dioManager.useMouse)
			{
				return Input.GetKeyDown(dioManager.action5Key);
			}

			return false;
		}

		private bool _acceptJoystickPushed = false;
		private bool AcceptObjectJoystickInput()
		{
			if (dioManager.useJoystick)
			{
				var zoomIn = Input.GetAxis("Submit");

				if (zoomIn == 1.0f)
				{
					if (_acceptJoystickPushed)
						return false;

					_acceptJoystickPushed = true;

					return true;
				}

				if (zoomIn == 0.0f)
				{
					_acceptJoystickPushed = false;
				}
			}

			return false;
		}

		#endregion
	}
}