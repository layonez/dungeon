using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovePlayer : MonoBehaviour {

	public int Moves = 0;
	public float speed = 1000;
	public List<Vector3> pointsList;
	public bool NeedMove=false;
	private LineRenderer line;
	
	void Start () {
		GameObject go = GameObject.Find ("Stone_Dungeon_Floor");
		DrawLine drawLineObject = go.GetComponent <DrawLine> ();
		pointsList = drawLineObject.pointsList;
		line = drawLineObject.line;
	}
	
	void Update () {
		if (NeedMove && pointsList.Count > 1) {
			var newPos= new Vector3(pointsList [1].x, pointsList [1].y);
			transform.position = Vector3.MoveTowards (transform.position, newPos, 5);
			if (Vector3.Distance (transform.position, pointsList[1]) < 1) {
				pointsList.Remove (pointsList [0]);
				Moves++;
			}
		}
		if (NeedMove && pointsList.Count < 2) {
			NeedMove = false;
			line.SetVertexCount(0);
			pointsList.RemoveRange(0,pointsList.Count);
		}
	}


	public void reciveMes(){
		NeedMove = true;
	}

 }
