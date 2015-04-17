using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MoveBadGuy : MonoBehaviour {

	private Graph G;
	private Vector3[,] gridPoints;
	// Use this for initialization
	void Start () {
		InicField ();
		InicFieldGraph ();
	}
	
	// Update is called once per frame
	void Update () {
	
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
	            var node = new Vector2D(p.x, p.y, false);
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

        var minI = ic-1;
        var maxI = ic+1;
        var minJ = jc-1;
        var maxJ = jc+1;

        if (minI < 0)
            minI++;
        if (minJ < 0)
            minJ++;
        if (maxI >= gridPoints.GetLength(0)-1)
            maxI--;
        if (maxJ >= gridPoints.GetLength(1) - 1)
            maxJ--;

        for (var i = minI; i <= maxI; i++)
            for (var j = minJ; j <= maxJ; j++)
            {
                var p2 = gridPoints[i, j];
                if (DrawLine.Valid(p1,p2))
                {
                    closePoints.Add(p2);
                }
              
            }
        return closePoints;
    }

}
