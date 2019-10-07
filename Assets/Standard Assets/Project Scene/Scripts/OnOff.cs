using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour {

	public GameObject Canvas;
	public GameObject Canvase;
	public GameObject Canvaser;
	public bool isShowing;

	// Use this for initialization
	void Start () {
		isShowing = false;
		Canvas.SetActive (false);
		Canvase.SetActive (false);
		Canvaser.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("space")) {
				isShowing = !isShowing;
				Canvas.SetActive (isShowing);
				Canvase.SetActive (isShowing);
				Canvaser.SetActive (isShowing);
		}
	}
}
