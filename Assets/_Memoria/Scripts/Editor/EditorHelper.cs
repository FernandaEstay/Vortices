using UnityEditor;

namespace Memoria.Editor
{
	public static class EditorHelper
	{
		public static void ShowScriptField(SerializedObject serializedObject)
		{
			SerializedProperty prop = serializedObject.FindProperty("m_Script");
			EditorGUILayout.PropertyField(prop, true);
		}

		public static void AddLabel(string title, bool bold)
		{
			EditorGUILayout.LabelField(title, bold ? EditorStyles.boldLabel : EditorStyles.label);
		}
	}
}