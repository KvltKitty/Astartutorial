using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	public Transform player;
	public float speed;
	private gridBrain _brain;
	public LayerMask obstacles;
	private bool clearShot;
	private float rayCD = 0.5f;
	private float lastRayCheck;
	public Transform[] path;
	public Transform curNode;
	// Use this for initialization
	void Start () {
		_brain = GameObject.FindGameObjectWithTag("Tiles").GetComponent<gridBrain>();
		clearShot = false;
		lastRayCheck = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(player == null){
			player = _brain.player;
		}
		else{
			if(Time.time - lastRayCheck > rayCD){
				Ray ray = new Ray(transform.position, player.transform.position - transform.position);
				Debug.DrawRay (ray.origin, ray.direction * (Mathf.Abs (Vector3.Distance (transform.position, player.transform.position))), Color.red);
				var rayCastHit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Abs (Vector3.Distance (transform.position, player.transform.position)), obstacles);
				if(rayCastHit){
					//Debug.Log (rayCastHit.transform.tag);
					clearShot = false;
					lastRayCheck = Time.time;
				}
				else{
					//Debug.Log ("Clear Shot");
					clearShot = true;
					lastRayCheck = Time.time;
				}
			}
			if(clearShot){
				transform.position = Vector2.MoveTowards(transform.position, player.position, speed);
			}
			else{
				path = calculatePath ();
			}
		}
	}

	Transform[] calculatePath(){



		Transform[] p = new Transform[1];
		return p;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.transform != curNode && !other.transform.tag.Equals ("Unwalkable")){
			curNode = other.transform;
			Debug.Log ("Cur AI Node is now " + other.transform.name);
		}
	}

}
