using UnityEngine;
using System;
using System.Collections;

public class P
{
	public static bool isInit = false;
	public static float magicNumber = 0f;

	public static void init(){
		if (isInit)
			return;
		isInit = true;
		GameObject g = new GameObject ();
		Sprite sp = Resources.Load<Sprite> ("pixel");

		g.AddComponent<SpriteRenderer> ().sprite = sp;
		g.transform.localScale = new Vector3 (Screen.height * Camera.main.aspect, Screen.height, 1f);
		magicNumber = 10f / g.GetComponent<SpriteRenderer> ().bounds.size.y;
		//10 = Camera.main.size*2
		//Debug.Log ("magic " + g.GetComponent<SpriteRenderer> ().bounds.size.y);
		UnityEngine.Object.Destroy(g);
	}

	/**
	 * percent of screen in "Scale" unit (garanteed to work in transform.localScale and Edit mode only)
	 */
	public static float pocSEditor(float percent, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return percent * (Screen.width + Screen.width*0.1f);
		case Side.H:
		case Side.HEIGHT:
			return percent * (Screen.height + Screen.height * 0.1f);
		}
		return -1f;
	}

	/**
	 * percent of screen in "Scale" unit (garanteed to work in transform.localScale and Game mode only)
	 */
	public static float pocSGame(float percent, Side side){
		//new Vector3 (Screen.height*Camera.main.aspect, Screen.height, 1f)
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (percent * (Screen.height*Camera.main.aspect)) * magicNumber;
		case Side.H:
		case Side.HEIGHT:
			return (percent * Screen.height) * magicNumber;
		}
		return -1f;
	}

	/**
	 * percent of screen in "Position" unit (garanteed to work in transform.position only)
	 */
	public static float pocP(float percent, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (percent * (Camera.main.orthographicSize * Camera.main.aspect));
		case Side.H:
		case Side.HEIGHT:
			return (percent * Camera.main.orthographicSize);
		}
		return -1f;
	}

	public static float unPocP(float worldPos, Side side){
		switch (side) {
		case Side.W:
		case Side.WIDTH:
			return (worldPos / (Camera.main.orthographicSize * Camera.main.aspect));
		case Side.H:
		case Side.HEIGHT:
			return (worldPos / Camera.main.orthographicSize);
		}
		return -1f;
	}

	public static IEnumerator FadeAlpha(SpriteRenderer sr, float time, float from){
		return AnimateValue (from, 0f, time, null, 
			(float value, object o) => {
				SpriteRenderer srr = (SpriteRenderer) o;
				srr.color = new Color(srr.color.r, srr.color.g, srr.color.b, value);
			}, sr);
	}

	public static IEnumerator AnimateScaleYPoc(Transform transform, float from, float to, float time, Interpolator i){
		return AnimateValue (P.pocSGame (from, Side.H), P.pocSGame (to, Side.H), time, i, 
			(float value, object o) => {
				Transform t = (Transform) o;
				t.localScale = new Vector3(t.localScale.x, value, t.localScale.z);
			}, transform);
	}
	
	public static IEnumerator AnimateValue(float from, float to, float duration, Interpolator interpolator, OnAnimationUpdateObj oau, object o){
		float startTime = Time.time;
		while (Time.time - startTime <= duration) {
			float value;
			if(interpolator != null)
				value = (from + (to - from)*interpolator((Time.time - startTime)/duration));
			else
				value = (from + (to - from)*((Time.time - startTime)/duration));
			if (oau != null)
				oau (value, o);
			yield return new WaitForEndOfFrame ();
		}
		if (oau != null)
			oau (to, o);
	}
	
	public static float AccelerateDeccelerateInterpolator(float input){
		return (Mathf.Cos ((input + 1) * Mathf.PI) / 2.0f) + 0.5f;
	}

	public static float OvershootInterpolator(float t){
		t -= 1.0f;
		//2.0f can be replaced by tension  
		return t * t * ((2.0f + 1) * t + 2.0f) + 1.0f;
	}

	public static float AnticipateInterpolator(float t){
		return t * t * ((2.0f + 1) * t - 2.0f);
	}
	
	 public static float DecelerateInterpolator(float input) {
		return (float)(1.0f - Mathf.Pow((1.0f - input), 2 * 2.0f));
    }
}

public delegate float Interpolator(float input);
public delegate void OnAnimationUpdateObj(float value, object o);

public enum Side{
	W,
	H, 
	WIDTH,
	HEIGHT
}


