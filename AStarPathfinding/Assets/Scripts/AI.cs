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
		if(Input.GetKeyDown (KeyCode.KeypadEnter)){
			printCurPath();
		}
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
				if(path != null){
				setColors (path);
				moveAlongPath(path);
				}
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
			Color temp = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			path[0].gameObject.GetComponent<SpriteRenderer>().color = temp;
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
		Debug.Log ("Cur Node Pathfinding is " + curNode.transform.name);
		curNodeCheck.gameObject.GetComponent<Node>().assignScore (0, playerNode.gameObject.GetComponent<Node>());
		curNodeCheck.gameObject.GetComponent<Node>().parent = null;
		openList.Add (curNodeCheck.gameObject.GetComponent<Node>());
		while(openList.Any ()){
			curNodeCheck = getLowest (openList);
			if(curNodeCheck == playerNode){
				return constructPath ();
			}
			openList.Remove (curNodeCheck.gameObject.GetComponent<Node>());
			closedList.Add (curNodeCheck.gameObject.GetComponent<Node>());
			Transform[] adjList = curNodeCheck.gameObject.GetComponent<Node>().adjList;
			foreach(Transform obj in adjList){
				if(closedList.Exists (x=>x.transform == obj)){
					continue;
				}
				if(!obj.gameObject.GetComponent<Node>().isWalkable){
					continue;
				}
				int tenativeG = curNode.gameObject.GetComponent<Node>().g + 1;

				if(!openList.Exists (x=>x.transform == obj) || tenativeG < obj.gameObject.GetComponent<Node>().g){
					obj.gameObject.GetComponent<Node>().parent = curNodeCheck.gameObject.GetComponent<Node>();
					obj.gameObject.GetComponent<Node>().assignScore(tenativeG, playerNode.gameObject.GetComponent<Node>());
					if(!openList.Exists (x=>x.transform == obj)){
						openList.Add (obj.gameObject.GetComponent<Node>());
					}
				}
			}

		}


		return closedList;
	}

	private Transform getLowest(List<Node> openList){
		//openList = openList.OrderBy(o=>o.totalScore).ToList ();
		int lowest = 999;
		Transform temp = openList[0].transform;
		foreach(Node obj in openList){
			if(obj.totalScore <= lowest){
				lowest = obj.totalScore;
				temp = obj.transform;
			}
		}

		return temp;
	}

	private List<Node> constructPath(){
		Node temp = playerNode.gameObject.GetComponent<Node>();
		Debug.Log ("Construct Path");
		while(temp.parent != null){
			path.Add (temp);
			temp = temp.parent;
		}
		/*for(int i = 0; i < 7; i++){
			path.Add (temp);
			temp = temp.parent;
			Debug.Log (temp.parent.name);
		}*/
		path.Reverse();
		return path;
	}

	private void printCurPath(){
		if(path != null){
			Debug.Log ("Path");
			foreach(Node obj in path){
				Debug.Log(obj.transform.name + " G: " + obj.g + " H: " + obj.h);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.transform != curNode && !other.transform.tag.Equals ("Unwalkable")){
			curNode = other.transform;
			Debug.Log ("Cur AI Node is now " + other.transform.name);
		}
	}

}
