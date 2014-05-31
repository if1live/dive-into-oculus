using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;



//Durovis Dive Head Tracking 
// copyright by Shoogee GmbH & Co. KG Refer to LICENCE.txt 
//OpenDive HEad Tracking for Unity by Stefan Welker

public class OpenDiveSensor : MonoBehaviour {
#if UNITY_EDITOR
#elif UNITY_ANDROID
	[DllImport("divesensor")]	private static extern void initialize_sensors();
	[DllImport("divesensor")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
	[DllImport("divesensor")]	private static extern int process();
	[DllImport("divesensor")]	private static extern void set_application_name(string name);
	[DllImport("divesensor")]	private static extern void use_udp(int switchon);
	[DllImport("divesensor")]	private static extern void get_version(string msg,int maxlength);
	[DllImport("divesensor")]	private static extern int get_error();

#elif UNITY_IPHONE
	[DllImport("__Internal")]	private static extern void initialize_sensors();
	[DllImport("__Internal")]	private static extern float get_q0();
	[DllImport("__Internal")]	private static extern float get_q1();
	[DllImport("__Internal")]	private static extern float get_q2();
	[DllImport("__Internal")]	private static extern float get_q3();
	[DllImport("__Internal")]	private static extern void DiveUpdateGyroData();
	[DllImport("__Internal")]	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);
#endif


	void Start() {
		// Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = 60;

#if UNITY_EDITOR
#elif UNITY_ANDROID
		Network.logLevel = NetworkLogLevel.Full;
		use_udp(1);
		initialize_sensors ();
		//int err = get_error();
#elif UNITY_IPHONE
		initialize_sensors();
#endif
	}
	

	void Update() {
#if UNITY_EDITOR
#elif UNITY_ANDROID
		float q0 = 0, q1 = 0, q2 = 0, q3 = 0;
		process();

		Quaternion rot;
		get_q(ref q0,ref q1,ref q2,ref q3);
		rot.x=-q2;rot.y=q3;rot.z=-q1;rot.w=q0;
		transform.rotation = rot;
#elif UNITY_IPHONE
		float q0 = 0, q1 = 0, q2 = 0, q3 = 0;
		DiveUpdateGyroData();
		get_q(ref q0,ref q1,ref q2,ref q3);

		Quaternion rot;
		rot.x=-q2;
		rot.y=q3;
		rot.z=-q1;
		rot.w=q0;
		transform.rotation = rot;
#endif

	}

	void OnGUI() {
	}
}

