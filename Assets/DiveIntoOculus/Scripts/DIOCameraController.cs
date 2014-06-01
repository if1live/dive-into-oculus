using UnityEngine;
using System.Collections;


public enum CameraMode {
	SideBySide = 0,	//for durovis dive
	OculusRift = 1,	//with barrel distortion
}

public class DIOCameraController : MonoBehaviour {
	public CameraMode cameraMode = CameraMode.OculusRift;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		SetCameraMode(cameraMode);
#elif UNITY_ANDROID
		SetCameraMode(CameraMode.SideBySide);
#elif UNITY_IPHONE
		SetCameraMode(CameraMode.SideBySide);	
#elif UNITY_STANDALONE
		SetCameraMode(CameraMode.OculusRift);
#else
#error "unknown platform"
#endif
	}

	// Update is called once per frame
	void Update () {
	
	}

	void SetCameraMode(CameraMode mode) {
		GameObject diveCamera = transform.Find ("Dive_Camera").gameObject;
		GameObject oculusCamera = transform.Find ("OVRCameraController").gameObject;

		switch (mode) {
		case CameraMode.SideBySide:
			oculusCamera.SetActive(false);
			diveCamera.SetActive(true);
			break;

		case CameraMode.OculusRift:
			oculusCamera.SetActive(true);
			diveCamera.SetActive(false);
			break;
		}
	}
}
