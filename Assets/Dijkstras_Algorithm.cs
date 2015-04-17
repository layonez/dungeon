using System;
using System.Collections.Generic;

	public class Edge
	{
		private static int EdgeIDSequencer = 0;
		
		#region Properties
		
		public int EdgeID { get; private set; }
		
		public Vector2D PointA { get; private set; }
		
		public Vector2D PointB { get; private set; }
		
		#endregion
		
		public Edge(Vector2D firstPoint, Vector2D secondPoint)
		{
			PointA = firstPoint; PointB = secondPoint;
			EdgeID = ++EdgeIDSequencer;
		}
		
		public Vector2D GetTheOtherVertex(Vector2D baseVertex) {
			if (baseVertex == PointA)
			{
				return PointB;
			}
			else if (baseVertex == PointB)
			{
				return PointA;
			}
			else
			{
				// somehow the base vertex doesn't equal to either point A or B
				return null;
			}
		}
		
		public override string ToString()
		{
			return "Edge ID: " + EdgeID.ToString() 
				+ "; Connected to vertices " + PointA.VertexID ;
		}
	}
	
	public class Vector2D 
	{
		public const int INFINITY = -1;
		private static int VertexIDSequencer = 0;
		
		#region Properties
		
		public int VertexID { get; private set; }
		
		public float AggregateCost { get; set; }
		
		public float XCoord { get; set; }
		
		public float YCoord { get; set; }
		
		public bool Deadend { get; private set; }
		
		public bool Visited { get; set; }
		
		// Internal members
		internal Edge EdgeWithLowestCost { get; set; }
		
		#endregion 
		
		public Vector2D(float x, float y, bool deadend)
		{
			Visited = false;
			XCoord = x;
			YCoord = y;
			Deadend = deadend;
			AggregateCost = INFINITY;
			VertexID = ++VertexIDSequencer;
			EdgeWithLowestCost = null;
		}
		
		public override string ToString()
		{
			return "Vertex ID: " + VertexID;
		}
	}
	
	public class Graph
	{
		private Vector2D _sourceNode;
		private List<Edge> _listOfEdges;
		
		#region Properties
		
		public List<Vector2D> AllNodes { get; private set; }
		
		// Read-Write properties
		public Vector2D SourceVertex
		{
			get {
				return _sourceNode;
			} 
			set
			{
				// SourceVertex is only valid if it is found in the graph.
				// Do not make any changes otherwise.
				foreach (var t in AllNodes)
				{
					if (t == value)
					{
						_sourceNode = value;
						break;
					}
				}
			}
		}
		
		#endregion
		
		public Graph()
		{
			_listOfEdges = new List<Edge>();
			AllNodes = new List<Vector2D>();
			
			_sourceNode = null; //_targetNode = null;
			
			//_totalCost = -1;
			//_optimalTraversal = new List<Vector2D>();
		}

	    public Vector2D getVertex(float x, float y)
	    {
	        foreach (var vector2D in AllNodes)
	        {
                if (Math.Abs(vector2D.XCoord - x) < 0.1 && Math.Abs(vector2D.YCoord - y) < 0.1)
                {
                    return vector2D;
                }
	        }
	        return null;
	    }

		/// <summary>
		/// Adds an edge to the graph.
		/// </summary>
		/// <param name="edge"></param>
		public void AddEdge(Edge edge) {
			_listOfEdges.Add(edge);
			
			// Reset stats due to a change to the graph.
			this.Reset();
		}
		
		/// <summary>
		/// Adds a vertex to the graph.
		/// </summary>
		/// <param name="node"></param>
		public void AddVertex(Vector2D node)
		{
			AllNodes.Add(node);
			
			// Reset stats due to a change to the graph.
			this.Reset();
		}
		
		/// <summary>
		/// As the name suggests, this method calculates the shortest path between the source and target node.
		/// If successful, it updates the TotalCost and the OptimalPath properties with the corresponding values.
		/// </summary>
		/// <returns>Success/Failure</returns>
		public bool CalculateShortestPath()
		{
			var destUnreachable = false;
			
			if (_sourceNode == null) // || _targetNode == null)
			{
				return false;
			}
			
			// Algorithm starts here
			
			// Reset stats
			this.Reset();
			
			// Set the cost on the source node to 0 and flag it as visited
			_sourceNode.AggregateCost = 0;
			
			
			// if the targetnode is not the sourcenode
			// if (_targetNode.AggregateCost == Vector2D.INFINITY) {
			// Start the traversal across the graph
			this.PerformCalculationForAllNodes();
			//}
			
			
			//_totalCost = _targetNode.AggregateCost;
			
			
			if (destUnreachable)
			{
				return false;
			}
			
			return true;
		}
		
		public List<Vector2D> RetrieveShortestPath(Vector2D targetNode)
		{
			var shortestPath = new List<Vector2D>();
			
			if (targetNode == null)
			{
				throw new InvalidOperationException("Target node is null.");
			}
			else
			{
				var currentNode = targetNode;
				
				shortestPath.Add(currentNode);
				
				while (currentNode.EdgeWithLowestCost != null)
				{
					currentNode = currentNode.EdgeWithLowestCost.GetTheOtherVertex(currentNode);
					shortestPath.Add(currentNode);
				}
			}
			
			// reverse the order of the nodes, because we started from target node first
			shortestPath.Reverse();
			
			return shortestPath;
		}
		
		private List<Edge> GetConnectedEdges(Vector2D startNode)
		{
			var connectedEdges = new List<Edge>();
			
			foreach (Edge t in _listOfEdges)
			{
				if (t.GetTheOtherVertex(startNode) != null &&
				    !t.GetTheOtherVertex(startNode).Visited)
				{
					connectedEdges.Add((Edge)t);
				}
			}
			
			return connectedEdges;
		}
		
		
		/// <summary>
		/// Resets the costs from this instance.
		/// </summary>
		private void Reset()
		{
			// reset visited flag and reset the aggregate cost on all nodes
			foreach (Vector2D t in AllNodes)
			{
				// The current node is now considered visited
				t.Visited = false;
				t.AggregateCost = Vector2D.INFINITY;
				t.EdgeWithLowestCost = null;
			}
		}
		
		private List<Vector2D> GetAListOfVisitedNodes()
		{
			var listOfVisitedNodes = new List<Vector2D>();
			
			foreach (var node in AllNodes)
			{
				if (node.Visited) {
					listOfVisitedNodes.Add(node);
				}
			}
			
			return listOfVisitedNodes;
		}
		
		private void PerformCalculationForAllNodes()
		{
			var currentNode = _sourceNode;
			
			// Start by marking the source node as visited
			currentNode.Visited = true;
			
			do
			{
				Vector2D nextBestNode = null;
				
				// Retrieve a list of all visited nodes and for each node, get a list of all edges
				// that are not connected to visited nodes and update the aggregate cost on these other nodes.
				foreach (var visitedNode in this.GetAListOfVisitedNodes())
				{
					foreach (var connectedEdge in this.GetConnectedEdges(visitedNode))
					{
						// Only update if the aggregate cost on the other node is infinite 
						// or is greater and equal to the aggregate cost on the current visited node.
						if (connectedEdge.GetTheOtherVertex(visitedNode).AggregateCost == Vector2D.INFINITY
						    || (visitedNode.AggregateCost) < connectedEdge.GetTheOtherVertex(visitedNode).AggregateCost)
						{
							connectedEdge.GetTheOtherVertex(visitedNode).AggregateCost = visitedNode.AggregateCost;
							
							// update the pointer to the edge with the lowest cost in the other node
							connectedEdge.GetTheOtherVertex(visitedNode).EdgeWithLowestCost = connectedEdge;
						}
						
						
						if (nextBestNode == null || connectedEdge.GetTheOtherVertex(visitedNode).AggregateCost < nextBestNode.AggregateCost)
						{
							nextBestNode = connectedEdge.GetTheOtherVertex(visitedNode);
						}
					}
					
					
				}
				
				// Move the currentNode onto the next optimal node.
				currentNode = nextBestNode;
				
				// Now set the visited property of the current node to true
				currentNode.Visited = true;
				
			} while (this.MoreVisitedNodes()); // Loop until every node's been visited.
		}
		
		private bool MoreVisitedNodes()
		{
			return GetAListOfVisitedNodes().Count < AllNodes.Count;
		}
	}


