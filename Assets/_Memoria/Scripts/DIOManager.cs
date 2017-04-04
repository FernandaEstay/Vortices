using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityCallbacks;
using Gamelogic;
using Leap.Unity;
using Memoria.Core;
using UnityEngine.UI;

namespace Memoria
{
	public enum SpherePresentation
	{
		Layered,
		Grouped,
	}

	public class DIOManager : GLMonoBehaviour, IStart, IFixedUpdate, IUpdate, IOnValidate
	{
		#region Fields

		//General Configuration
		public LoadingScene loadingScene;
		public ButtonPanel buttonPanel;
        public PanelMouse panelMouse;
		public bool useLeapMotion = true;
		public bool usePitchGrab = true;
		public bool useHapticGlove;
		public bool useKeyboard;
		public bool useMouse;
        public bool visualizationPlane = true;
		public bool useJoystick;
        public bool mouseInput;
        public bool kinectInput;

		//DataOutput Configuration
		[HideInInspector]
		public CsvCreator csvCreator;
		public string csvCreatorPath;

		//Oculus Rift Configuration
		public LookPointerRaycasting rayCastingDetector;
		public LookPointer lookPointerPrefab;
		public Vector3 lookPointerScale = Vector3.one;
		public float closeRange = 2.0f;

		//LeapMotion Configuration
		public LeapHeadMountedRig leapMotionRig;
		public PinchDetector pinchDetectorLeft;
		public PinchDetector pinchDetectorRight;

		//OpenGlove Haptic Configuration
		public UnityOpenGlove unityOpenGlove;

		//Input Configuration
		public float horizontalSpeed;
		public float verticalSpeed;
		public float radiusFactor = 1.0f;
		public float radiusSpeed = 1.0f;
		public float alphaFactor = 1.0f;
		public float alphaSpeed = 1.0f;
		public float alphaWaitTime = 0.8f;
		public KeyCode action1Key;
		public KeyCode action2Key;
		public KeyCode action3Key;
		public KeyCode action4Key;
		public KeyCode action5Key;

        //Visualization configuration
		public bool autoTuneVisualizationOnPlay;
        public LoadImagesController loadImageController;
        public Text visualizationCounter;

        //Sphere Configuration
        public DIOController informationPrefab;
		public SphereController spherePrefab;
		public List<SphereController> sphereControllers;

        //Plane Configuration
        public PlaneController planePrefab;
        public DIOController informationPlanePrefab;
        public List<PlaneController> planeControllers;


		[HideInInspector]
		public DIOController pitchedDioController;
		[HideInInspector]
		public int actualVisualization;
		[HideInInspector]
		public List<Tuple<float, float>> radiusAlphaVisualizationList;
		[HideInInspector]
		public LookPointer lookPointerInstance;
		[HideInInspector]
		public bool movingSphere;
		[HideInInspector]
		public Action initialSphereAction;
		[HideInInspector]
		public Action finalSphereAction;

        [HideInInspector]
        public bool movingPlane;
        [HideInInspector]
        public Action initialPlaneAction;
        [HideInInspector]
        public Action finalPlaneAction;

        private const string Scope = "Config";

		#endregion

		#region Properties

		public bool IsAnyDioPitched
		{
			get
			{
                var fullDioList = visualizationPlane ? sphereControllers.SelectMany(s => s.dioControllerList) : planeControllers.SelectMany(s => s.dioControllerList);
                return fullDioList.Any(dio => dio.pitchGrabObject.isPinched);
			}
		}

		public bool AreAllDioOnSphere
		{
			get
			{
                var fullDioList = visualizationPlane ? sphereControllers.SelectMany(s => s.dioControllerList) : planeControllers.SelectMany(s => s.dioControllerList);
                return fullDioList.All(dio => dio.inSpherePosition);
			}
		}

		public bool InLastVisualization
		{
			get { return actualVisualization == (sphereControllers.Count > planeControllers.Count ? sphereControllers.Count - 1 : planeControllers.Count - 1); }
		}

		public bool InFirstVisualization
		{
			get { return actualVisualization == 0; }
		}

		#endregion

		#region UnityCallbacks

		public void Start()
		{
			SetVariables(); 
			var visualizationTextureIndex = 0;
			var visualizationIndex = 0;
			actualVisualization = 0;
			radiusAlphaVisualizationList = new List<Tuple<float, float>> { Tuple.New(0.0f, 0.0f) };

			movingSphere = false;
            movingPlane = false;

			csvCreator = new CsvCreator(csvCreatorPath);

			var leapSpaceChildrens = leapMotionRig.leapSpace.transform.GetChildren();

			foreach (var leapSpacechildren in leapSpaceChildrens)
			{
				leapSpacechildren.gameObject.SetActive(useLeapMotion);
			}

			if (autoTuneVisualizationOnPlay)
			{
                if (visualizationPlane)
                    AutoTunePlanes();
                else
                    AutoTuneSpheres();
                
			}
            if (visualizationPlane) 
            {
                foreach (var planeController in planeControllers)
                {
                    planeController.InitializeDioControllers(this, visualizationIndex, transform.position, visualizationTextureIndex, true);
                    radiusAlphaVisualizationList.Add(Tuple.New(planeController.distance, planeController.alpha));

                    visualizationTextureIndex += planeController.elementsToDisplay;
                    visualizationIndex += 1;
                }
            }
            else
            {
                foreach (var sphereController in sphereControllers)
                {
                    sphereController.InitializeDioControllers(this, visualizationIndex, transform.position, visualizationTextureIndex, true);
                    radiusAlphaVisualizationList.Add(Tuple.New(sphereController.sphereRadius, sphereController.alpha));

                    visualizationTextureIndex += sphereController.elementsToDisplay;
                    visualizationIndex += 1;
                }
            }

			if (lookPointerPrefab != null && !(useKeyboard && useMouse))
			{
				var lookPointerPosition = new Vector3(0.0f, 0.0f, radiusAlphaVisualizationList[1].First);
				lookPointerInstance = Instantiate(lookPointerPrefab, leapMotionRig.centerEyeAnchor, lookPointerPosition, Quaternion.identity);
				lookPointerInstance.transform.localScale = lookPointerScale;

				lookPointerInstance.Initialize(this);
			}

			rayCastingDetector.Initialize(this);

            if (mouseInput && !useKeyboard && !useJoystick)
            {
                panelMouse.Initialize(this);
                buttonPanel.gameObject.SetActive(false);
            }
            else
            {
                buttonPanel.Initialize(this);
                panelMouse.gameObject.SetActive(false);
            }

			//loadImageController.Initialize(this);
			loadingScene.Initialize(this);

			unityOpenGlove.Initialize(this);

			if (useLeapMotion)
			{
				buttonPanel.transform.parent = leapMotionRig.centerEyeAnchor.transform;

				var buttonPanelPosition = buttonPanel.transform.position;
				buttonPanelPosition.z = 0.4f;
				buttonPanel.transform.position = new Vector3(buttonPanelPosition.x, buttonPanelPosition.y, buttonPanelPosition.z);

				if (!usePitchGrab)
				{
					buttonPanel.zoomOut3DButton.gameObject.SetActive(false);
				}
			}
            if(!mouseInput)
			    buttonPanel.zoomIn3DButton.gameObject.SetActive(false);

			StartCoroutine(loadImageController.LoadFolderImages());

			initialSphereAction = () =>
			{
				buttonPanel.DisableZoomIn();
				buttonPanel.DisableZoomOut();
				buttonPanel.DisableAccept();
				buttonPanel.DisableMoveCameraInside();
				buttonPanel.DisableMoveCameraOutside();
			};

			finalSphereAction = () =>
			{
				buttonPanel.EnableMoveCameraInside();
				buttonPanel.EnableMoveCameraOutside();
			};

            if (mouseInput && !useKeyboard && !useJoystick)
            {
                initialPlaneAction = () =>
                {
                    panelMouse.DisableMoveCameraInside();
                    panelMouse.DisableMoveCameraOutside();
                };

                finalPlaneAction = () =>
                {
                    panelMouse.EnableMoveCameraInside();
                    panelMouse.EnableMoveCameraOutside();
                };
            }
            else
            {
                initialPlaneAction = () =>
                {
                    buttonPanel.DisableZoomIn();
                    buttonPanel.DisableZoomOut();
                    buttonPanel.DisableAccept();
                    buttonPanel.DisableMoveCameraInside();
                    buttonPanel.DisableMoveCameraOutside();
                };

                finalPlaneAction = () =>
                {
                    buttonPanel.EnableMoveCameraInside();
                    buttonPanel.EnableMoveCameraOutside();
                };
            }
        }

		private void SetVariables()
		{
			useLeapMotion = GLPlayerPrefs.GetBool(Scope, "UseLeapMotion");
			usePitchGrab = GLPlayerPrefs.GetBool(Scope, "UsePitchGrab");
			useHapticGlove = GLPlayerPrefs.GetBool(Scope, "UseHapticGlove");
			useJoystick = GLPlayerPrefs.GetBool(Scope, "UseJoystic");

			csvCreatorPath = GLPlayerPrefs.GetString(Scope, "DataOutput");

			unityOpenGlove.leftComDevice = GLPlayerPrefs.GetString(Scope, "LeftCom");
			unityOpenGlove.rightComDevice = GLPlayerPrefs.GetString(Scope, "RightCom");
			
			loadImageController.Initialize(this);
			loadImageController.images = Convert.ToInt32(GLPlayerPrefs.GetString(Scope, "Images"));
			loadImageController.LoadImageBehaviour.pathImageAssets = GLPlayerPrefs.GetString(Scope, "FolderImageAssetText");
			loadImageController.LoadImageBehaviour.pathSmall = GLPlayerPrefs.GetString(Scope, "FolderSmallText");
			loadImageController.LoadImageBehaviour.filename = GLPlayerPrefs.GetString(Scope, "FileName");

			OnValidate();
		}

		public void FixedUpdate()
		{
			//Fixed Camera Movement
			var cameraTransform = leapMotionRig.leapCamera.gameObject.transform;
			if (cameraTransform.localEulerAngles.x <= 310.0f && cameraTransform.localEulerAngles.x >= 50.0f)
			{
				var differenceToUpperLimit = 310.0f - cameraTransform.localEulerAngles.x;
				var differenceToLowerLimit = cameraTransform.localEulerAngles.x - 50.0f;

				cameraTransform.localEulerAngles =
					new Vector3(
						differenceToUpperLimit > differenceToLowerLimit ? 50.0f : 310.0f,
						cameraTransform.localEulerAngles.y,
						cameraTransform.localEulerAngles.z);
			}

			if (useKeyboard)
			{
				KeyboardInput();

				if (useMouse)
					MouseInput();
			}

			if (useJoystick)
				JoystickInput();
		}

		public void Update()
		{
            if(visualizationPlane)
			    visualizationCounter.text = string.Format("{0}/{1}", actualVisualization + 1, planeControllers.Count);
            else
                visualizationCounter.text = string.Format("{0}/{1}", actualVisualization + 1, sphereControllers.Count);
        }

		public void OnValidate()
		{
			if (!useLeapMotion)
			{
				usePitchGrab = false;
				useHapticGlove = false;
			}

			if (!useKeyboard)
			{
				useMouse = false;
			}

			horizontalSpeed = Mathf.Max(0.0f, horizontalSpeed);
			verticalSpeed = Mathf.Max(0.0f, verticalSpeed);

			lookPointerScale = new Vector3(
				Mathf.Max(0.0f, lookPointerScale.x),
				Mathf.Max(0.0f, lookPointerScale.y),
				Mathf.Max(0.0f, lookPointerScale.z));

			closeRange = Mathf.Max(0.1f, closeRange);
		}

		#endregion

		#region Inputs

		private void KeyboardInput()
		{
			if (loadingScene.loading)
				return;

			if (Input.GetKeyDown(action3Key))
			{
                if(visualizationPlane)
                    MovePlaneInside(1, initialPlaneAction, finalPlaneAction);
                else
                    MoveSphereInside(1, initialSphereAction, finalSphereAction);
			}
			else if (Input.GetKeyDown(action4Key))
			{
                if (visualizationPlane)
                    MovePlaneOutside(1, initialPlaneAction, finalPlaneAction);
                else
				    MoveSphereOutside(1, initialSphereAction, finalSphereAction);
			}
		}

		private void MouseInput()
		{
			if (loadingScene.loading)
				return;

			var wheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (wheelAxis < 0.0f)
            {
                if (visualizationPlane)
                    MovePlaneInside(1, initialPlaneAction, finalPlaneAction);
                else
                    MoveSphereInside(1, initialSphereAction, finalSphereAction);
            }
            else if (wheelAxis > 0.0f)
            {
                if (visualizationPlane)
                    MovePlaneOutside(1, initialPlaneAction, finalPlaneAction);
                else
                    MoveSphereOutside(1, initialSphereAction, finalSphereAction);
            }
		}

		private void JoystickInput()
		{
			if (loadingScene.loading)
				return;

			var moveSphereInsideAxis = Input.GetAxis("Action3");
			var moveSphereOutsideAxis = Input.GetAxis("Action4");

            var movePlaneInsideAxis = Input.GetAxis("Action3");
            var movePlaneOutsideAxis = Input.GetAxis("Action4");

            if (visualizationPlane)
            {
                MovePlaneInside(movePlaneInsideAxis, initialPlaneAction, finalPlaneAction);
                MovePlaneOutside(movePlaneOutsideAxis, initialPlaneAction, finalPlaneAction);
            }
            else
            {
                MoveSphereInside(moveSphereInsideAxis, initialSphereAction, finalSphereAction);
                MoveSphereOutside(moveSphereOutsideAxis, initialSphereAction, finalSphereAction);
            }
            
		}

		#endregion

		#region Camera Methods

		public void MoveCameraHorizontal(float horizontalAxis)
		{
			var cameraTransform = leapMotionRig.leapCamera.gameObject.transform;

			cameraTransform.Rotate(Vector3.up * horizontalSpeed * horizontalAxis, Space.World);
		}

		public void MoveCameraVertical(float verticalAxis)
		{
			var cameraTransform = leapMotionRig.leapCamera.gameObject.transform;

			cameraTransform.Rotate(Vector3.left * verticalSpeed * verticalAxis, Space.Self);
		}

		#endregion

		#region Sphere Methods

		public void MoveSphereHorizontal(float horizontalAxis)
		{
			var sphereTransform = sphereControllers[actualVisualization].transform;

			sphereTransform.Rotate(Vector3.down * horizontalSpeed * horizontalAxis, Space.Self);
		}

		private int _sphereVerticalCounter = 50;
		public void MoveSphereVertical(float verticalAxis)
		{
            if (verticalAxis == 1.0f && _sphereVerticalCounter >= 100)
			{
				_sphereVerticalCounter = 100;
				return;
			}

			if (verticalAxis == -1.0f && _sphereVerticalCounter <= 0)
			{
				_sphereVerticalCounter = 0;
				return;
			}

			var sphereTransform = sphereControllers[actualVisualization].transform;

			sphereTransform.Rotate(Vector3.right * verticalSpeed * verticalAxis, Space.World);

			_sphereVerticalCounter += (int)verticalAxis;
		}

		public void MoveSphereInside(float insideAxis, Action initialAction, Action finalAction)
		{
            if (insideAxis == 1.0f && !movingSphere && lookPointerInstance.actualPitchGrabObject == null &&
				!lookPointerInstance.zoomingIn && !lookPointerInstance.zoomingOut && AreAllDioOnSphere)
			{
				StartCoroutine(MoveSphereInside(initialAction, finalAction));
			}
			else
			{
				if (finalAction != null)
					finalAction();
			}
		}
        public void MovePlaneInside(float insideAxis, Action initialAction, Action finalAction)
        {
            if (insideAxis == 1.0f && !movingPlane && lookPointerInstance.actualPitchGrabObject == null &&
                !lookPointerInstance.zoomingIn && !lookPointerInstance.zoomingOut && AreAllDioOnSphere)
            {
                StartCoroutine(MovePlaneInside(initialAction, finalAction));
            }
            else
            {
                if (finalAction != null)
                    finalAction();
            }
        }
        public void MoveSphereOutside(float outsideAxis, Action initialAction, Action finalAction)
		{
            if (outsideAxis == 1.0f && !movingSphere && lookPointerInstance.actualPitchGrabObject == null &&
				!lookPointerInstance.zoomingIn && !lookPointerInstance.zoomingOut && AreAllDioOnSphere)
			{
				StartCoroutine(MoveSphereOutside(initialAction, finalAction));
			}
			else
			{
				if (finalAction != null)
					finalAction();
			}
		}
        public void MovePlaneOutside(float outsideAxis, Action initialAction, Action finalAction)
        {
            if (outsideAxis == 1.0f && !movingPlane && lookPointerInstance.actualPitchGrabObject == null &&
                !lookPointerInstance.zoomingIn && !lookPointerInstance.zoomingOut && AreAllDioOnSphere)
            {
                StartCoroutine(MovePlaneOutside(initialAction, finalAction));
            }
            else
            {
                if (finalAction != null)
                    finalAction();
            }
        }

        private IEnumerator MoveSphereInside(Action initialAction, Action finalAction)
		{
			if (movingSphere)
				yield break;

			movingSphere = true;

                var notInZeroSphereControllers =
                sphereControllers.Where(
                    sphereController =>
                        sphereController.notInZero
                    ).ToList();
            


			if (notInZeroSphereControllers.Count == 1)
			{
				movingSphere = false;

				yield break;
			}

			var radiusAlphaTargetReached = new List<Tuple<bool, bool>>();
			for (int i = 0; i < notInZeroSphereControllers.Count; i++)
			{
				radiusAlphaTargetReached.Add(Tuple.New(false, false));
			}

			var actualRadiusFactor = radiusFactor * -1;
			csvCreator.AddLines("Changing Sphere", (actualVisualization + 2).ToString());

			if (initialAction != null)
				initialAction();

			while (true)
			{
				for (int i = 0; i < notInZeroSphereControllers.Count; i++)
				{
					var sphereController = notInZeroSphereControllers[i];
					var radiusTargetReached = false;
					var alphaTargerReached = false;

					//Radius
					var targetRadius = radiusAlphaVisualizationList[i].First;
					sphereController.sphereRadius += actualRadiusFactor * radiusSpeed;

					if (TargetReached(actualRadiusFactor, sphereController.sphereRadius, targetRadius))
					{
						radiusTargetReached = true;
						sphereController.sphereRadius = targetRadius;
					}

					//Alpha
					var actualAlphaFactor = i == 0 ? alphaFactor * -1 : alphaFactor;
					var targetAlpha = radiusAlphaVisualizationList[i].Second;
					sphereController.alpha += actualAlphaFactor * alphaSpeed;

					if (TargetReached(actualAlphaFactor, sphereController.alpha, targetAlpha))
					{
						alphaTargerReached = true;
						sphereController.alpha = targetAlpha;
					}

					sphereController.ChangeVisualizationConfiguration(transform.position, sphereController.sphereRadius,
						sphereController.alpha);
					radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
				}

				if (radiusAlphaTargetReached.All(t => t.First && t.Second))
					break;

				yield return new WaitForFixedUpdate();
			}

			sphereControllers[actualVisualization].notInZero = false;
			sphereControllers[actualVisualization].gameObject.SetActive(false);
			actualVisualization++;

			if (finalAction != null)
				finalAction();

			movingSphere = false;
		}
        private IEnumerator MovePlaneInside(Action initialAction, Action finalAction)
        {
            if (movingPlane)
                yield break;

            movingPlane = true;

            var notInZeroPlaneControllers =
            planeControllers.Where(
                planeController =>
                    planeController.notInZero
                ).ToList();

            if (notInZeroPlaneControllers.Count == 1)
            {
                movingPlane = false;

                yield break;
            }

            var radiusAlphaTargetReached = new List<Tuple<bool, bool>>();
            for (int i = 0; i < notInZeroPlaneControllers.Count; i++)
            {
                radiusAlphaTargetReached.Add(Tuple.New(false, false));
            }

            var actualRadiusFactor = radiusFactor * -1;
            csvCreator.AddLines("Changing Plane", (actualVisualization + 2).ToString());

            if (initialAction != null)
                initialAction();

            while (true)
            {
                for (int i = 0; i < notInZeroPlaneControllers.Count; i++)
                {
                    var planeController = notInZeroPlaneControllers[i];
                    var radiusTargetReached = false;
                    var alphaTargerReached = false;

                    //Radius
                    var targetRadius = radiusAlphaVisualizationList[i].First;
                    planeController.distance += actualRadiusFactor * radiusSpeed;

                    if (TargetReached(actualRadiusFactor, planeController.distance, targetRadius))
                    {
                        radiusTargetReached = true;
                        planeController.distance = targetRadius;
                    }

                    //Alpha
                    var actualAlphaFactor = i == 0 ? alphaFactor * -1 : alphaFactor;
                    var targetAlpha = radiusAlphaVisualizationList[i].Second;
                    planeController.alpha += actualAlphaFactor * alphaSpeed;

                    if (TargetReached(actualAlphaFactor, planeController.alpha, targetAlpha))
                    {
                        alphaTargerReached = true;
                        planeController.alpha = targetAlpha;
                    }

                    planeController.ChangeVisualizationConfiguration(transform.position, planeController.distance,
                        planeController.alpha);
                    radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
                }

                if (radiusAlphaTargetReached.All(t => t.First && t.Second))
                    break;

                yield return new WaitForFixedUpdate();
            }

            planeControllers[actualVisualization].notInZero = false;
            planeControllers[actualVisualization].gameObject.SetActive(false);
            actualVisualization++;

            if (finalAction != null)
                finalAction();

            movingPlane = false;
        }
        private IEnumerator MoveSphereOutside(Action initialAction, Action finalAction)
		{
			if (movingSphere)
				yield break;

			movingSphere = true;

			var notInZeroSphereControllers =
				sphereControllers.Where(
					sphereController =>
						sphereController.notInZero
					).ToList();

			var inZeroSphereControllers =
				sphereControllers.Where(
					sphereController =>
						!sphereController.notInZero
					).ToList();

			if (inZeroSphereControllers.Count == 0)
			{
				movingSphere = false;

				yield break;
			}

			var sphereControllerList = new List<SphereController> { inZeroSphereControllers.Last() };
			sphereControllerList.AddRange(notInZeroSphereControllers);

			var radiusAlphaTargetReached = new List<Tuple<bool, bool>>();
			for (int i = 0; i < sphereControllerList.Count; i++)
			{
				radiusAlphaTargetReached.Add(Tuple.New(false, false));
			}

			sphereControllers[actualVisualization - 1].gameObject.SetActive(true);
			csvCreator.AddLines("Changing Sphere", actualVisualization.ToString());

			if (initialAction != null)
				initialAction();

			var alphaWaitTimeCounter = 0.0f;
			while (true)
			{
				for (int i = 0; i < sphereControllerList.Count; i++)
				{
					var sphereController = sphereControllerList[i];
					var radiusTargetReached = false;
					var alphaTargerReached = false;

					//Radius
					var targetRadius = radiusAlphaVisualizationList[i + 1].First;
					sphereController.sphereRadius += radiusFactor * radiusSpeed;

					if (TargetReached(radiusFactor, sphereController.sphereRadius, targetRadius))
					{
						radiusTargetReached = true;
						sphereController.sphereRadius = targetRadius;
					}

					if (alphaWaitTimeCounter >= alphaWaitTime)
					{
						//Alpha
						var actualAlphaFactor = i == 0
							? alphaFactor
							: alphaFactor * -1;
						var targetAlpha = radiusAlphaVisualizationList[i + 1].Second;
						sphereController.alpha += actualAlphaFactor * alphaSpeed;

						if (TargetReached(actualAlphaFactor, sphereController.alpha, targetAlpha))
						{
							alphaTargerReached = true;
							sphereController.alpha = targetAlpha;
						}
					}
					alphaWaitTimeCounter += Time.fixedDeltaTime;

					sphereController.ChangeVisualizationConfiguration(transform.position, sphereController.sphereRadius, sphereController.alpha);
					radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
				}

				if (radiusAlphaTargetReached.All(t => t.First && t.Second))
					break;

				yield return new WaitForFixedUpdate();
			}

			actualVisualization--;
			sphereControllers[actualVisualization].notInZero = true;

			if (finalAction != null)
				finalAction();

			movingSphere = false;
		}

        private IEnumerator MovePlaneOutside(Action initialAction, Action finalAction)
        {
            if (movingPlane)
                yield break;

            movingPlane = true;

            var notInZeroPlaneControllers =
                planeControllers.Where(
                    planeController =>
                        planeController.notInZero
                    ).ToList();

            var inZeroPlaneControllers =
                planeControllers.Where(
                    planeController =>
                        !planeController.notInZero
                    ).ToList();

            if (inZeroPlaneControllers.Count == 0)
            {
                movingPlane = false;

                yield break;
            }

            var planeControllerList = new List<PlaneController> { inZeroPlaneControllers.Last() };
            planeControllerList.AddRange(notInZeroPlaneControllers);

            var radiusAlphaTargetReached = new List<Tuple<bool, bool>>();
            for (int i = 0; i < planeControllerList.Count; i++)
            {
                radiusAlphaTargetReached.Add(Tuple.New(false, false));
            }

            planeControllers[actualVisualization - 1].gameObject.SetActive(true);
            csvCreator.AddLines("Changing Plane", actualVisualization.ToString());

            if (initialAction != null)
                initialAction();

            var alphaWaitTimeCounter = 0.0f;
            while (true)
            {
                for (int i = 0; i < planeControllerList.Count; i++)
                {
                    var planeController = planeControllerList[i];
                    var radiusTargetReached = false;
                    var alphaTargerReached = false;

                    //Radius
                    var targetRadius = radiusAlphaVisualizationList[i + 1].First;
                    planeController.distance += radiusFactor * radiusSpeed;

                    if (TargetReached(radiusFactor, planeController.distance, targetRadius))
                    {
                        radiusTargetReached = true;
                        planeController.distance = targetRadius;
                    }

                    if (alphaWaitTimeCounter >= alphaWaitTime)
                    {
                        //Alpha
                        var actualAlphaFactor = i == 0
                            ? alphaFactor
                            : alphaFactor * -1;
                        var targetAlpha = radiusAlphaVisualizationList[i + 1].Second;
                        planeController.alpha += actualAlphaFactor * alphaSpeed;

                        if (TargetReached(actualAlphaFactor, planeController.alpha, targetAlpha))
                        {
                            alphaTargerReached = true;
                            planeController.alpha = targetAlpha;
                        }
                    }
                    alphaWaitTimeCounter += Time.fixedDeltaTime;

                    planeController.ChangeVisualizationConfiguration(transform.position, planeController.distance, planeController.alpha);
                    radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
                }

                if (radiusAlphaTargetReached.All(t => t.First && t.Second))
                    break;

                yield return new WaitForFixedUpdate();
            }

            actualVisualization--;
            planeControllers[actualVisualization].notInZero = true;

            if (finalAction != null)
                finalAction();

            movingPlane = false;
        }

        private bool TargetReached(float factor, float value, float target)
		{
            if (factor >= 0)
			{
				if (value >= target)
				{
					return true;
				}
			}
			else
			{
				if (value <= target)
				{
					return true;
				}
			}

			return false;
		}

		public void AutoTuneSpheres()
		{
			int sphereToShow = loadImageController.images / 39;
			int extraImages = loadImageController.images % 39;

			if (extraImages != 0)
				sphereToShow++;
			else
				extraImages = 39;

			if (sphereControllers != null)
			{
				foreach (var sphereController in sphereControllers)
				{
					DestroyImmediate(sphereController.gameObject);
				}
			}

			sphereControllers = new List<SphereController>();

			for (int i = 0; i < sphereToShow; i++)
			{
				var newAlpha = 0.7f - 0.3f * i;
				var newRadius = 0.45f + 0.15f * i;

				if (newAlpha < 0.0f)
					newAlpha = 0.0f;

				var newSphere = IdealSphereConfiguration(
					i == sphereToShow - 1 ? extraImages : 39,
					newRadius,
					newAlpha);

				sphereControllers.Add(newSphere);
			}
		}

		private SphereController IdealSphereConfiguration(int elements, float radius, float alpha)
		{
			var rows = elements / 13;

			if (elements % 13 != 0)
				rows++;

			var sphereController = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);
			sphereController.transform.parent = transform;
			sphereController.transform.ResetLocal();

			sphereController.elementsToDisplay = elements;
			sphereController.visualizationRow = rows;
			sphereController.rowHightDistance = 0.2f;
			sphereController.rowRadiusDifference = 0.05f;
			sphereController.scaleFactor = new Vector3(0.2f, 0.2f, 0.001f);
			sphereController.sphereRadius = radius;
			sphereController.alpha = alpha;
			sphereController.autoAngleDistance = true;
			sphereController.debugGizmo = false;

			return sphereController;
		}

        #endregion

        #region Plane

        private void AutoTunePlanes()
        {
            int planesToShow = loadImageController.images / 12;
            int extraImages = loadImageController.images % 12;

            if (extraImages != 0)
                planesToShow++;
            else
                extraImages = 12;

            if (planeControllers != null)
            {
                foreach (var planeController in planeControllers)
                {
                    DestroyImmediate(planeController.gameObject);
                }
            }

            planeControllers = new List<PlaneController>();

            for (int i = 0; i < planesToShow; i++)
            {
                var newAlpha = 0.7f - 0.3f * i;
                var newRadius = 0.45f + 0.15f * i * 4;

                if (newAlpha < 0.0f)
                    newAlpha = 0.0f;

                var newPlane = IdealPlaneConfiguration(
                    i == planesToShow - 1 ? extraImages : 12,
                    newRadius,
                    newAlpha);

                planeControllers.Add(newPlane);
            }
        }
        private PlaneController IdealPlaneConfiguration(int elements, float distance, float alpha)
        {

            var rows = elements / 4;

            if (elements % 4 != 0)
                rows++;

            var planeController = Instantiate(planePrefab, Vector3.zero, Quaternion.identity);
            planeController.transform.parent = transform;      //hace que transform (DIOmanager) sea el padre de visualizationController (esfera actual)

            planeController.transform.ResetLocal();
            planeController.elementsToDisplay = elements;

            planeController.visualizationRow = rows;

            planeController.rowHightDistance = 0.4f;
            planeController.rowDistanceDifference = 0f;

            planeController.scaleFactor = new Vector3(0.35f, 0.3f, 0.001f);     //multiplicador para cambiar el tamaño de cada elemento, asegurando que todos se ven iguales.
            planeController.distance = distance;
            planeController.alpha = alpha;
            planeController.autoAngleDistance = true;


            planeController.debugGizmo = false;

            return planeController;
        }
        #endregion
    }
}