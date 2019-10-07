using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButtonScript : MonoBehaviour {

	[SerializeField]
	private Texture2D[] backgrounds;

	[SerializeField]
	private Texture2D blank;

	[SerializeField]
	private Text countText;

	[SerializeField]
	private GameObject[] titles;

	private float startTime = -1;
	private int countDuration = 5;
	private int currentCount = -1;
	private int gameMode;
	private bool shouldCount = true;


    // Use this for initialization
    void Start () {
		int gameModeCount = 3; // 3 types of modes
		gameMode = UnityEngine.Random.Range(0,gameModeCount);
		//gameMode = 3; // set this to activate therapist mode
		currentCount = countDuration;
		titles [gameMode].SetActive (true);
		startTime = Time.time;
	}

    private void feedToSaveObject()
    {

    }

    // Update is called once per frame
    void Update () {
		if (shouldCount && (Time.time - startTime) < countDuration) {
			countText.text = (countDuration - (int)Mathf.Round(Time.time - startTime)).ToString();
		} else {
			shouldCount = false;
			if (gameMode == 0) { // Just Draw on a white canvas
				//Debug.Log (gameMode);
				GameObject go = GameObject.Find ("Gamemode Saver");
				go.GetComponent<GamemodeSaveBehavior> ().setGameMode (Game.GameMode.None);
				go.GetComponent<GamemodeSaveBehavior> ().setTemplate (null);
				SceneManager.LoadScene ("Project Scene");
			} else if (gameMode == 1) { // Draw with a random background
				//Debug.Log (gameMode);
				int randomBackground = UnityEngine.Random.Range (0, backgrounds.Length);
				GameObject go = GameObject.Find ("Gamemode Saver");
				go.GetComponent<GamemodeSaveBehavior> ().setTemplate (backgrounds [randomBackground]);
				go.GetComponent<GamemodeSaveBehavior> ().setGameMode (Game.GameMode.None);
				SceneManager.LoadScene ("Project Scene");
			} else if (gameMode == 2) { // Catch the squares!
				//Debug.Log(gameMode);
				GameObject go = GameObject.Find ("Gamemode Saver");
				go.GetComponent<GamemodeSaveBehavior> ().setGameMode (Game.GameMode.Simple);
				go.GetComponent<GamemodeSaveBehavior> ().setTemplate (null);
				SceneManager.LoadScene ("Project Scene");
			} else if (gameMode == 3) { // Catch the squares!
			//Debug.Log(gameMode);
			GameObject go = GameObject.Find ("Gamemode Saver");
			go.GetComponent<GamemodeSaveBehavior> ().setGameMode (Game.GameMode.Therapist);
			go.GetComponent<GamemodeSaveBehavior> ().setTemplate (null);
			SceneManager.LoadScene ("Project Scene");
			}
		}
	}
}
