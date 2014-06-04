using UnityEngine;
using System.Collections;


public enum CameraMode
{
	SideBySide = 0,	//for durovis dive
	OculusRift = 1,	//with barrel distortion
	MeshBarrelDistortion,
}

public class DIOCameraController : MonoBehaviour
{
	public CameraMode editorCameraMode = CameraMode.OculusRift;
	public CameraMode androidCameraMode = CameraMode.SideBySide;
	public CameraMode iosCameraMode = CameraMode.SideBySide;
	public CameraMode desktopCameraMode = CameraMode.OculusRift;


	// Use this for initialization
	void Start ()
	{
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
	void Update () 
	{
	
	}

	void SetMeshBarrelDistortion(GameObject meshDistortionCamera)
	{
		// dive camera를 조작해서 mesh 기반 렌더링을 할수있도록 만든다
		RenderTexture leftRT = new RenderTexture(Screen.width/2, Screen.height, 24);
		RenderTexture rightRT = new RenderTexture(Screen.width/2, Screen.height, 24);
		
		GameObject leftCamera = meshDistortionCamera.transform.Find("Camera_left").gameObject;
		leftCamera.camera.targetTexture = leftRT;
		
		GameObject rightCamera = meshDistortionCamera.transform.Find ("Camera_right").gameObject;
		rightCamera.camera.targetTexture = rightRT;

		GameObject leftMesh = meshDistortionCamera.transform.Find ("LeftDistortionMesh").gameObject;
		MeshRenderer leftMeshRenderer = leftMesh.GetComponent<MeshRenderer> ();
		leftMeshRenderer.material.mainTexture = leftRT;
		leftMesh.transform.localScale = new Vector3 (Screen.width / 4, Screen.height /2, 1);
		leftMesh.transform.position = new Vector3 (-Screen.width / 4, 0, 1);

		GameObject rightMesh = meshDistortionCamera.transform.Find ("RightDistortionMesh").gameObject;
		MeshRenderer rightMeshRenderer = rightMesh.GetComponent<MeshRenderer> ();
		rightMeshRenderer.material.mainTexture = rightRT;
		rightMesh.transform.localScale = new Vector3 (Screen.width / 4, Screen.height /2, 1);
		rightMesh.transform.position = new Vector3 (Screen.width / 4, 0, 1);

		GameObject camera = meshDistortionCamera.transform.Find ("Camera").gameObject;
		camera.camera.orthographicSize = Screen.height / 2.0f;
		float fov = Screen.width / Screen.height;
		camera.camera.fieldOfView = fov;
	}

	void SetCameraMode(CameraMode mode)
	{
		GameObject diveCamera = transform.Find ("Dive_Camera").gameObject;
		GameObject oculusCamera = transform.Find ("OVRCameraController").gameObject;
		GameObject meshDistortionCamera = transform.Find ("MeshBarrelDistortion").gameObject;

		oculusCamera.SetActive(false);
		diveCamera.SetActive(false);
		meshDistortionCamera.SetActive(false);

		switch (mode) {
		case CameraMode.SideBySide:
			diveCamera.SetActive(true);
			break;

		case CameraMode.OculusRift:
			oculusCamera.SetActive(true);
			break;

		case CameraMode.MeshBarrelDistortion:
			meshDistortionCamera.SetActive(true);
			SetMeshBarrelDistortion(meshDistortionCamera);
			break;

		default:
			break;
		}
	}
}
