using UnityEngine;

public class PointBehavior : MonoBehaviour {

    private Game.GameMode mode = Game.GameMode.None;
    private Color[] colors;
    private GameObject[] hands;

	[SerializeField]
	private Color hitColor;

	[SerializeField]
	private Color targetColor;

    Vector3 viewPos;
    Camera main;
    Color ownCol;
	Vector3 nextPosition;
	Vector3 currentPosition;
	float leftRight = 1;
	float duration = 1.0f;
	float t = 0;
	bool moving = false;

	private float centerX = 0f;
	private float centerY = -5f;
	private float spawnDistance = 5f;
	private float randomPos = 1f;
	private float theta = -1f;
	private float XPos = -1f;
	private float YPos = -1f;

    // Use this for initialization
    void Awake () {
        colors = new Color[4] { Color.red, Color.blue, Color.green, Color.yellow };
        hands = GameObject.FindGameObjectsWithTag("Hand");

        main = Camera.main;
        viewPos = main.WorldToScreenPoint(transform.position);
        viewPos = new Vector3(viewPos.x, viewPos.y, 0f);

		gameObject.transform.position = new Vector3(
			UnityEngine.Random.Range(0f, spawnDistance),
			UnityEngine.Random.Range(-1f, (spawnDistance-1)),
			1f
		);
		currentPosition = gameObject.transform.position;
		viewPos = main.WorldToScreenPoint(transform.position);
    }

	public void updatePosition(bool immediate = false) {
		randomPos = Random.Range (1.0f, 25.0f);
		theta = (randomPos / 25.0f) * (1.0f * Mathf.PI);
		XPos = (centerX + (spawnDistance * Mathf.Cos(theta)));
		YPos = (centerY + (spawnDistance * Mathf.Sin (theta)));
		if (immediate) {
			gameObject.transform.position = new Vector3 (XPos, YPos, -1f);
			currentPosition = gameObject.transform.position;
			viewPos = main.WorldToScreenPoint(transform.position);
		} else {
			nextPosition = new Vector3 (XPos, YPos, -1f);
		}
			
	}


	// Update is called once per frame
	void Update () {
		if(mode == Game.GameMode.None)
        {
            return;
        }

        hands = GameObject.FindGameObjectsWithTag("Hand");

        if (hands.Length == 0) {
            return;
        }

		if (moving) {
			t += Time.deltaTime / duration;
			if (t < 1) {
				gameObject.transform.position = Vector3.Slerp (currentPosition, nextPosition, t);
			}
			if (t > 1) {
				gameObject.GetComponent<Renderer> ().material.SetColor ("_Color", targetColor);
			}
			if (t > 1.5) {
				moving = false;
				t = 0;
				currentPosition = gameObject.transform.position;
				viewPos = main.WorldToScreenPoint(transform.position);
			}
		} else {
			foreach (GameObject hand in hands) {
				Vector3 handPos = main.WorldToScreenPoint (hand.transform.position);

				Color handCol = hand.GetComponent<Track> ().color;

				handPos = new Vector3 (handPos.x, handPos.y, 0f);

				if (mode == Game.GameMode.Simple) {
					//Debug.Log(Vector3.Distance(viewPos, handPos));
					if (Vector3.Distance (viewPos, handPos) < 60f) {
						// Turn the object green
						gameObject.GetComponent<Renderer> ().material.SetColor ("_Color", hitColor);
						// Calculate new location
						nextPosition = new Vector3 (
							Random.Range (0f, (spawnDistance)*leftRight),
							Random.Range (-1f, spawnDistance-1f),
								-1f
							);
						leftRight = leftRight * -1;
						// Move to new location
						moving = true;

						//Destroy(gameObject);
					}
				} else if (mode == Game.GameMode.Complex) {
					if (Vector3.Distance (viewPos, handPos) < 50f && handCol == ownCol) {
						Destroy (gameObject);
					}
				} else if (mode == Game.GameMode.Therapist) {
					//Debug.Log(Vector3.Distance(viewPos, handPos));
					if (Vector3.Distance (viewPos, handPos) < 60f) {
						gameObject.GetComponent<Renderer> ().material.SetColor ("_Color", hitColor);
						updatePosition (false);
						moving = true;
					}
				}
			}
		}
    }

	public void updateSpawnDistance(float cX, float cY, float distance) {
		centerX = cX;
		centerY = cY;
		spawnDistance = distance;
	}

    internal void updateMode(Game.GameMode gm)
    {

        mode = gm;

		if (gm == Game.GameMode.Simple) {
			GetComponent<Renderer> ().material.color = Color.red;
		} else if (gm == Game.GameMode.Complex) {
			GetComponent<Renderer> ().material.color = colors [(int)Random.Range (0, 3.99f)];
		} else if (gm == Game.GameMode.Therapist) {
			updatePosition (true);
		}

        ownCol = GetComponent<Renderer>().material.color;
    }
}
