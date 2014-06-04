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
			component.editorCameraMode = (CameraMode)EditorGUILayout.EnumPopup("Unity Editor Camera", component.editorCameraMode);
			component.androidCameraMode = (CameraMode)EditorGUILayout.EnumPopup("Android Camera", component.androidCameraMode);
			component.iosCameraMode = (CameraMode)EditorGUILayout.EnumPopup("iOS Camera", component.iosCameraMode);
			component.desktopCameraMode = (CameraMode)EditorGUILayout.EnumPopup("Desktop Standalone Camera", component.desktopCameraMode);
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
