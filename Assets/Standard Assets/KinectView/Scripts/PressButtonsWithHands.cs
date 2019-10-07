using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonsWithHands : MonoBehaviour {

    float timer = 0f;

    Camera main;
    GameObject button;

	// Use this for initialization
	void Start () {
        main = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 direction = transform.position - main.transform.position;

        RaycastHit hitinfo;

        if(Physics.Raycast(transform.position, direction, out hitinfo, 100f))
        {
            if(button == hitinfo.collider.gameObject)
            {
                timer += Time.deltaTime;
            }

            else
            {
                button = hitinfo.collider.gameObject;
                timer = 0f;
            }
        }

        if(timer >= 2f)
        {
            timer = 0f;

            if(button.GetComponent<Button>() != null)
            {
                button.GetComponent<Button>().onClick.Invoke();
            }
            
        }

	}
}
