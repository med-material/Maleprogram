using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GamemodeSaveBehavior : MonoBehaviour {

    [SerializeField]
    private int music = 0;

    [SerializeField]
    private Game.GameMode gameMode = Game.GameMode.None;

    [SerializeField]
    private Texture2D template;

	[SerializeField]
	private AudioClip[] gameMusic;

	private AudioSource audioSource;
	private int currentAudio = 0;
	private bool shouldAudio = true;

    GameObject canvas;
	GameObject pictureTemplate;

	private float gameInactiveTime;
	private int gameStandbyTime = 1;
	private bool isRecording = false;
	private Process obs;

	public static GamemodeSaveBehavior Instance { get; private set; }

    // Use this for initialization
    void Awake () {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			return;
		}

		DontDestroyOnLoad(gameObject);

		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);
		}

		if (!isRecording) {
			StartRecording ();
		}
		gameInactiveTime = Time.time;

		audioSource = this.GetComponent<AudioSource> ();
		currentAudio = Random.Range (0, gameMusic.Length);
		audioSource.clip = gameMusic[currentAudio];
		audioSource.volume = 1.0f;
		audioSource.Play ();

	}

	public void OnApplicationQuit() {
		if (Instance != this) {
			return;
		}
		if (isRecording) {
			KillRecording ();
		}
	}

	public void StartRecording() {
		if (isRecording) {
			return;
		}
		isRecording = true;

		foreach (var process in Process.GetProcessesByName("obs64"))
		{
			process.Kill();
		}

		obs = new Process();
		obs.StartInfo.FileName = "C:\\Program Files\\obs-studio\\bin\\64bit\\obs64.exe";
		obs.StartInfo.Arguments = "--startrecording --minimize-to-tray";
		obs.StartInfo.WorkingDirectory = "C:\\Program Files\\obs-studio\\bin\\64bit";
		obs.StartInfo.CreateNoWindow = true;
		obs.StartInfo.UseShellExecute = false;
		obs.Start();
	}

	public void KillRecording() {
		if (!isRecording) {
			return;
		}
		isRecording = false;

		obs.Kill ();
	}

    private void OnEnable()
    {
		if (Instance != this) {
			return;
		}
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update () {
		if (Instance != this) {
			return;
		}

		if (audioSource.isPlaying && !shouldAudio) {
			if (audioSource.volume <= 0f) {
				audioSource.Stop ();
			}
			audioSource.volume -= 0.01f;
		}

		if (!audioSource.isPlaying && shouldAudio) {
			currentAudio++;
			if (currentAudio >= gameMusic.Length) {
				currentAudio = 0;
			}
			audioSource.clip = gameMusic[currentAudio];
			audioSource.volume = 1.0f;
			audioSource.Play ();
		}
	}

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		if (Instance != this) {
			return;
		}
		if (scene.name == "Project Scene") {
			shouldAudio = true;
			canvas = GameObject.Find ("Canvas");
			pictureTemplate = GameObject.Find ("PictureTemplate");
			canvas.GetComponent<Game> ().changeGameMode (gameMode);
			//canvas.GetComponent<paint> ().original;
			if (template == null) {
				pictureTemplate.SetActive (false);
			} else {
				pictureTemplate.GetComponent<Renderer> ().material.mainTexture = template;
			}

		} else if (scene.name == "Screensaver") {
			shouldAudio = false;
			KillRecording ();
		} else if (scene.name == "Menu") {
			shouldAudio = true;
			StartRecording ();
		}

    }

    public void setGameMode(Game.GameMode mode)
    {
        gameMode = mode;
    }

	public Game.GameMode getGameMode()
	{
		return gameMode;
	}

    public void setTemplate(Texture2D tex)
    {
        template = tex;
    }

    public void setMusic(int i)
    {
        music = i;
    }
}
