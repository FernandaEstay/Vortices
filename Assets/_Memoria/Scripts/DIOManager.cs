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
		public bool useLeapMotion = true;
		public bool usePitchGrab = true;
		public bool useHapticGlove;
		public bool useKeyboard;
		public bool useMouse;
		public bool useJoystick;

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

		//Sphere Configuration
		public bool autoTuneSpheresOnPlay;
		public DIOController informationPrefab;
		public Text sphereCounter;
		public SphereController spherePrefab;
		public List<SphereController> sphereControllers;
		public LoadImagesController loadImageController;

		[HideInInspector]
		public DIOController pitchedDioController;
		[HideInInspector]
		public int actualSphere;
		[HideInInspector]
		public List<Tuple<float, float>> radiusAlphaSphereList;
		[HideInInspector]
		public LookPointer lookPointerInstance;
		[HideInInspector]
		public bool movingSphere;
		[HideInInspector]
		public Action initialSphereAction;
		[HideInInspector]
		public Action finalSphereAction;

		private const string Scope = "Config";

		#endregion

		#region Properties

		public bool IsAnyDioPitched
		{
			get
			{
				var fullDioList = sphereControllers.SelectMany(s => s.dioControllerList);
				return fullDioList.Any(dio => dio.pitchGrabObject.isPinched);
			}
		}

		public bool AreAllDioOnSphere
		{
			get
			{
				var fullDioList = sphereControllers.SelectMany(s => s.dioControllerList);
				return fullDioList.All(dio => dio.inSpherePosition);
			}
		}

		public bool InLastSphere
		{
			get { return actualSphere == sphereControllers.Count - 1; }
		}

		public bool InFirstSphere
		{
			get { return actualSphere == 0; }
		}

		#endregion

		#region UnityCallbacks

		public void Start()
		{
			SetVariables();

			var sphereTextureIndex = 0;
			var sphereIndex = 0;
			actualSphere = 0;
			radiusAlphaSphereList = new List<Tuple<float, float>> { Tuple.New(0.0f, 0.0f) };
			movingSphere = false;

			csvCreator = new CsvCreator(csvCreatorPath);

			var leapSpaceChildrens = leapMotionRig.leapSpace.transform.GetChildren();

			foreach (var leapSpacechildren in leapSpaceChildrens)
			{
				leapSpacechildren.gameObject.SetActive(useLeapMotion);
			}

			if (autoTuneSpheresOnPlay)
			{
				AutoTuneSpheres();
			}

			foreach (var sphereController in sphereControllers)
			{
                /*  this -> DiOManager            * sphereIndex -> id sphere          * tranform.position -> centro de la esfera                 */
				sphereController.InitializeDioControllers(this, sphereIndex, transform.position, sphereTextureIndex, true);
				radiusAlphaSphereList.Add(Tuple.New(sphereController.sphereRadius, sphereController.sphereAlpha));

				sphereTextureIndex += sphereController.elementsToDisplay;
				sphereIndex += 1;
			}

			if (lookPointerPrefab != null && !(useKeyboard && useMouse))
			{
				var lookPointerPosition = new Vector3(0.0f, 0.0f, radiusAlphaSphereList[1].First);
				lookPointerInstance = Instantiate(lookPointerPrefab, leapMotionRig.centerEyeAnchor, lookPointerPosition, Quaternion.identity);
				lookPointerInstance.transform.localScale = lookPointerScale;

				lookPointerInstance.Initialize(this);
			}

			rayCastingDetector.Initialize(this);

			buttonPanel.Initialize(this);

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
			sphereCounter.text = string.Format("{0}/{1}", actualSphere + 1, sphereControllers.Count);
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
				MoveSphereInside(1, initialSphereAction, finalSphereAction);
			}
			else if (Input.GetKeyDown(action4Key))
			{
				MoveSphereOutside(1, initialSphereAction, finalSphereAction);
			}
		}

		private void MouseInput()
		{
			if (loadingScene.loading)
				return;

			var wheelAxis = Input.GetAxis("Mouse ScrollWheel");

			if (wheelAxis < 0.0f)
				MoveSphereInside(1, initialSphereAction, finalSphereAction);
			else if (wheelAxis > 0.0f)
				MoveSphereOutside(1, initialSphereAction, finalSphereAction);
		}

		private void JoystickInput()
		{
			if (loadingScene.loading)
				return;

			var moveSphereInsideAxis = Input.GetAxis("Action3");
			var moveSphereOutsideAxis = Input.GetAxis("Action4");

			MoveSphereInside(moveSphereInsideAxis, initialSphereAction, finalSphereAction);
			MoveSphereOutside(moveSphereOutsideAxis, initialSphereAction, finalSphereAction);
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
			var sphereTransform = sphereControllers[actualSphere].transform;

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

			var sphereTransform = sphereControllers[actualSphere].transform;

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
			csvCreator.AddLines("Changing Sphere", (actualSphere + 2).ToString());

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
					var targetRadius = radiusAlphaSphereList[i].First;
					sphereController.sphereRadius += actualRadiusFactor * radiusSpeed;

					if (TargetReached(actualRadiusFactor, sphereController.sphereRadius, targetRadius))
					{
						radiusTargetReached = true;
						sphereController.sphereRadius = targetRadius;
					}

					//Alpha
					var actualAlphaFactor = i == 0 ? alphaFactor * -1 : alphaFactor;
					var targetAlpha = radiusAlphaSphereList[i].Second;
					sphereController.sphereAlpha += actualAlphaFactor * alphaSpeed;

					if (TargetReached(actualAlphaFactor, sphereController.sphereAlpha, targetAlpha))
					{
						alphaTargerReached = true;
						sphereController.sphereAlpha = targetAlpha;
					}

					sphereController.ChangeVisualizationConfiguration(transform.position, sphereController.sphereRadius,
						sphereController.sphereAlpha);
					radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
				}

				if (radiusAlphaTargetReached.All(t => t.First && t.Second))
					break;

				yield return new WaitForFixedUpdate();
			}

			sphereControllers[actualSphere].notInZero = false;
			sphereControllers[actualSphere].gameObject.SetActive(false);
			actualSphere++;

			if (finalAction != null)
				finalAction();

			movingSphere = false;
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

			sphereControllers[actualSphere - 1].gameObject.SetActive(true);
			csvCreator.AddLines("Changing Sphere", actualSphere.ToString());

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
					var targetRadius = radiusAlphaSphereList[i + 1].First;
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
						var targetAlpha = radiusAlphaSphereList[i + 1].Second;
						sphereController.sphereAlpha += actualAlphaFactor * alphaSpeed;

						if (TargetReached(actualAlphaFactor, sphereController.sphereAlpha, targetAlpha))
						{
							alphaTargerReached = true;
							sphereController.sphereAlpha = targetAlpha;
						}
					}
					alphaWaitTimeCounter += Time.fixedDeltaTime;

					sphereController.ChangeVisualizationConfiguration(transform.position, sphereController.sphereRadius, sphereController.sphereAlpha);
					radiusAlphaTargetReached[i] = Tuple.New(radiusTargetReached, alphaTargerReached);
				}

				if (radiusAlphaTargetReached.All(t => t.First && t.Second))
					break;

				yield return new WaitForFixedUpdate();
			}

			actualSphere--;
			sphereControllers[actualSphere].notInZero = true;

			if (finalAction != null)
				finalAction();

			movingSphere = false;
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
			int sphereToShow = loadImageController.images / 39; //cantidad de esferas  en SetVariables a images se le asigna el valor GLPlayerPrefs.GetString(Scope, "Images");
            int extraImages = loadImageController.images % 39;

			if (extraImages != 0) // si sobran imagenes que mostrar se agrega una esfera extra
				sphereToShow++;
			else
				extraImages = 39;

			if (sphereControllers != null)          //List<SphereController> != null
			{
				foreach (var sphereController in sphereControllers)
				{
					DestroyImmediate(sphereController.gameObject);      //se destruyen las esferas
				}
			}

			sphereControllers = new List<SphereController>();

			for (int i = 0; i < sphereToShow; i++)
			{
				var newAlpha = 0.7f - 0.3f * i;     //representa en canal del color en formato RGBA (Red, Green Blue, Alpha) el cuál determina la transparencia. En ese código se determina cuanto alpha es adecuado según lo lejos que estén del centro.
                var newRadius = 0.45f + 0.15f * i;  //radio de la esfera

				if (newAlpha < 0.0f)
					newAlpha = 0.0f;

				var newSphere = IdealSphereConfiguration(
					i == sphereToShow - 1 ? extraImages : 39,       //si corresponde a la ultima esfera se asigna el valor de extraImages, si no el valor 39
					newRadius,
					newAlpha);

				sphereControllers.Add(newSphere);
			}
		}

		private SphereController IdealSphereConfiguration(int elements, float radius, float alpha)
		{
			var rows = elements / 13;           //filas es 39/13 = 3

			if (elements % 13 != 0)
				rows++;

            //SphereController spherePrefab -> objeto
            //Vector.zero -> posición
            //Quaternion.Identity corresponde a "no rotation" - el objeto está perfectamente alineado con el mundo o los ejes de los padres -> rotación
            var sphereController = Instantiate(spherePrefab, Vector3.zero, Quaternion.identity);


			sphereController.transform.parent = transform;      //hace que transform (DIOmanager) sea el padre de sphereController (esfera actual)

			sphereController.transform.ResetLocal();
            sphereController.elementsToDisplay = elements;

			sphereController.sphereRows = rows;

			sphereController.rowHightDistance = 0.2f;       //distancia que hay entre cada fila de la esfera

            sphereController.rowRadiusDifference = 0.05f;   //diferencia de radio del centro, esta diferencia entre las filas le da la forma de esfera.
            sphereController.scaleFactor = new Vector3(0.2f, 0.2f, 0.001f);     //multiplicador para cambiar el tamaño de cada elemento, asegurando que todos se ven iguales.
            sphereController.sphereRadius = radius;
			sphereController.sphereAlpha = alpha;
			sphereController.autoAngleDistance = true;


			sphereController.showDebugGizmo = false;

			return sphereController;
		}

		#endregion
	}
}