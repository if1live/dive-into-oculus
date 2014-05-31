using UnityEngine;
using System.Collections;

public enum EaseType {None, In, Out, InOut}

public class Fade {
	public static IEnumerator Alpha (GUITexture tex, float start, float end, float timer) {
		yield return Alpha(tex, start, end, timer, EaseType.None);
	}
	
	public static IEnumerator Alpha (GUITexture tex, float start, float end, float timer, EaseType easeType) {
		float t = 0.0f;
		while (t < 1.0) {
			t += Time.deltaTime * (1.0f/timer);
			Color color = tex.color;
			color.a = Mathf.Lerp(start, end, Ease(t, easeType)) * 0.5f;
			tex.color = color;
			yield return 0;
		}
	}

	static float Ease (float t, EaseType easeType) {
		if (easeType == EaseType.None) {
			return t;
		} else if (easeType == EaseType.In) {
			return Mathf.Lerp (0.0f, 1.0f, 1.0f - Mathf.Cos (t * Mathf.PI * .5f));
		} else if (easeType == EaseType.Out) {
			return Mathf.Lerp (0.0f, 1.0f, Mathf.Sin (t * Mathf.PI * .5f));
		} else {
			return Mathf.SmoothStep (0.0f, 1.0f, t);
		}
	}
}
