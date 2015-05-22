using UnityEngine;
using System.Collections;

//should handle all calculations
public class gridBrain : MonoBehaviour {
	private Transform[] curNode;
	public Transform player;
	public Transform playerNode;
	private player _player;

	void Start () {
		curNode = new Transform[transform.childCount];
		for(int i = 0; i < transform.childCount; i++){
			curNode[i] = transform.GetChild(i);
		}
		foreach(Transform obj in curNode)
		{
			Node node = obj.GetComponent<Node>();
			if(obj.tag.Equals("Unwalkable")){
				node.setIsWalkable(false);
			}
			else{
				node.setIsWalkable(true);
			}
			int row = node.row;
			int column = node.column;
			//Debug.Log (row + " " + column);
			if(row - 1 >= 0)
			{
				node.adjList[0] = transform.FindChild((row - 1).ToString() + "_" + column.ToString());
			}
			if(row + 1 <= 6)
			{
				node.adjList[1] = transform.FindChild((row + 1).ToString() + "_" + column.ToString());
			}
			if(column - 1 >= 0)
			{
				node.adjList[2] = transform.FindChild(row.ToString() + "_" + (column - 1).ToString());
            }
			if(column + 1 <= 10)
			{
				node.adjList[3] = transform.FindChild(row.ToString() + "_" + (column + 1).ToString());
			}
			
		}
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		_player = player.GetComponent<player>();

	}
	

	void Update () {
		if(_player.curNode != playerNode){
			playerNode = _player.curNode;
		}
	}

	public Transform getPlayerNode(){
		return _player.curNode;
	}
}
