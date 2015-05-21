using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
	public Transform curNode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.UpArrow)){
			transform.Translate(Vector3.up * 0.1f);
		}
		if(Input.GetKeyDown (KeyCode.DownArrow)){
			transform.Translate(Vector3.down * 0.1f);
		}
		if(Input.GetKeyDown (KeyCode.LeftArrow)){
			transform.Translate(Vector3.left * 0.1f);
		}
		if(Input.GetKeyDown (KeyCode.RightArrow)){
			transform.Translate(Vector3.right * 0.1f);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.transform != curNode && !other.transform.tag.Equals ("Unwalkable")){
			curNode = other.transform;
			Debug.Log ("Cur Node is now " + other.transform.name);
		}
	}
}
