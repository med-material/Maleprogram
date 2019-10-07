using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour {

    //length of the array in frames
    //higher numbers give straighter lines
    //lower numbers give more natural brushes
    int len = 5;

    //accessed by the paint script
    public Vector3 currentPosition;

    //using a list makes it easy to remove the oldest point
    //a dict would be faster but unorganized, then hashmap but there's no reason over a list
    private List<Vector3> positions;

    //start colour
    public Color color = Color.red;

    //hand tip to palm distance
    private float distance = 0f;

    //tip of the hand
    private GameObject tip;

	// Use this for initialization
	void Start () {
        //initialize the list and get the appropriate tip
		positions = new List<Vector3> ();
		currentPosition = transform.position;
		positions.Add(currentPosition);
        getTip();

	}

    // Update is called once per frame
    void FixedUpdate() {
        //update the current position and it to the list
        currentPosition = transform.position;
        positions.Add(currentPosition);
        
        //if the list is too long, discard the oldest point
		if (positions.Count > len)
		{
			positions.RemoveAt(0);
		}

        //update the distance
        calcDistance();
	}

    private void calcDistance()
    {
        distance = Vector3.Distance(transform.position, tip.transform.position);
    }

    private void getTip()
    {
        if(name == "HandLeft")
        {
            tip = transform.parent.Find("HandTipLeft").gameObject;
        } else
        {
            tip = transform.parent.Find("HandTipRight").gameObject;
        }
    }

    public float GetDistance()
    {
        return distance;
    }

    public Vector3 average()
    {
		//initialize a zero vector
        Vector3 pos = Vector3.zero;

        //add all the points in the list
        for (int i = 0; i < positions.Count; i++)
        {
            pos += positions[i];
        }

        //average
        pos /= positions.Count;
        return pos;
    }

    public Vector3 weightedAverage() //For use with WeightedAverage Enum
    {
        Vector3 avg = average(); //reference is the average position
        List<Vector3> vecList = new List<Vector3>(); //we will use this for the weighted vectors
        float totalWeight = 0f; //for averaging

        for (int i = 0; i < positions.Count; i++) //go through all the positions in our list
        {
            float weight = 1 / Vector3.Distance(positions[i], avg); //the weight of this point
            vecList.Add(positions[i] * weight); //add the position to our new array
            totalWeight += weight; //add it's weight to our total
        }

        Vector3 pos = Vector3.zero; //we will return this new point
        for (int i = 0; i < vecList.Count; i++) //go through our new list
        {
            pos += vecList[i]; //add all the weightec points together
        }
        pos /= totalWeight; //"average" using the total weight
        return pos; //return statement
    }
}
