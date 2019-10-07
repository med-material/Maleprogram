
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Windows.Kinect;


public class LogToCSV : MonoBehaviour {

    string path;

    StreamWriter sw;
	private List<string> logEntries;

    GameObject manager;
    GameObject canvas;
    Dictionary<ulong, GameObject> bodies;

    ColorSourceManager csm;

    int i = 0;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas");
        manager = GameObject.Find("BodyView");
        csm = GameObject.Find("ColorSourceManager").GetComponent<ColorSourceManager>();
		logEntries = new List<string> ();

        string gamemode = canvas.GetComponent<Game>().getGameMode().ToString();

        DateTime dt = DateTime.Now;
        path = Application.dataPath + "/Data/Session " + dt.ToString("yyyy-MM-dd HH;mm;ss ") + gamemode +".csv";
        //Attempt to create folder
        try
        {
            if (!Directory.Exists(Application.dataPath + "/Data"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Data");
                
            }

            //sw.WriteLine()

        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
        }

        sw = new StreamWriter(path);

	}
	
	void FixedUpdate () {

        bodies = manager.GetComponent<BodySourceView>().getDict();

        int num = bodies.Count;

        

        if(i < 1)
        {
            i++;
            return;
        }
        else
        {
            i = 0;
        }

        foreach(KeyValuePair<ulong, GameObject> kvp in bodies)
        {
            Track leftHand = kvp.Value.transform.FindChild("HandLeft").gameObject.GetComponent<Track>();
            Track rightHand = kvp.Value.transform.FindChild("HandRight").gameObject.GetComponent<Track>();

            string lcp = "" + leftHand.currentPosition.x + "," + leftHand.currentPosition.y + "," + leftHand.currentPosition.z;
            string rcp = "" + rightHand.currentPosition.x + "," + rightHand.currentPosition.y + "," + rightHand.currentPosition.z;

            string id = kvp.Key.ToString();
            string time = DateTime.Now.ToString("HH:mm:ss.fff");

            string str = "" + id + "," + time + "," + "Left" + "," + lcp + "," + num;
			logEntries.Add (str);

			str = "" + id + "," + time + "," + "Right" + "," + rcp + "," + num;
			logEntries.Add (str);
        }
	}

	public void SaveLogFile() {
		foreach (var log in logEntries) {
			sw.WriteLine (log);
			sw.Flush ();
		}
	}

	public void takeScreenshot() {
		StartCoroutine(saveImg());
	}

    IEnumerator saveImg()
    {
		// temporarily turn off visibility of body objects
		var bodies = GameObject.FindGameObjectsWithTag("HideForScreenshot");
		foreach (GameObject body in bodies) {
			body.SetActive (false);
		}

        //encode the canvas texture to PNG
        Texture2D img = csm.GetColorTexture();
        img.Apply(true, false);
        Texture2D newImg = FlipTexture(img);
        byte[] bytes = newImg.EncodeToJPG();

        //get the current date
        DateTime dt = DateTime.Now;

        //write the array to the datapath and name it after the current date
        File.WriteAllBytes(Application.dataPath + "/Data/Session " + dt.ToString("yyyy-MM-dd HH-mm-ss-fff") + "_camera.jpg", bytes);
		Application.CaptureScreenshot(Application.dataPath + "/Data/Session " + dt.ToString("yyyy-MM-dd HH-mm-ss-fff") + "_screenshot.jpg");

		/*foreach (GameObject body in bodies) {
			body.SetActive (true);
		}*/

		yield return null;
	}

    Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width / 3, original.height / 3);

        int xN = flipped.width;
        int yN = flipped.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(i, yN - j - 1, original.GetPixel(i * 3, j * 3));
            }
        }
        flipped.Apply();

        return flipped;
    }
}
