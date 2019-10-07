using System.Collections;
using UnityEngine;

/*
 * credit to user toddisarockstar on Stackoverflow for providing the beginning of this script
 * http://answers.unity3d.com/answers/1324741/view.html
 */

public enum Filter
{
    None,Average,WeightedAverage
}

public class Paint : MonoBehaviour
{

	//what filter are we using
    public Filter filterMode;

    //thats a lot of small and insignificant variables
    //TODO: cleanup (not me)
    int i;
    int i2;
    Color c;
    float f;

    int sw;
    int sh;

    float mx;
    float my;

    int px;
    int py;

    ulong it = 0;

    //drop a texture into the inspector 
    //in the texture import settings you must set the type to "advanced"
    //and then set "read/write" to true!!!!!!!
    public Texture2D original;

    //unsure why this is public
    //TODO: test encapsulation
    public Camera cam;

    //the texture we will paint on
    Texture2D myimage;

    //brush size(s), length to determine closed fist, and something else
    //TODO: remove oldp and test
    public int brush;
    public int erase;
    public float fist = 0f;
    int oldp;


    //array of all the hands we will find
    GameObject[] hands;

    //the music
    //TODO: add more and make a smart system for it, eg array
    //      then choosing other music just adds +5 to the elements to add volume to
	AudioSource Drums;
	AudioSource Bass;
	AudioSource FX;
	AudioSource Piano;
	AudioSource Vocal;

    //ButtonBehaviour btn;

    void Start()
    {
        //this got messy in the main function
		//findObjects ();



        //if there is no original picture, eg angry birds, then just make a blank canvas
        if (original == null) { original = new Texture2D(1920, 1080); }

        //copy our original size into our new paintable image 
        myimage = new Texture2D(original.width, original.height);

        //apply the original image to our canvas
        //TODO: cleanup. Can it be done with canvas.material = original? who knows
        i = original.width;
        while (i > 0)
        {
            i--;
            i2 = original.height;
            while (i2 > 0)
            {
                i2--;
                c = original.GetPixel(i, i2);
                myimage.SetPixel(i, i2, c);
            }
        }


        myimage.Apply();
        myimage.filterMode = FilterMode.Bilinear; // was Point

        //set some of the global vars
        sw = Screen.width;
        sh = Screen.height;

        //start adjusting the music volume
		//StartCoroutine (evaluateTexture ());
    }


	void FixedUpdate()
	{

		it++;

		//get all hands in the scene
		hands = GameObject.FindGameObjectsWithTag("Hand");

		int radius = (int)brush / 2;
		int r2 = radius * radius;
		int area = r2 << 2;
		int rr = radius << 1;

		/*if(hands.Length > 0)
        {
            btn.resetTimer();
        }*/

		bool blue   = false;
		bool green  = false;
		bool red    = false;
		bool yellow = false;

		//iterate through them
		foreach(GameObject hand in hands)
		{
			//get the color to draw with from the hand
			Color drawCol = hand.GetComponent<Track>().color;

			//if we are correctly tracking the hand and it is not a fist, we can paint
			if (hand.GetComponent<LineRenderer>().endColor == Color.green || hand.GetComponent<LineRenderer>().endColor == Color.red && hand.GetComponent<Track>().GetDistance() > fist)
			{

				Track track = hand.GetComponent<Track>();

				//create a vector to determine the hand's position
				//we get the position directly from the hand because it already holds all the information necessary
				Vector3 pos = Vector3.zero;
				if (filterMode == Filter.None) {
					pos = track.currentPosition;
				} else if(filterMode == Filter.Average)
				{
					pos = track.average();
				} else if(filterMode == Filter.WeightedAverage)
				{
					pos = track.weightedAverage();
				}

				//transform this position to screen coordinates
				Vector3 screenPos = cam.WorldToScreenPoint(pos);

				//normalise with the total width andh height
				mx = screenPos.x / sw;
				my = screenPos.y / sh;

				//now expand to canvas coordinates
				//i cant remember why the -1 for py
				px = Mathf.RoundToInt(myimage.width * mx);
				py = Mathf.RoundToInt(myimage.height * my) - 1;

				//subtract half the brush size in  either direction for the double loop start position
				//px += -Mathf.RoundToInt(brush * .5f);
				//py += -Mathf.RoundToInt(brush * .5f);

				for (int q = 0; q < area; q++)
				{
					int tx = (q % rr) - radius;
					int ty = (q / rr) - radius;

					if (myimage.GetPixel(px + tx, py + ty).r > 0.4f
						|| myimage.GetPixel(px + tx, py + ty).g > 0.4f
						|| myimage.GetPixel(px + tx, py + ty).b > 0.4f)
					{
						if (tx * tx + ty * ty <= r2)
						{
							myimage.SetPixel(px + tx, py + ty, drawCol);
						}
					}


				}


				/*
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        if (x * x + y * y <= radius * radius)
                        {
                            if (   myimage.GetPixel(px + x, py + y).r > 0.4f
                                || myimage.GetPixel(px + x, py + y).g > 0.4f
                                || myimage.GetPixel(px + x, py + y).b > 0.4f)
                            {
                                //set the current pixel to our drawcolor
                                myimage.SetPixel(px + x, py + y, drawCol);

                            }
                        }
                    }
                }
                */

				/*
				for(int i = 0; i < brush; i++) {
					for(int j = 0; j < brush; j++) { 
						//if statements to make sure we dont draw off the image width or height
						if (px + i > -1 && px + i < myimage.width)
						{
							if (py + j > -1 && py + j < myimage.height)
							{
								//check if any of this is particularly black
								//so it doesnt draw over the template, if any
								if(myimage.GetPixel(px+i,py+j).r > 0.4f 
									|| myimage.GetPixel(px+i,py+j).g > 0.4f
									|| myimage.GetPixel(px+i,py+j).b > 0.4f)
								{
									//set the current pixel to our drawcolor
									myimage.SetPixel(px + i, py + j, drawCol);

								}


							}
						}
					}
				}
				*/

			}

			//music has been moved here
			//first check which colours are active
			/*
            if(drawCol == Color.blue)
            {
                blue = true;
            }
            else if (drawCol == Color.green)
            {
                green = true;
            }
            else if (drawCol == Color.yellow)
            {
                yellow = true;
            }
            else if (drawCol == Color.red)
            {
                red = true;
            }*/

			//this is the gfx fix, but it didnt work
			//Graphics.CopyTexture(myimage, GetComponent<Renderer>().material.mainTexture);
		}

		//check which colours are active and set music volume
		/*
		Bass.volume  = blue   ? 1f : 0f;
        FX.volume    = green  ? 1f : 0f;
        Piano.volume = red    ? 1f : 0f;
        Vocal.volume = yellow ? 1f : 0f;
		*/
		//apply once per frame, that's all we need to keep it relatively low-resource intensive
		if(it % 2 == 1)
		{
			myimage.Apply();
			GetComponent<Renderer>().material.mainTexture = myimage;
		}


	}


	/*public void applyPaint(Track track) {
		it++;

		int radius = (int)brush / 2;
		int r2 = radius * radius;
		int area = r2 << 2;
		int rr = radius << 1;

		Color drawCol = track.color;

		Vector3 pos = Vector3.zero;
		if (filterMode == Filter.None) {
			pos = track.currentPosition;
		} else if(filterMode == Filter.Average)
		{
			pos = track.average();
		} else if(filterMode == Filter.WeightedAverage)
		{
			pos = track.weightedAverage();
		}

		//transform this position to screen coordinates
		Vector3 screenPos = cam.WorldToScreenPoint(pos);

		//normalise with the total width andh height
		mx = screenPos.x / sw;
		my = screenPos.y / sh;

		//now expand to canvas coordinates
		//i cant remember why the -1 for py
		px = Mathf.RoundToInt(myimage.width * mx);
		py = Mathf.RoundToInt(myimage.height * my) - 1;

		//subtract half the brush size in  either direction for the double loop start position
		//px += -Mathf.RoundToInt(brush * .5f);
		//py += -Mathf.RoundToInt(brush * .5f);

		for (int q = 0; q < area; q++)
		{
			int tx = (q % rr) - radius;
			int ty = (q / rr) - radius;

			if (myimage.GetPixel(px + tx, py + ty).r > 0.4f
				|| myimage.GetPixel(px + tx, py + ty).g > 0.4f
				|| myimage.GetPixel(px + tx, py + ty).b > 0.4f)
			{
				if (tx * tx + ty * ty <= r2)
				{
					myimage.SetPixel(px + tx, py + ty, drawCol);
					Debug.Log ("Applying pixels to canvas");
				}
			}


		}

	}*/


    //deprecated DO NOT USE
	/*IEnumerator evaluateTexture() 
	{

		float blue = 0;
		float red = 0;
		float yellow = 0;
		float green = 0;

        int halfbrush = Mathf.RoundToInt(brush / 2);

        //iterate through the entire canvas at half the brush size
		for (int i = 0; i < myimage.width; i += halfbrush) {
			for (int j = 0; j < myimage.height; j += halfbrush) {
				if (myimage.GetPixel (i, j) == Color.blue) {
					blue += halfbrush*halfbrush;
				} else if (myimage.GetPixel (i, j) == Color.red) {
					red += halfbrush * halfbrush;
                } else if (myimage.GetPixel (i, j) == Color.yellow) {
					yellow += halfbrush * halfbrush;
                } else if (myimage.GetPixel (i, j) == Color.green) {
					green += halfbrush * halfbrush;
                }
			}
		}

        //the ratio (as it should rather be called) is 1/5 the practical size of the canvas
		float cutoff = ((myimage.width - myimage.width/6) * (myimage.height - myimage.height/4) / 5);

        //set the music volumes to a number between 0 and the max volume we want it
		Drums.volume = 1f;
		Bass.volume = Mathf.Min (0.908f, blue / cutoff);
		FX.volume = Mathf.Min (1f, green / cutoff);
		Piano.volume = Mathf.Min (0.826f, red / cutoff);
		Vocal.volume = Mathf.Min (1f, yellow / cutoff);

        //wait 1 second and do it again
		yield return new WaitForSeconds (1f);
		StartCoroutine (evaluateTexture ());
	}*/

	/*private void findObjects () 
	{
        //this should make sense, just finding all the audio
		GameObject soundObj = GameObject.Find ("Main Camera/MusicPlayer");
		AudioSource[] audios = soundObj.GetComponents<UnityEngine.AudioSource> ();
		Drums = audios [0];
		Bass = audios [1];
		FX = audios [2];
		Piano = audios [3];
		Vocal = audios [4];
        btn = GetComponent<ButtonBehaviour>();
	}*/
}


