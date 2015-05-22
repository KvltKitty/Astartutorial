using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : MonoBehaviour {
	public Transform player;
	public float speed;
	private gridBrain _brain;
	public LayerMask obstacles;
	private bool clearShot;
	private float rayCD = 0.5f;
	private float lastRayCheck;
	public List<Node> path;
	private Transform lastPlayerNode;
	public Transform curNode;
	private Transform playerNode;
	private Transform curNodeCheck;

	private Node _node;
	private List<Node> openList;
	private List<Node> closedList;
	// Use this for initialization
	void Start () {
		_brain = GameObject.FindGameObjectWithTag("Tiles").GetComponent<gridBrain>();
		path = new List<Node>();
		openList = new List<Node>();
		closedList = new List<Node>();
		clearShot = false;
		lastRayCheck = -100.0f;
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
				if(path != null){
					resetColors (path);
				}
				path = calculatePath ();
				setColors (path);
				moveAlongPath(path);
			}
		}
	}

	private void resetColors(List<Node> path){
		foreach(Node obj in path){
			Color temp = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			obj.gameObject.GetComponent<SpriteRenderer>().color = temp;
		}
	}

	private void setColors(List<Node> path){
		foreach(Node obj in path){
			Color temp = new Color(0.0f, 0.0f, 1.0f, 1.0f);
			obj.gameObject.GetComponent<SpriteRenderer>().color = temp;
		}
	}

	private void moveAlongPath(List<Node> Path){
		if(Mathf.Abs (Vector3.Distance (transform.position, path[0].transform.position)) <= 0.01){
			path.RemoveAt(0);
		}
		transform.position = Vector2.MoveTowards (transform.position, path[0].transform.position, speed);

	}

	private List<Node> calculatePath(){
		playerNode = _brain.getPlayerNode();
		if(playerNode == lastPlayerNode){
			return path;
		}
		lastPlayerNode = playerNode;
		path.Clear();
		openList.Clear ();
		closedList.Clear ();
		curNodeCheck = curNode;
		int i = 0;
		while(curNodeCheck != playerNode){
			i++;
			_node = curNodeCheck.GetComponent<Node>();
			Transform[] curAdjList = _node.adjList;
			for(int j = 0; j < curAdjList.Length; j++){
				if(curAdjList[j] != null){
					if(closedList.Exists(x=>x.transform == curAdjList[j].gameObject.GetComponent<Node>().transform)){
						//if in closed list, ignore
					}
					else if(!openList.Exists (x=>x.transform == curAdjList[j].gameObject.GetComponent<Node>().transform)){
						curAdjList[j].gameObject.GetComponent<Node>().assignScore(i, playerNode.gameObject.GetComponent<Node>());
						if(curAdjList[j].gameObject.GetComponent<Node>().isWalkable){
							openList.Add (curAdjList[j].gameObject.GetComponent<Node>());
						}
					}
					else if(openList.Exists (x=>x.transform == curAdjList[j].gameObject.GetComponent<Node>().transform)){
						int temp = curAdjList[j].gameObject.GetComponent<Node>().totalScore;
						if((i + curAdjList[j].gameObject.GetComponent<Node>().h) < temp){
							curAdjList[j].gameObject.GetComponent<Node>().assignScore(i, playerNode.gameObject.GetComponent<Node>());
						}
					}
				}
			}
			//sort the open list
			openList = openList.OrderBy(o=>o.totalScore).ToList ();
			curNodeCheck = openList[0].transform;
			closedList.Add (openList[0]);
			openList.RemoveAt (0);

		}
		int n = 0;
		foreach(Node obj in closedList){
			Debug.Log (n + " " + obj.transform.name + " " + obj.totalScore);
			n++;
		}

		return closedList;
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.transform != curNode && !other.transform.tag.Equals ("Unwalkable")){
			curNode = other.transform;
			Debug.Log ("Cur AI Node is now " + other.transform.name);
		}
	}

}
