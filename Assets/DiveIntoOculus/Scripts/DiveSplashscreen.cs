using UnityEngine;
using System.Collections;

public class DiveSplashscreen : MonoBehaviour {
	// Use this for initialization
	void Start () {
		StartCoroutine(OnStart());
	}

	IEnumerator OnStart() {
		GUITexture guiObject = this.GetComponent<GUITexture> ();
		//set position
		Rect texRect = guiObject.pixelInset;
		texRect.center = new Vector2 (Screen.width / 4.0f, Screen.height / 2.0f);
		guiObject.pixelInset = texRect;

		float timer = 1.0f;
		IEnumerator fadeIn = Fade.Alpha (guiObject, 0.0f, 1.0f, timer, EaseType.InOut);
		while (fadeIn.MoveNext()) {
			yield return 0;
		}
		IEnumerator fadeOut = Fade.Alpha (guiObject, 1.0f, 0.0f, timer, EaseType.InOut);
		while (fadeOut.MoveNext()) {
			yield return 0;
		}
		GameObject.DestroyObject (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
