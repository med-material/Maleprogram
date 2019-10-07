using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour {

	Image sprite;
	Text text;
	public float duration;
	public bool startNow;
	public float startDelay;

	private float f = 1f;
	private bool fadeOut = false;
	private float startTime;
	private Color defaultColor;

	// Use this for initialization
	void Start () {
		sprite = this.GetComponent<Image> ();
		text = this.GetComponent<Text> ();
		if (sprite != null) {
			defaultColor = sprite.color;
			//sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, 1f);
			//Debug.Log ("found sprite");
		}
		if (text != null) {
			defaultColor = text.color;
			//text.color = new Color (text.color.r, text.color.g, text.color.b, 1f);
			//Debug.Log ("found text");
		}
		startTime = Time.fixedTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (!startNow) {
			startTime = Time.fixedTime;
			return;
		}

		if (startDelay < (Time.fixedTime - startTime)) {
			fadeOut = true;
		}
		//Debug.Log(Time.fixedTime - startTime + " seconds passed");	

		if (fadeOut) {
			if (sprite != null) {
				sprite.color = new Color (sprite.color.r, sprite.color.g, sprite.color.b, f);
			}
			if (text != null) {
				text.color = new Color (text.color.r, text.color.g, text.color.b, f);
			}
			if (f <= 0) {
				fadeOut = false;
				startNow = false;
				f = 1;
			}
			f -= Time.deltaTime / duration;
			//Debug.Log ("alpha is: " + f);
		}
	}

	public void StartFade()
	{
		startNow = true;
	}

	public void ResetFade()
	{
		//print ("resetFade() called");
		f = 1f;
		//startNow = true;
		fadeOut = false;

		/*if (sprite != null)
		{
			sprite.color = defaultColor;
			//Debug.Log ("found sprite");
		}
		if (text != null)
		{
			text.color = defaultColor;
			//Debug.Log ("found text");
		}*/
		startTime = Time.fixedTime;
	}
}
