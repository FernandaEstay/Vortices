using Gamelogic;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
	public class LookPointerRaycasting : GLMonoBehaviour, IOnValidate, IUpdate
	{
		public float maxDistance;
		public LayerMask ignoredLayerMask;
		public bool debugOutput;

		private DIOManager _dioManager;
		private RaycastHit _raycastHit;
		private Ray _ray;
		private Vector3 _forwardVector;
		private PitchGrabObject _actualPitchGrabObject;

		public void Initialize(DIOManager dioManager)
		{
			_dioManager = dioManager;
		}

		public void OnValidate()
		{
			maxDistance = Mathf.Max(0.0f, maxDistance);
		}

		public void Update()
		{
			_forwardVector = transform.TransformDirection(Vector3.forward);
			_ray = new Ray(transform.position, _forwardVector);

			if (Physics.Raycast(_ray, out _raycastHit, maxDistance, ignoredLayerMask))
			{
				var posiblePitcheGrabObject = _raycastHit.transform.gameObject.GetComponent<PitchGrabObject>();

				if (posiblePitcheGrabObject == null)
					return;

				if (posiblePitcheGrabObject.dioController.visualizationController.id != _dioManager.actualVisualization)
				{
                    if (_actualPitchGrabObject != null)
                        _actualPitchGrabObject.OnUnDetect();

                    return;
				}

				if (_actualPitchGrabObject == null)
				{
					_actualPitchGrabObject = posiblePitcheGrabObject;
				}
				else
				{
					if (_actualPitchGrabObject.idName != posiblePitcheGrabObject.idName)
					{
						_actualPitchGrabObject.OnUnDetect();
						_actualPitchGrabObject = posiblePitcheGrabObject;
					}
				}

				DebugLog(posiblePitcheGrabObject);

				_actualPitchGrabObject.OnDetected();
			}
			else
			{
				if (_actualPitchGrabObject == null)
				{
					_dioManager.buttonPanel.DisableZoomIn();
					return;
				}

				_actualPitchGrabObject.OnUnDetect();
			}
		}

		private void DebugLog(PitchGrabObject posiblePitcheGrabObject)
		{
			if (!debugOutput)
				return;

			print("Tag: " + _raycastHit.collider.tag);
			print(string.Format("ID Name: {0}, Sphere ID: {1}",posiblePitcheGrabObject.idName, posiblePitcheGrabObject.dioController.visualizationController.id));
			print("Actual Sphere: " + posiblePitcheGrabObject.dioController.DioManager.actualVisualization);
		}
	}
}