﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Game : MonoBehaviour {

    public enum GameMode
    {
        None, Simple, Complex, Therapist
    }

    [SerializeField]
    private GameObject point;

	[SerializeField]
	private Text countDownText;
	private string countDownTextTemplate;

    private int  multiplier = 2;
    private int amount = 0;

	private int sessionLength = 1;
	private float sessionTimeStart = -1;
	private bool sessionFinish;

    private GameMode gm = GameMode.Therapist;

	[SerializeField]
	private Camera sceneCamera;

	[SerializeField]
	private float spawnDistance = 5f;

	private LineRenderer lineRenderer;
	private int vertexIndex = -1;
	private float currentXPos = 0f;
	private float currentYPos = 0f;
	private float centerX = 0f;
	private float centerY = -5f;
	private float countTarget = -1;
	private float currentCount = -1.0f;
	private float theta = -1.0f; 

	private PointBehavior gamePoint = null;

	[SerializeField]
	private LogToCSV logger;

    BodySourceView view;

	// Use this for initialization
	void Start () {
        view = GameObject.Find("BodyView").GetComponent<BodySourceView>();
		sessionTimeStart = Time.time;
		countDownTextTemplate = countDownText.text;
		logger = gameObject.GetComponent<LogToCSV> ();
		lineRenderer = this.GetComponent<LineRenderer> ();

		if (gm == GameMode.Simple) {
			amount = 1;
			//amount = view.amountOfPlayers() * multiplier;
			int numOfPointsInScene = GameObject.FindGameObjectsWithTag ("Point").Length;
			gameObject.GetComponent<Paint> ().enabled = false;
			if (numOfPointsInScene < amount) {
				spawnAnother ();
			}
		} else {
			gameObject.GetComponent<Paint> ().enabled = true;
		}

		if (gm == GameMode.Therapist) {
			amount = 1;
			int numOfPointsInScene = GameObject.FindGameObjectsWithTag ("Point").Length;
			gameObject.GetComponent<Paint> ().enabled = false;
			if (numOfPointsInScene < amount) {
				spawnAnother ();
			}
			CalculateCircle ();
			// SetSpawnDistance
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gm == GameMode.Therapist) {
			countDownText.text = "Distance: " + spawnDistance.ToString("0.0");
			return;
		}
		float time = Time.time;
		sessionFinish = (Time.time - sessionTimeStart > sessionLength * 60);

		var timeSpan = TimeSpan.FromSeconds (sessionLength * 60 - (Time.time - sessionTimeStart));
		countDownText.text = string.Format (countDownTextTemplate, timeSpan.Minutes.ToString (), timeSpan.Seconds.ToString ("00"));

		//Debug.Log (Time.time - sessionTimeStart);
		if (sessionFinish) {
			logger.SaveLogFile ();
			var hands = GameObject.FindGameObjectsWithTag ("Hand");
			if (hands.Length > 0) {
				// only take pictures for painting tasks
				if (gm == GameMode.None) {
					logger.takeScreenshot ();
				}
				SceneManager.LoadScene ("Menu");
			} else {
				SceneManager.LoadScene ("Screensaver");
			}
		}
	}

	public void OnApplicationQuit() {
		logger.SaveLogFile ();
	}

	private void OnMouseDown() {
		if (gm != GameMode.Therapist) {
			return;
		}
		var width = Screen.width;
		var height = Screen.height;
		var pos = Input.mousePosition;
		//var x = pos.x - (width / 2);
		//var y = pos.y - (height / 2);
		var new_pos = sceneCamera.ScreenToWorldPoint (pos);
		var offset_pos = new Vector2(new_pos.x - centerX, new_pos.y - centerY);
		Debug.Log (offset_pos.ToString ());
		spawnDistance = offset_pos.magnitude;
		CalculateCircle ();

		if (gamePoint != null) {
			gamePoint.updateSpawnDistance (centerX, centerY, spawnDistance);
			gamePoint.updatePosition (true);
		}
		//Debug.Log (new_pos.ToString () + " - distance: " + new_pos.magnitude);
		//Debug.Log ("x: " + x.ToString() + "y: " + y.ToString());

	}
	
	private void CalculateCircle() {
		// clear the line first.
		lineRenderer.numPositions = 0;
		vertexIndex = -1;

		for (int i = 1; i < 25; i++) {
			//Debug.Log ("i: " + i.ToString ());
			vertexIndex++;
			theta = ((float)i / 25.0f) * (1.0f * Mathf.PI);
			//Debug.Log (theta.ToString() + " = (" + i + " / 100) * (1 * Mathf.PI)");
			currentXPos = (centerX + (spawnDistance * Mathf.Cos(theta)));
			currentYPos = (centerY + (spawnDistance * Mathf.Sin (theta)));
			//Debug.Log (currentXPos.ToString() + " = (" + centerX.ToString() + " + (" + spawnDistance.ToString() + " * " + Mathf.Cos (theta).ToString() + "))");
			lineRenderer.numPositions++;
			lineRenderer.SetPosition(vertexIndex, new Vector3(currentXPos, currentYPos, 2.00f));
			//Debug.Log("vertIndex[" + vertexIndex + "] x(" + currentXPos + ") y(" + currentYPos + ") positionCount[" + lineRenderer.numPositions + "] theta(" + theta + ")");
		}

	}
	

    internal void changeGameMode(GameMode mode)
    {
		gm = mode;
    }

    internal void spawnAnother()
    {
		GameObject go = Instantiate(point, new Vector3(0f,0f,0f), point.transform.localRotation);
		gamePoint = go.GetComponent<PointBehavior> ();
		gamePoint.updateSpawnDistance (centerX, centerY, spawnDistance);
		gamePoint.updateMode(gm);
    }

    public GameMode getGameMode()
    {
        return gm;
    }
}
