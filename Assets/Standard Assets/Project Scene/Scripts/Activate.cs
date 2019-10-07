using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour {

    //time to hold hand
    float threshold = 2f;

    //distance from the palette to each block
    float radius = 2.2f;

    //max distance from hand to palette
    //TODO: adjust up slightly
    uint distanceMax = 50;

    private Camera camera;
    private GameObject spinebase;
    private GameObject handright;
    private GameObject handleft;
    private Color[] colors;

    //whether we can open it up or not
    //public?
    public bool disabled = false;

    //a slightly quicker way to pass than if statements
    //public?
    public int side = 0;

    //add time to this
    float timer = 0f;

	// Use this for initialization
	void Start () {
        //get the parents name and find all the relevant objects
        string parname = transform.parent.name;
        spinebase = GameObject.Find(parname + "SpineBase");
        handleft = GameObject.Find(parname + "HandLeft");
        handright = GameObject.Find(parname + "HandRight");
        camera = Camera.main;

        //we could do this globally but who cares, right
        colors = new Color[5] { Color.red, Color.blue, Color.white, Color.green, Color.yellow };

        //TODO: unsure why this is here, need to test
        //I know it changes colour to red, but couldnt it just use changeColor(Color.red)?
        changeColor(handleft.GetComponent<Track>().color);
    }

    //change own colour
    //TODO: test internal
    public void changeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    // Update is called once per frame
    void Update () {
        if(spinebase == null)
        {
            Destroy(this);
        }
        //follow the spinebase
        //if side != 0 (left or right), it will be positioned to the side (-1 * 3 and 1 * 3 for x, 1 for y)
        //if side == 0 (center), it will be positioned at 0 x, 2 y

        transform.position = new Vector3(spinebase.transform.position.x + side * 3f, spinebase.transform.position.y + (2 - Math.Abs(side)), spinebase.transform.position.z - 0.1f);

        //the main funcitonality
        activation();
    }

    private void activation()
    {
        //get camera and hand positions in screen coords
        Vector3 campos = camera.WorldToScreenPoint(transform.position);
        Vector3 hlpos = camera.WorldToScreenPoint(handleft.transform.position);
        Vector3 hrpos = camera.WorldToScreenPoint(handright.transform.position);

        //this is a fucking mess, but it works
        //it checks if both hands are close to the center or one hand is close to the peripheral palettes
        if (Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hlpos.x, hlpos.y)) < distanceMax
            && Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hrpos.x, hrpos.y)) < distanceMax
            && !disabled && side == 0)
        {
            timer += Time.deltaTime;
        }
        else if (Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hlpos.x, hlpos.y)) < distanceMax
            || Vector2.Distance(new Vector2(campos.x, campos.y), new Vector2(hrpos.x, hrpos.y)) < distanceMax
            && !disabled && side != 0)
        {
            timer += Time.deltaTime;
        } else
        {
            timer = 0f;
        }

        if(timer >= threshold)
        {
            //reset timer and disable
            timer = 0f;
            disabled = true;

            //this seems misnamed, it enables it after 2 seconds
            StartCoroutine(disableIt());

            //spawn the blocks in a half-circle around the palette
            for (int i = 0; i < colors.Length; i++)
            {
                //instantiate the block
                GameObject block = Instantiate(Resources.Load("Prefabs/Colorblock") as GameObject);
                
                //position it in the half-circle
                block.transform.position = new Vector3(
                    transform.position.x + radius * Mathf.Sin((-100 + i * 200 / (colors.Length - 1)) * Mathf.Deg2Rad),
                    transform.position.y + radius * Mathf.Cos((-100 + i * 200 / (colors.Length - 1)) * Mathf.Deg2Rad),
                    transform.position.z
                    );

                //scale it up, i don't know why we do that here since we could just fix the colorblock prefab
                block.transform.localScale = new Vector3(1.5f, 1.5f, 0.1f);

                //rotate it slightly because orthographic camera and we need the shader to work
                block.transform.rotation *= Quaternion.AngleAxis(5f * side, Vector3.up);
                block.transform.rotation *= Quaternion.AngleAxis(-10f * side, Vector3.right);

                

                //make it follow the palette
                block.transform.parent = transform;

                //tag it so we can destroy it
                block.tag = "Block";

                //give it a colour and a side
                block.GetComponent<Renderer>().material.color = colors[i];
                block.GetComponent<Colorchange>().side = side;
            }

            StartCoroutine(destroyBlockOnTimer());
        }
    }

    private IEnumerator destroyBlockOnTimer()
    {
        yield return new WaitForSeconds(4f);

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    //called by the blocks
    public void enableIt()
    {
        //disable this?
        disabled = true;

        //enable it again after 2 secs
        StartCoroutine(disableIt());

        //destroy all blocks
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator disableIt()
    {
        yield return new WaitForSeconds(2f);
        disabled = false;
    }

    
}