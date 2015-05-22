using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
	public Transform[] adjList;

	public bool isWalkable;
	public int row;
	public int column;

	public int g;
	public int h;
	public int totalScore;

	// Use this for initialization
	void Awake () {
		Debug.Log ("Awake");
		adjList = new Transform[4];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void assignScore(int i, Node goalNode){
		g = i;
		h = (Mathf.Abs(row - goalNode.row) + Mathf.Abs (column - goalNode.column));
		totalScore = g+h;
	}
	public void setIsWalkable(bool i){
		isWalkable = i;
	}
}
