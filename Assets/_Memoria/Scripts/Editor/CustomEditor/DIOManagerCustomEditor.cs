using System.Collections.Generic;
using Gamelogic;
using Gamelogic.Editor;
using Gamelogic.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Memoria.Editor
{
    [CustomEditor(typeof(DIOManager), true), CanEditMultipleObjects]
    public class DIOManagerCustomEditor : GLEditor<DIOManager>
    {
        private GLSerializedProperty _loadingScene;
        private GLSerializedProperty _buttonPanel;
        private GLSerializedProperty _panelMouse;
        private GLSerializedProperty _useLeapMotion;
        private GLSerializedProperty _usePitchGrab;
        private GLSerializedProperty _useHapticGlove;
        private GLSerializedProperty _useKeyboard;
        private GLSerializedProperty _useMouse;
        private GLSerializedProperty _useJoystick;
        private GLSerializedProperty _visualizationPlane;

        private GLSerializedProperty _mouseInput;
        private GLSerializedProperty _kinectInput;

        private GLSerializedProperty _csvCreatorPath;

        private GLSerializedProperty _rayCastingDetector;
        private GLSerializedProperty _lookPointerPrefab;
        private GLSerializedProperty _lookPointerScale;
        private GLSerializedProperty _closeRange;

        private GLSerializedProperty _leapMotionRig;
        private GLSerializedProperty _pinchDetectorLeft;
        private GLSerializedProperty _pinchDetectorRight;

        private GLSerializedProperty _unityOpenGlove;

        private GLSerializedProperty _horizontalSpeed;
        private GLSerializedProperty _verticalSpeed;
        private GLSerializedProperty _radiusFactor;
        private GLSerializedProperty _radiusSpeed;
        private GLSerializedProperty _alphaFactor;
        private GLSerializedProperty _alphaSpeed;
        private GLSerializedProperty _alphaWaitTime;
        private GLSerializedProperty _action1Key;
        private GLSerializedProperty _action2Key;
        private GLSerializedProperty _action3Key;
        private GLSerializedProperty _action4Key;
        private GLSerializedProperty _action5Key;

        private GLSerializedProperty _autoTuneVisualizationOnPlay;
        private GLSerializedProperty _visualizationCounter;
        private GLSerializedProperty _loadImageController;

        private GLSerializedProperty _informationPrefab;
        private GLSerializedProperty _spherePrefab;
        private GLSerializedProperty _sphereControllers;

        private GLSerializedProperty _planePrefab;
        private GLSerializedProperty _informationPlanePrefab;
        private GLSerializedProperty _planeControllers;

        public void OnEnable()
        {
            _loadingScene = FindProperty("loadingScene");
            _buttonPanel = FindProperty("buttonPanel");
            _panelMouse = FindProperty("panelMouse");
            _useLeapMotion = FindProperty("useLeapMotion");
            _usePitchGrab = FindProperty("usePitchGrab");
            _useHapticGlove = FindProperty("useHapticGlove");
            _useKeyboard = FindProperty("useKeyboard");
            _useMouse = FindProperty("useMouse");
            _useJoystick = FindProperty("useJoystick");
            _visualizationPlane = FindProperty("visualizationPlane");
            _mouseInput = FindProperty("mouseInput");
            _kinectInput = FindProperty("kinectInput");

            _csvCreatorPath = FindProperty("csvCreatorPath");

            _rayCastingDetector = FindProperty("rayCastingDetector");
            _lookPointerPrefab = FindProperty("lookPointerPrefab");
            _lookPointerScale = FindProperty("lookPointerScale");
            _closeRange = FindProperty("closeRange");

            _leapMotionRig = FindProperty("leapMotionRig");
            _pinchDetectorLeft = FindProperty("pinchDetectorLeft");
            _pinchDetectorRight = FindProperty("pinchDetectorRight");

            _unityOpenGlove = FindProperty("unityOpenGlove");

            _horizontalSpeed = FindProperty("horizontalSpeed");
            _verticalSpeed = FindProperty("verticalSpeed");
            _radiusFactor = FindProperty("radiusFactor");
            _radiusSpeed = FindProperty("radiusSpeed");
            _alphaFactor = FindProperty("alphaFactor");
            _alphaSpeed = FindProperty("alphaSpeed");
            _alphaWaitTime = FindProperty("alphaWaitTime");
            _action1Key = FindProperty("action1Key");
            _action2Key = FindProperty("action2Key");
            _action3Key = FindProperty("action3Key");
            _action4Key = FindProperty("action4Key");
            _action5Key = FindProperty("action5Key");

            _autoTuneVisualizationOnPlay = FindProperty("autoTuneVisualizationOnPlay");
            _informationPrefab = FindProperty("informationPrefab");
            _visualizationCounter = FindProperty("visualizationCounter");
            _spherePrefab = FindProperty("spherePrefab");
            _planePrefab = FindProperty("planePrefab");
            _informationPlanePrefab = FindProperty("informationPlanePrefab");
            _loadImageController = FindProperty("loadImageController");
            _sphereControllers = FindProperty("sphereControllers");
            _planeControllers = FindProperty("planeControllers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorHelper.ShowScriptField(serializedObject);

            Splitter();

            EditorHelper.AddLabel("General Configuration", true);
            AddField(_loadingScene);
            AddField(_buttonPanel);
            AddField(_panelMouse);
            AddField(_useLeapMotion);

            if (_useLeapMotion.boolValue)
            {
                EditorGUI.indentLevel += 1;
                AddField(_usePitchGrab);
                AddField(_useHapticGlove);
                EditorGUI.indentLevel -= 1;
            }

            AddField(_useKeyboard);

            /*
            if (_useKeyboard.boolValue)
            {
                EditorGUI.indentLevel += 1;
                AddField(_useMouse);
                EditorGUI.indentLevel -= 1;
            }
            */

            AddField(_useJoystick);
            AddField(_mouseInput);
            AddField(_kinectInput);

            Splitter();

            EditorHelper.AddLabel("Visualization Configuration", true);
            AddField(_visualizationPlane);
            AddField(_autoTuneVisualizationOnPlay);
            AddField(_visualizationCounter);
            AddField(_loadImageController);

            Splitter();

            EditorHelper.AddLabel("DataOutput Configuration", true);
            _csvCreatorPath.Label = "Path";
            AddField(_csvCreatorPath);

            Splitter();

            EditorHelper.AddLabel("Oculus Rift Configuration", true);
            AddField(_rayCastingDetector);
            AddField(_lookPointerPrefab);
            AddField(_lookPointerScale);
            AddField(_closeRange);

            Splitter();

            EditorHelper.AddLabel("Leap Motion Configuration", true);
            AddField(_leapMotionRig);
            AddField(_pinchDetectorLeft);
            AddField(_pinchDetectorRight);

            Splitter();

            EditorHelper.AddLabel("OpenGlove Haptic Configuration", true);
            AddField(_unityOpenGlove);

            Splitter();

            EditorHelper.AddLabel("Input Configuration", true);
            AddField(_horizontalSpeed);
            AddField(_verticalSpeed);
            AddField(_radiusFactor);
            AddField(_radiusSpeed);
            AddField(_alphaFactor);
            AddField(_alphaSpeed);
            AddField(_alphaWaitTime);
            if (_useKeyboard.boolValue)
            {
                EditorHelper.AddLabel("> Keyboard", true);
                AddField(_action1Key);
                AddField(_action2Key);
                AddField(_action3Key);
                AddField(_action4Key);
                AddField(_action5Key);
            }

            Splitter();
            EditorHelper.AddLabel("Plane Configuration", true);
            AddField(_informationPlanePrefab);
            Target.planePrefab = EditorGUILayout.ObjectField(new GUIContent("PlaneController Prefab"), _planePrefab.SerializedProperty.objectReferenceValue, typeof(PlaneController), false) as PlaneController;

            Splitter();

            EditorHelper.AddLabel("Sphere Configuration", true);
            AddField(_informationPrefab);

            Target.spherePrefab = EditorGUILayout.ObjectField(new GUIContent("SphereController Prefab"), _spherePrefab.SerializedProperty.objectReferenceValue, typeof(SphereController), false) as SphereController;

            RemoveAndAddSphereButtons();
            AutoTuneSphereButtons();

            for (int i = 0; i < _sphereControllers.SerializedProperty.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorHelper.AddLabel("Sphere " + (i + 1), true);
                AddSphereControllerField(_sphereControllers.SerializedProperty.GetArrayElementAtIndex(i));
                EditorGUILayout.EndHorizontal();
            }

            Splitter();

            serializedObject.ApplyModifiedProperties();
        }

        private void AddSphereControllerField(SerializedProperty sphereControllerSerializedProperty)
        {
            if (sphereControllerSerializedProperty.objectReferenceValue == null)
                return;

            var sphereControllerProperty = new SerializedObject(sphereControllerSerializedProperty.objectReferenceValue);

            sphereControllerProperty.Update();

            var elementsToDisplay = sphereControllerProperty.FindProperty("elementsToDisplay");
            var sphereRows = sphereControllerProperty.FindProperty("visualizationRow");
            var rowHightDistance = sphereControllerProperty.FindProperty("rowHightDistance");
            var rowRadiusDifference = sphereControllerProperty.FindProperty("rowRadiusDifference");
            var scaleFactor = sphereControllerProperty.FindProperty("scaleFactor");
            var sphereRadius = sphereControllerProperty.FindProperty("sphereRadius");
            var sphereAlpha = sphereControllerProperty.FindProperty("alpha");
            var autoAngleDistance = sphereControllerProperty.FindProperty("autoAngleDistance");
            var angleDistance = sphereControllerProperty.FindProperty("angleDistance");
            var showDebugGizmo = sphereControllerProperty.FindProperty("debugGizmo");
            var sphereDebugColor = sphereControllerProperty.FindProperty("debugColor");

            EditorGUILayout.PropertyField(elementsToDisplay);
            EditorGUILayout.PropertyField(sphereRows);

            if (sphereRows.intValue != 1)
            {
                EditorGUILayout.PropertyField(rowHightDistance);
                EditorGUILayout.PropertyField(rowRadiusDifference);
            }
            EditorGUILayout.PropertyField(scaleFactor);
            EditorGUILayout.PropertyField(sphereRadius);
            EditorGUILayout.PropertyField(sphereAlpha);
            EditorGUILayout.PropertyField(autoAngleDistance);

            if (!autoAngleDistance.boolValue)
                EditorGUILayout.PropertyField(angleDistance);

            EditorGUILayout.PropertyField(showDebugGizmo);

            if (showDebugGizmo.boolValue)
                EditorGUILayout.PropertyField(sphereDebugColor);

            sphereControllerProperty.ApplyModifiedProperties();
        }

        private void RemoveAndAddSphereButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
            {
                if (Target.sphereControllers.Count > 0)
                {
                    var lastIndex = Target.sphereControllers.Count - 1;
                    var sphereController = Target.sphereControllers[lastIndex];
                    DestroyImmediate(sphereController.gameObject);
                    Target.sphereControllers.RemoveAt(lastIndex);
                }
            }

            if (GUILayout.Button("+"))
            {
                if (Target.sphereControllers == null)
                    Target.sphereControllers = new List<SphereController>();

                var sphereController = Instantiate(_spherePrefab.objectReferenceValue, Vector3.zero, Quaternion.identity) as SphereController;
                sphereController.transform.parent = Target.transform;
                sphereController.transform.ResetLocal();

                Target.sphereControllers.Add(sphereController);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AutoTuneSphereButtons()
        {
            if (Target.sphereControllers.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Auto-Tune Spheres"))
                {
                    Target.AutoTuneSpheres();
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}