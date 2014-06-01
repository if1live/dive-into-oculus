#define CUSTOM_LAYOUT

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(DIOCameraController))]

public class CameraEditorWindow : Editor {
	// target component
	private DIOCameraController component;

	void OnEnable() {
		component = (DIOCameraController)target;
	}

	void OnDestroy() {
	}

	public override void OnInspectorGUI() {
		GUI.color = Color.white;

		Undo.RecordObject(component, "DIOCameraController");
		{
#if CUSTOM_LAYOUT
			component.cameraMode = (CameraMode)EditorGUILayout.EnumPopup("Editor Camera Mode", component.cameraMode);
			//OVREditorGUIUtility.Separator();	
#else
			DrawDefaultInspector ();
#endif
		}
		if (GUI.changed) {
			EditorUtility.SetDirty(component);
		}
	}
}
