using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RPB : MonoBehaviour {

	public Transform LoadingBar;
	public Transform Procent;
	public Transform Text;
	[SerializeField] private float currentAmount;
	[SerializeField] private float speed;
	bool dennis = false;

	// Use this for initialization
	void Start () {
		GameObject.Find ("Canvas").GetComponent<OnOff> ().isShowing.Equals (false);
	}

	// Update is called once per frame
	void Update () {
		if (GameObject.Find ("Canvas").GetComponent<OnOff> ().isShowing = true) {
			if (currentAmount < 100) {
				currentAmount += speed * Time.deltaTime;
				Procent.GetComponent<Text> ().text = ((int)currentAmount).ToString () + "%";
				Text.gameObject.SetActive (true);
			} else {
				Text.gameObject.SetActive (false);
				Text.GetComponent<Text> ().text = "DONE!";
				SceneManager.LoadScene ("Menu");
			}
			LoadingBar.GetComponent<Image> ().fillAmount = currentAmount / 100;
			LoadingBar.GetComponent<Image> ().color = Color.Lerp(Color.red, Color.green, LoadingBar.GetComponent<Image> ().fillAmount);
			
		}
	}
}
