using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MoveBadGuy : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	public SpriteRenderer PlayerRenderer;
	private int _moves=0;
	private Graph G;
	private Vector3[,] gridPoints;
	private Vector3 destination;

    private bool moving = false;

	// Use this for initialization
	void Start () {
		InicField ();
		InicFieldGraph ();
	}	

	bool isCloseToEnemy(Vector3 p){
		var Vector = spriteRenderer.bounds.center;
		var dist=Vector3.Distance(Vector , p);
		if(dist<50)
			return true;
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject go = GameObject.Find ("Player");
		MovePlayer movePlayer = go.GetComponent <MovePlayer> ();
		if (_moves < movePlayer.Moves && moving)
			Move();
	}
	public void ChangeDestination(){
	    if (!moving)
	    {
            foreach (var v in gridPoints)
	        {
	            if (isCloseToPlayer(v))
	            {
	                destination = G.getVertex(v.x, v.y).Vect;
	                break;
	            }
	        }
			foreach (var v in gridPoints)
			{
				if (isCloseToEnemy(v))
				{
					G.SourceVertex = G.getVertex(v.x, v.y);
					break;
				}
			} 
            G.CalculateShortestPath();
            path = G.RetrieveShortestPath(G.getVertex(destination.x, destination.y));
            moving = true;
	    }
	}

    public List<Vector3> path { get; set; }


    private void Move()
    {
		moving = true;
        foreach (var v in gridPoints)
        {
            if (isCloseToEnemy(v))
            {
                G.SourceVertex = G.getVertex(v.x, v.y);
				break;
            }
        } 

        transform.position = Vector3.MoveTowards(transform.position, path[1], 5);
		if (Vector3.Distance(transform.position, path[1]) < 5 || path.Count<=1)
        {
			moving=false;
            _moves++;
        }
    }

	bool isCloseToPlayer(Vector3 mousePos){
		var Vector = PlayerRenderer.bounds.center;
		var dist=Vector3.Distance(Vector , mousePos);
		if(dist<50)
			return true;
		return false;
	}

    void InicField ()
	{
		GameObject go = GameObject.Find ("Stone_Dungeon_Floor");
		DrawLine drawLineObject = go.GetComponent <DrawLine> ();
		gridPoints = drawLineObject.gridPoints;
	}

	void InicFieldGraph ()
	{
	    G = new Graph ();
	    for (var i = 0; i < gridPoints.GetLength(0); i++)
	        for (var j = 0; j < gridPoints.GetLength(1); j++)
	        {
	            var p = gridPoints[i, j];
	            var node = new Vector2D(p);
                G.AddVertex(node);
	            foreach (var closeP in GetClosePoints(p,i,j))
	            {
                    G.AddEdge(new Edge(node, G.getVertex(closeP.x, closeP.y)));
	            }
	        }
	}

    private List<Vector3> GetClosePoints(Vector3 p1, int ic, int jc)
    {
        var closePoints = new List<Vector3>();
        foreach (var point in gridPoints)
        {
            if (DrawLine.IsClose(point, p1) && DrawLine.Valid(point, p1))
            {
                closePoints.Add(point);
            }
        }
               
        return closePoints;
    }

}
