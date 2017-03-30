using Gamelogic;
using UnityEngine;

namespace Memoria
{
	public class DIOController : GLMonoBehaviour
	{
		public PitchGrabObject pitchGrabObject;

		[HideInInspector]
		public Vector3 originalDioPosition;
		[HideInInspector]
		public Vector3 originalAnchorPosition;
		[HideInInspector]
		public Quaternion originalDioRotation;

		[HideInInspector]
		public bool inSpherePosition;
		[HideInInspector]
		public SphereController sphereController;

		public DIOManager DioManager
		{
			get { return sphereController.dioManager; }
		}

		public void Initialize(SphereController assignedSphereController, int id)
		{
			sphereController = assignedSphereController;

			pitchGrabObject.Initialize(this, id);

			inSpherePosition = true;
		}
	}
}