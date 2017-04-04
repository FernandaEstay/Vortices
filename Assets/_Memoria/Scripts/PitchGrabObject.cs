using System.Linq;
using Gamelogic;
using Leap.Unity;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
	public class PitchGrabObject : GLMonoBehaviour, IOnTriggerStay, IOnMouseOver
	{
		public PinchDetector pinchDetectorLeft;
		public PinchDetector pinchDetectorRight;
		public HapticDetector hapticDetector;

		public bool allowTranslation = true;
		public bool allowRotation = true;
		public bool allowTwoHandScale = true;

		public string[] tagTriggers;

		[HideInInspector]
		public bool isPinched;
		[HideInInspector]
		public MeshRenderer objectMeshRender;
		[HideInInspector]
		public DIOController dioController;
		[HideInInspector]
		public string idName;
		[HideInInspector]
		public bool isSelected;

		protected DIOManager DioManager
		{
			get { return dioController.DioManager; }
		}

		private Transform _anchor;
		private bool _isLookPointerOn;
		private int _id;

		public void Initialize(DIOController fatherDioController, int id)
		{
			_id = id;

			if (fatherDioController.DioManager.usePitchGrab && (pinchDetectorLeft == null || pinchDetectorRight == null))
			{
				Debug.LogWarning(
					"Both Pinch Detectors of the LeapRTS component must be assigned. This component has been disabled.");
				enabled = false;
				return;
			}

			dioController = fatherDioController;
			enabled = true;
			isSelected = false;

			hapticDetector.Initialize(fatherDioController.DioManager);

			objectMeshRender = GetComponent<MeshRenderer>();

			var pinchControl = new GameObject("DIO Anchor");
			_anchor = pinchControl.transform;
			_anchor.transform.parent = transform.parent;
			transform.parent = _anchor;

			isPinched = false;
			_isLookPointerOn = false;
		}

		public void InitializeMaterial(Texture2D texture2D)
		{
			GetComponent<MeshRenderer>().material.mainTexture = texture2D;
		}

		public void OnDetected()
		{
			if (!_isLookPointerOn)
			{
				_isLookPointerOn = true;
				DioManager.lookPointerInstance.LookPointerEnter(this);
                if(!DioManager.mouseInput)
				    DioManager.buttonPanel.EnableAccept();

				if (DioManager.lookPointerInstance.actualPitchGrabObject == null)
					DioManager.buttonPanel.EnableZoomIn();
			}
			else
			{
				DioManager.lookPointerInstance.LookPointerStay(this);
			}
		}

		public void OnUnDetect()
		{
			if (_isLookPointerOn)
			{
				DioManager.lookPointerInstance.LookPointerExit(this);

				if (DioManager.lookPointerInstance.actualPitchGrabObject == null)
				{
					_isLookPointerOn = false;
					DioManager.buttonPanel.DisableAccept();
				}

				DioManager.buttonPanel.DisableZoomIn();

			}
		}

		#region UnityCallbacks

		public void OnTriggerStay(Collider other)
		{
			if (!_isLookPointerOn)
				return;

			if (!dioController.DioManager.usePitchGrab)
				return;

			if (DioManager.lookPointerInstance.actualPitchGrabObject != null)
				if (!DioManager.lookPointerInstance.actualPitchGrabObject.Equals(this))
					return;

			if (!tagTriggers.Contains(other.tag))
				return;

			if (dioController.DioManager.IsAnyDioPitched && !isPinched)
				return;

			if (DioManager.movingSphere || DioManager.lookPointerInstance.zoomingOut)
				return;

			var didUpdate = false;
			didUpdate |= pinchDetectorLeft.DidChangeFromLastFrame;
			didUpdate |= pinchDetectorRight.DidChangeFromLastFrame;

			if (didUpdate)
			{
				transform.SetParent(null, true);
			}

			if (pinchDetectorLeft.IsPinching && pinchDetectorRight.IsPinching)
			{
				TransformDoubleAnchor();
			}
			else if (pinchDetectorLeft.IsPinching)
			{
				TransformSingleAnchor(pinchDetectorLeft);
			}
			else if (pinchDetectorRight.IsPinching)
			{
				TransformSingleAnchor(pinchDetectorRight);
			}
			else
			{
				isPinched = false;
			}

			if (didUpdate)
			{
				transform.SetParent(_anchor, true);
			}
		}

		internal void SetId(string newIdName)
		{
			idName = newIdName;
		}

		public void OnMouseOver()
		{
			if (DioManager.loadingScene.loading)
				return;

			if (DioManager.useKeyboard && DioManager.useMouse && !DioManager.movingSphere)
			{
				if (Input.GetMouseButton(0))
				{
					DioManager.lookPointerInstance.DirectZoomInCall(this, null);
				}
				else if (Input.GetMouseButton(1))
				{
					DioManager.lookPointerInstance.DirectZoomOutCall(null);
				}
			}
		}

		#endregion

		#region Pitch Leap

		private void TransformDoubleAnchor()
		{
			isPinched = true;

			SetInitialState();

			if (allowTranslation)
				_anchor.position = (pinchDetectorLeft.Position + pinchDetectorRight.Position) / 2.0f;

			if (allowRotation)
			{
				Quaternion pp = Quaternion.Lerp(pinchDetectorLeft.Rotation, pinchDetectorRight.Rotation, 0.5f);
				Vector3 u = pp * Vector3.up;
				_anchor.LookAt(pinchDetectorLeft.Position, u);
			}

			if (allowTwoHandScale)
			{
				_anchor.localScale = Vector3.one * Vector3.Distance(pinchDetectorLeft.Position, pinchDetectorRight.Position);
			}
		}

		private void TransformSingleAnchor(PinchDetector singlePinch)
		{
			isPinched = true;

			SetInitialState();

			if (allowTranslation)
			{
				_anchor.position = singlePinch.Position;
			}

			if (allowRotation)
			{
				_anchor.rotation = singlePinch.Rotation;
			}

			_anchor.localScale = Vector3.one;
		}

		private void SetInitialState()
		{
			if (dioController.inSpherePosition)
			{
				dioController.inSpherePosition = false;
                if (!DioManager.mouseInput)
                {
                    DioManager.buttonPanel.EnableZoomOut();
                    DioManager.buttonPanel.EnableAccept();
                }
				DioManager.buttonPanel.DisableMoveCameraInside();
				DioManager.buttonPanel.DisableMoveCameraOutside();

				DioManager.csvCreator.AddLines("PitchZoomIn", idName);

				DioManager.lookPointerInstance.SetZoomInInitialStatus(this);
			}
		}

		#endregion
		
		public override bool Equals(object o)
		{
			var otherPitchGrabObject = (PitchGrabObject)o;

			if (otherPitchGrabObject == null)
				return false;

			return _id == otherPitchGrabObject._id;
		}

		public override int GetHashCode()
		{
			return _id ^ _id;
		}
	}
}