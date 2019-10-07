using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorchange : MonoBehaviour {

    //time to hold hand
    float threshold = 1.5f;

    //distance from the palette to each block
    uint distanceMax = 50;

    //add time to this
    float timer = 0f;

    private Camera camera;
    private GameObject handright;
    private GameObject handleft;
    private Color col;

    //a slightly quicker way to pass than if statements
    //public?
    public int side = 0;

    // Use this for initialization
    void Start () {

        //this is simple stuff
        string parname = transform.parent.parent.name;
        handleft = GameObject.Find(parname + "HandLeft");
        handright = GameObject.Find(parname + "HandRight");
        camera = Camera.main;
        col = GetComponent<Renderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {

        //get camera and hand positions in screen coords
        Vector3 campos = camera.WorldToScreenPoint(transform.position);
        Vector3 hlpos = camera.WorldToScreenPoint(handleft.transform.position);
        Vector3 hrpos = camera.WorldToScreenPoint(handright.transform.position);

        //if a hand is held over the block, add time to the timer
        if (Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hlpos.x, hlpos.y)) < distanceMax
            || Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hrpos.x, hrpos.y)) < distanceMax)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
        }

        if(timer >= threshold)
        {
            Track lt = handleft.GetComponent<Track>();
            Track rt = handright.GetComponent<Track>();

            //we couldn't fully avoid the if statements
            if(side == -1)
            {
                lt.color = col;
            } else if (side == 1)
            {
                rt.color = col;
            } else
            {
                lt.color = col;
                rt.color = col;
            }

            //change the palette's colour and activate selfdestruct sequence
            transform.parent.gameObject.GetComponent<Activate>().changeColor(col);
            transform.parent.GetComponent<Activate>().enableIt();

            /*GameObject[] des = GameObject.FindGameObjectsWithTag("Block");
            foreach(GameObject go in des)
            {
                GameObject.Destroy(go);
            }*/
        }
    }
}
