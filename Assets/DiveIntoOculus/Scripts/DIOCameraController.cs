using UnityEngine;
using System.Collections;

public class DIOCameraController : MonoBehaviour {
	private GameObject OculusCamera;
	private GameObject DiveCamera;

	// Use this for initialization
	void Start () {
		DiveCamera = transform.Find ("Dive_Camera").gameObject;
		OculusCamera = transform.Find ("OVRCameraController").gameObject;

#if UNITY_ANDROID
		//UseDiveMode();
#elif UNITY_IPHONE
		UseDiveMode();
#else
		UseOculusMode();
#endif
	}

	// Update is called once per frame
	void Update () {
	
	}

	void UseOculusMode() {
		OculusCamera.SetActive(true);
		DiveCamera.SetActive(false);
	}
	void UseDiveMode() {
		OculusCamera.SetActive(false);
		DiveCamera.SetActive(true);
	}
}
