using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTemplate : MonoBehaviour {

    [SerializeField]
    private Texture2D temp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void activate()
    {
        GameObject go = GameObject.Find("Gamemode Saver");
        go.GetComponent<GamemodeSaveBehavior>().setTemplate(temp);
    }
}
