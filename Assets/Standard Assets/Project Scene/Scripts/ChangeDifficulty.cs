using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDifficulty : MonoBehaviour {

    [SerializeField]
    private Game.GameMode mode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void activate()
    {
        GameObject go = GameObject.Find("Gamemode Saver");
        go.GetComponent<GamemodeSaveBehavior>().setGameMode(mode);
    }
}
