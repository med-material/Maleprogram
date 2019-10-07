using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    //Define a serialport on COM3
    //TODO: Option for changing in final program in case it is not COM3
    private SerialPort sp;
    public string com = "COM3";

    //ints that allow os to check the single frame the button is pressed
    int lastFrame = 0;
    int currentFrame = 0;

    float timer = 0f;

	private LogToCSV logger;

    //[SerializeField]
    //Texture2D tex;


    // Use this for initialization
    void Start()
    {
        //have to run in background in order to not be backlogged by 1024 calls to readline
        //do not delete
        Application.runInBackground = true;
		logger = gameObject.GetComponent<LogToCSV> ();
        //open the serialport and establish communication
        //readtimeout should have worked to reduce lag but will retest
        sp = new SerialPort(com, 4800);
        sp.Open();
        sp.ReadTimeout = 1;
        sp.Handshake = Handshake.RequestToSend;

        //run our "update" function as a coroutine
        StartCoroutine(Updater());
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
		if(sp != null && !sp.IsOpen)
        {
            sp.Open();
        }

		/*if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Project Scene"))
        {
            timer += Time.deltaTime;
        }
        

        if(timer >= 120f)
        {
            StartCoroutine(save());
            return;
        }*/

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    
    IEnumerator Updater()
    {
        bool b = true;
        while(b)
        {
			yield return new WaitForSeconds(0.25f / 4800f); // changed from 1f, seems to make button more responsive
            if (sp.IsOpen)
            {
                try
                {
                    //set lastframe to our old current frame and get current frame data from arduino
                    lastFrame = currentFrame;
                    currentFrame = int.Parse(sp.ReadLine());
                }
                catch (System.Exception e)
                {
                    //Debug.Log(e + " " + e.StackTrace);
                }
				if (lastFrame == 0 && currentFrame == 1) {
                    
                    //stop the loop (safety measure to not get several calls)
                    b = false;

                    //start the process of saving and exiting to the other scene
                    //yield return StartCoroutine(save());
					if (logger != null) {
						//logger.takeScreenshot ();
					}
					SceneManager.LoadScene("Menu");
                }
            }
        }
    }

    /*IEnumerator save()
    {   //wait for everything to be done in this frame (so we dont get half a paint stroke, for example)     
        yield return new WaitForEndOfFrame();


		if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Screensaver"))
		{
			SceneManager.LoadScene("Menu");
		} else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Project Scene"))
        {
			//if we are in the painting scene we need to save the painting and go to the "menu"
			//encode the canvas texture to PNG
            //Texture2D canvas = (Texture2D)GetComponent<Renderer>().material.mainTexture;
            //byte[] bytes = canvas.EncodeToPNG();

            //get the current data
            //DateTime dt = DateTime.Now;

            //write the array to the datapath and name it after the current date
            //File.WriteAllBytes(Application.dataPath + "/../Maleri " + dt.ToString("yyyy-MM-dd HH;mm;ss") + ".png", bytes);

            //jump to the menu
			if (logger != null) {
				logger.takeScreenshot ();
			}
            SceneManager.LoadScene("Menu");
        }
        else
        {
			SceneManager.LoadScene("Menu");
            //if we enter this way, reset the settings
            //GamemodeSaveBehavior saveState = GameObject.Find("Gamemode Saver").GetComponent<GamemodeSaveBehavior>();
            //saveState.setGameMode(Game.GameMode.None);
            //saveState.setTemplate(tex);

            //asynchronously load the main scene
            //SceneManager.LoadScene("Project Scene");

            //while we make some nice loading text
            //StartCoroutine(loadtext());

            
        }
    }*/

    /*IEnumerator loadtext()
    {
        //prepare the string and text element
        string extra = "INDLÆSER";
        Text text = GameObject.Find("Canvas/Image/Text").GetComponent<Text>();
        text.text += "\n";

        //add one letter per amount of time
        for (int i = 0; i < extra.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            text.text += extra[i];
        }
    }*/

    /*public void resetTimer()
	{
		timer = 0f;
	}*/
}