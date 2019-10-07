using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityEngine.SceneManagement;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject palet;
    public bool doublepalette;
    
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	[SerializeField]
    private BodySourceManager _BodyManager;

	[SerializeField]
	private float movementDamp = 0.6f;
   
	[SerializeField]
	private Color[] handColors;

	[SerializeField]
	private Paint painter;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        /*{ Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },*/
        
        /*{ Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },*/
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    
    void FixedUpdate () 
    {       
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
          	}
				                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
		body.tag = "HideForScreenshot";
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
			
			GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube); //  new GameObject ();

            jointObj.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;

			jointObj.transform.position = new Vector3 (jointObj.transform.position.x, jointObj.transform.position.y, 1f);

			LineRenderer lr = jointObj.AddComponent<LineRenderer>();
			lr.SetVertexCount(2);
			lr.material = BoneMaterial;

			lr.numCapVertices = 10;
			lr.numCornerVertices = 10;
			if (jt == Kinect.JointType.SpineBase) {
				lr.startWidth = 3f;
				lr.endWidth = 3.2f;
			} else if (jt == Kinect.JointType.Neck) {
				lr.startWidth = 1f;
				lr.endWidth = 1f;
			} else if (	jt == Kinect.JointType.ThumbRight || jt == Kinect.JointType.ThumbLeft || jt == Kinect.JointType.HandTipRight || jt == Kinect.JointType.HandTipLeft) {
				lr.startWidth = 0f;
				lr.endWidth = 0f;
			} else {
				lr.startWidth = 0.5f;
				lr.endWidth = 0.5f;
			}

            if (jt == Windows.Kinect.JointType.HandLeft || jt == Windows.Kinect.JointType.HandRight)
            {
                jointObj.tag = "Hand";
                jointObj.AddComponent<Track>();
				int ranCol = UnityEngine.Random.Range(0, handColors.Length);
				jointObj.GetComponent<Track>().color = handColors [ranCol];
				var newMat = new Material (Shader.Find("Unlit/Color"));
				newMat.color = handColors[ranCol];
				lr.material = newMat;
                /*if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Menu"))
                {
                    jointObj.AddComponent<PressButtonsWithHands>();
                }*/

            } /*else if (jt == Kinect.JointType.SpineBase && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Project Scene"))
            {
               
				if(doublepalette)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject pallette = Instantiate(palet, new Vector3(jointObj.transform.position.x, jointObj.transform.position.y, jointObj.transform.position.z), Quaternion.identity);
                        pallette.transform.parent = body.transform;
                        pallette.transform.rotation = Quaternion.Euler(90f, 0f, 5f * (i * 2 - 1));
                        //pallette.transform.localScale = new Vector3(2f, 2f, 1f);
                        //pallette.GetComponent<Activate>().side = i * 2 - 1;
                    }
                } else
                {
                    GameObject pallette = Instantiate(palet, new Vector3(jointObj.transform.position.x, jointObj.transform.position.y + 1f, jointObj.transform.position.z), Quaternion.identity);
                    pallette.transform.parent = body.transform;
                }
                
                
            }*/

        }
        
        //TODO: Fix spawn position
        
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());

			LineRenderer lr = jointObj.GetComponent<LineRenderer>();
			if (lr == null) {
				continue;
			}

			var lerp_pos = Vector3.Lerp( GetVector3FromJoint (sourceJoint), jointObj.localPosition, movementDamp);
			jointObj.localPosition = lerp_pos;//new Vector3 (lerp_pos.x, lerp_pos.y, 1f); //GetVector3FromJoint(sourceJoint);
			//jointObj.transform.position = new Vector3 (jointObj.transform.position.x, jointObj.transform.position.y, 1f);

            if(targetJoint.HasValue)
            {
				lr.SetPosition(0, new Vector3(jointObj.localPosition.x, jointObj.localPosition.y, 1f));
				var pos = Vector2.Lerp((Vector2)GetVector3FromJoint (targetJoint.Value), lr.GetPosition(1), movementDamp);
				lr.SetPosition(1, new Vector3(pos.x, pos.y, 1f));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            } else {
                lr.enabled = false;
            }

			/*if (jt == Windows.Kinect.JointType.HandLeft || jt == Windows.Kinect.JointType.HandRight) {
				Debug.Log (jointObj.GetComponent<Track>());
				painter.applyPaint (jointObj.GetComponent<Track> ());
			}*/
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }

    public Dictionary<ulong, GameObject> getDict()
    {
        return _Bodies;
    }

    public int amountOfPlayers()
    {
        return _Bodies.Count;
    }
}
