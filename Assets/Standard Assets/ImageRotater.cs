using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Globalization;

public class ImageRotater : MonoBehaviour {

	string directoryPath;

	[SerializeField]
	Image screenImage;

	[SerializeField]
	Image cameraImage;

	[SerializeField]
	Text dateText;

	List<Sprite> cameraSprites;
	List<Sprite> screenSprites;
	List<DateTime> creationDates;

	int rotationSecs = 8;
	float startTime;

	[SerializeField]
	private Fadein fadeIn;

	[SerializeField]
	private FadeOut[] fadeOuts;


	int current = 0;

	// Use this for initialization
	void Start () {
		directoryPath = Application.dataPath + "/Data";
		cameraSprites = new List<Sprite>();
		screenSprites = new List<Sprite> ();
		creationDates = new List<DateTime> ();
		startTime = Time.time;

		string[] files = Directory.GetFiles (directoryPath);
		Debug.Log("file count: " + files.Length.ToString());
		Array.Reverse (files);
		foreach (var filestring in files) {
			Debug.Log (filestring);

			if (filestring.Contains ("meta")) {
				continue;
			}

			if (filestring.Contains("_camera.jpg")) {
				cameraSprites.Add(LoadNewSprite(filestring));
				string filename = Path.GetFileNameWithoutExtension (filestring).Replace ("Session ", "").Replace ("_camera", "");
				//Debug.Log (filename);
				CultureInfo provider = CultureInfo.InstalledUICulture;
				DateTime creationDate = DateTime.ParseExact(filename, "yyyy-MM-dd HH-mm-ss-fff", provider);
				creationDates.Add (creationDate);
				//Debug.Log (creationDate.Date);
			} else if (filestring.Contains("_screenshot.jpg")) {
				screenSprites.Add(LoadNewSprite(filestring));
			}


		} 

		//Debug.Log ("cameraSprites: " + cameraSprites.Count.ToString ());
		//Debug.Log ("screenSprites: " + screenSprites.Count.ToString ());
		cameraImage.sprite = cameraSprites [current];
		screenImage.sprite = screenSprites [current];
		dateText.text = String.Format ("{0:dddd, d MMMM, yyyy}", creationDates [current]).ToUpper();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > rotationSecs) {
			current++;
			if (current == cameraSprites.Count || current == screenSprites.Count) {
				current = 0;
			}
			fadeIn.ResetFade ();
			fadeIn.StartFade ();
			foreach (var fadeOut in fadeOuts) {
				fadeOut.ResetFade ();
				fadeOut.StartFade ();
			}
			cameraImage.sprite = cameraSprites [current];
			screenImage.sprite = screenSprites [current];
			dateText.text = String.Format ("{0:dddd, d MMMM, yyyy}", creationDates [current]).ToUpper();
			startTime = Time.time;
		}
	}

	public Texture2D LoadTexture(string FilePath) {
		 
		     // Load a PNG or JPG file from disk to a Texture2D
		     // Returns null if load fails
		 
		     Texture2D Tex2D;
		     byte[] FileData;
		 
		     if (File.Exists(FilePath)){
			       FileData = File.ReadAllBytes(FilePath);
			       Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
			       if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
				         return Tex2D;                 // If data = readable -> return texture
			     }   
		     return null;                     // Return null if load failed
		   }

	public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f) {
		   
	     // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
	     
	     Sprite NewSprite = new Sprite();
	     Texture2D SpriteTexture = LoadTexture(FilePath);
	     NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),new Vector2(0,0), PixelsPerUnit);
		 
	     return NewSprite;
   }
}
