using UnityEngine;
using System.Collections;


public enum CameraMode {
	SideBySide = 0,	//for durovis dive
	OculusRift = 1,	//with barrel distortion
	MeshBarrelDistortion,
}

public class DIOCameraController : MonoBehaviour {
	public CameraMode editorCameraMode = CameraMode.OculusRift;
	public CameraMode androidCameraMode = CameraMode.SideBySide;
	public CameraMode iosCameraMode = CameraMode.SideBySide;
	public CameraMode desktopCameraMode = CameraMode.OculusRift;


	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		SetCameraMode(editorCameraMode);
#elif UNITY_ANDROID
		SetCameraMode(androidCameraMode);
#elif UNITY_IPHONE
		SetCameraMode(iosCameraMode);
#elif UNITY_STANDALONE
		SetCameraMode(desktopCameraMode);
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

		case CameraMode.MeshBarrelDistortion:
			oculusCamera.SetActive(false);
			diveCamera.SetActive(true);

			// TODO dive camera를 조작해서 mesh 기반 렌더링을 할수있도록 만든다
			break;

		default:
			break;
		}
	}
}
