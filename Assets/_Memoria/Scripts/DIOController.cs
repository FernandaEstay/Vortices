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
		public VisualizationController visualizationController;

		public DIOManager DioManager
		{
			get { return visualizationController.dioManager; }
		}

		public void Initialize(VisualizationController assignedVisualizationController, int id)
		{
			visualizationController = assignedVisualizationController;

			pitchGrabObject.Initialize(this, id);

			inSpherePosition = true;
		}
	}
}