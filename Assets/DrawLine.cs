using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour 
{
	public SpriteRenderer PlayerRenderer;

	public LineRenderer line;
	private SpriteRenderer spriteRenderer;
	private bool isMousePressed;
	public List<Vector3> pointsList;
	private Vector3 mousePos;
	private float stepSize;

	public Vector3[,] gridPoints;

	void fillGridPoints ()
	{		
		gridPoints=new Vector3[8,8];
		spriteRenderer = GetComponent<SpriteRenderer> ();
		stepSize = (spriteRenderer.bounds.max.x - spriteRenderer.bounds.min.x) / 8;
		var startX = spriteRenderer.bounds.min.x+stepSize/2;
		var startY = spriteRenderer.bounds.min.y+stepSize/2;
		for (var i=0; i<8; i++)
			for (var j=0; j<8; j++) {

			gridPoints[i,j]=new Vector3(startX+stepSize*i, startY + stepSize*j);
		
		}
	}
	//метод типа main запускается при появлении элемента 
	void Awake()
	{
		// Create line renderer component and set its property
		line = gameObject.AddComponent<LineRenderer>();

		var m = Shader.Find ("Particles/Additive");
		if (m != null) {
			
			line.material =  new Material(m);
		}

		line.SetVertexCount(0);
		line.SetWidth(50f,50f);
		line.SetColors(Color.green, Color.green);
		line.useWorldSpace = true;	
		isMousePressed = false;
		pointsList = new List<Vector3>();

		fillGridPoints ();
	}
	//тупым перебором находим ближайшую клетку  в гриде
	Vector3 GetClosestPoint (Vector3 mousePos)
	{
		foreach(var vector in gridPoints){
			mousePos.z=0;
			var dist=Vector3.Distance(vector , mousePos);
			if(dist<stepSize/3)
				return vector;
		}
		return new Vector3(0,0,0);
	}
	bool isCloseToPlayer(Vector3 mousePos){
		var Vector = PlayerRenderer.bounds.center;
		var dist=Vector3.Distance(Vector , mousePos);
		if(dist<stepSize/3)
			return true;
		return false;
	}
	//находятся ли точки рядом, в пределах размера клетки
	bool IsClose(Vector3 vec1, Vector3 vec2){
		var dist=Vector3.Distance(vec1 , vec2);
		//если хотим убрать хождение по диагонали, то нужно поставить множитель 1.3
		if(dist<stepSize*1.6)
			return true;
		return false;
	}

	//	-----------------------------------	
	void Update () 
	{
		GameObject go = GameObject.Find ("Player");
		MovePlayer movePlayer = go.GetComponent <MovePlayer> ();
		var NeedMove = movePlayer.NeedMove;

		if(Input.GetMouseButtonDown(0) && !NeedMove)
		{
			isMousePressed = true;

		}
		else if(Input.GetMouseButtonUp(0))
		{
			isMousePressed = false;
		}
		// Drawing line when mouse is moving(presses)
		if(isMousePressed)
		{
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var p = GetClosestPoint(mousePos);

			if (!pointsList.Contains (p) && p!=new Vector3(0,0,0)) 
			{
				if(pointsList.Count>0)
				{
					if(!Valid(p,pointsList[pointsList.Count-1]))return;
				}
				else if(!Valid(p,PlayerRenderer.bounds.center))return;
						
				
				if((pointsList.Count==0 && isCloseToPlayer(p))||
				   (pointsList.Count>0 && IsClose(pointsList[pointsList.Count-1], p)))
				{
					pointsList.Add (p);

					line.SetVertexCount (pointsList.Count);				
					line.SetPosition (pointsList.Count - 1, pointsList [pointsList.Count - 1]);
				}
			}
		}
	}
	public static bool Valid(Vector3 dir, Vector3 dir1) {
		var hit = Physics2D.Linecast(dir, dir1);
		return (hit.collider == null);
	}

	//	-----------------------------------	
	//	Following method checks whether given two points are same or not
	//	-----------------------------------	
	private bool checkPoints (Vector3 pointA, Vector3 pointB)
	{
		return (pointA.x == pointB.x && pointA.y == pointB.y);
	}

}