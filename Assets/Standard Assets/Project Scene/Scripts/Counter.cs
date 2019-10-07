using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Counter : MonoBehaviour {

	public Text text;
	public float timeRemaining;
	// Use this for initialization
	void Start () {
		timeRemaining = 3.0f;
	}

	// Update is called once per frame
	void Update () {
		timeRemaining -= Time.deltaTime;
		text.text = "Time left:" + Mathf.Round (timeRemaining);
		if (timeRemaining < 0) {
			SceneManager.LoadScene ("Loading");
		}
	}
		
}