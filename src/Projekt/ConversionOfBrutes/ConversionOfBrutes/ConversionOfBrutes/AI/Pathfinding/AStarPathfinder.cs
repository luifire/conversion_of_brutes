/*
 * Author: Pius Meinert 
 * TODO: Maybe use binary heaps or something like this for performance boost. 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.Pathfinding
{   
	[Serializable]
	public sealed class AStarPathfinder
    {
        private readonly List<Node> mNodes;
        private readonly NavigationGraph mNavigationGraph;
        // Uncovered nodes
        readonly List<Node> mOpenNodeList = new List<Node>();
        // Searched nodes
        readonly List<Node> mClosedNodeList = new List<Node>();

		/// <summary>
		/// Constructor
		/// </summary>
        public AStarPathfinder()
        {
			mNavigationGraph = SaveAndLoad.NavigationGraph;
			mNodes = mNavigationGraph.GetNodes();
        }

        private void ResetNodes()
        {
            mOpenNodeList.Clear();
            mClosedNodeList.Clear();
	        foreach (Node node in mNodes)
	        {
		        node.mInOpenNodeList = false;
		        node.mInClosedNodeList = false;
		        node.mDistanceTravelled = float.MaxValue;
		        node.mDistanceToGoal = float.MaxValue;
	        }
        }

        private LinkedList<Vector2> GetPath(Node startNode, Node endNode)
        {
            mClosedNodeList.Add(endNode);

            Node parent = endNode.mParent;
            // Trace back via parents.
            while (parent != startNode)
            {
                mClosedNodeList.Add(parent);
                parent = parent.mParent;
            }
			mClosedNodeList.Add(startNode);

            LinkedList < Vector2 > finalPath = new LinkedList<Vector2>();
            
            // Reverse the path, transform into world space.
            for (int i = mClosedNodeList.Count - 1; i >= 0; i--)
            {
                finalPath.AddLast(new Vector2(mClosedNodeList[i].mPosition.X, mClosedNodeList[i].mPosition.Y));
            }
	        if (finalPath.Count > 2)
	        {
		        return SimplifyPath(finalPath, new Vector2(startNode.mPosition.X, startNode.mPosition.Y));
	        }
	        else
	        {
				finalPath.Remove(new Vector2(startNode.mPosition.X, startNode.mPosition.Y));
		        return finalPath;
	        }
        }

	    private LinkedList<Vector2> SimplifyPath(ICollection<Vector2> path, Vector2 startPosition)
	    {
			LinkedList<Vector2> simplePath = new LinkedList<Vector2>();
		    int i = 1;
		    Vector2 p = path.ElementAt(0);
		    Vector2 n1 = path.ElementAt(i);
		    Vector2 n2 = path.ElementAt(i+1);
			simplePath.AddLast(p);

		    while (true) //n2 != path[path.Count-1]) // TODO: make this a for loop!
		    {
			    if (mNavigationGraph.GetBorderEdges().Any(edge => Intersection(p, n2, edge.Item1, edge.Item2)))
			    {
				    simplePath.AddLast(n1);
				    p = n1;
			    }
				i++;
				if (i == path.Count - 1)
				{
					break;
				}
				n1 = path.ElementAt(i);
				n2 = path.ElementAt(i+1);
		    }
			simplePath.AddLast(n2);
		    bool directLine = true;
		    foreach (Tuple<Vector2, Vector2, float> edge in mNavigationGraph.GetBorderEdges())
		    {
			    if (Intersection(path.ElementAt(0), n2, edge.Item1, edge.Item2))
			    {
				    directLine = false;
			    }

				// end position in forbidden area
			    if (simplePath.Count > 1 && Intersection(path.ElementAt(path.Count-2), n2, edge.Item1, edge.Item2))
			    {
				    simplePath.Remove(n2);

					// go as close as possible to forbidden position
					// http:// www.wikiwand.com/de/Orthogonalprojektion#/Projektion_auf_eine_Gerade
				    Vector2 r0 = edge.Item1;
				    Vector2 x = n2;
				    Vector2 u = edge.Item2 - edge.Item1;
				    Vector2 proj = r0 + ( (x - r0).X * u.X + (x - r0).Y * u.Y ) / (u.X * u.X + u.Y * u.Y) * u;
				    Vector2 d = proj - n2;
					d.Normalize();
					proj = proj + 5 * d;
				    if (IsWaypointValid(proj, simplePath.Last()))
				    {
					    simplePath.AddLast(proj);
						if (!Intersection(simplePath.ElementAt(simplePath.Count - 3), proj, edge.Item1, edge.Item2))
						{
							simplePath.Remove(simplePath.ElementAt(simplePath.Count - 2));
						}
				    }

				    directLine = false;
			    }
		    }
		    if (directLine)
		    {
			    simplePath.Clear();
				simplePath.AddLast(path.ElementAt(0));
				simplePath.AddLast(n2);
		    }
			simplePath.Remove(startPosition);
		    return simplePath;
	    }

		private bool IsWaypointValid(Vector2 v1, Vector2 v2)
		{
			return mNavigationGraph.GetBorderEdges().All(edge => !Intersection(v1, v2, edge.Item1, edge.Item2));
		}

		private Node GetBestNode()
        {
            Node currentBestNode = mOpenNodeList[0];
            float smallestDistanceToGoal = float.MaxValue;
            foreach (Node node in mOpenNodeList)
            {
				// Uncovered node closest to goal
	            if (node.mDistanceToGoal < smallestDistanceToGoal)
	            {
		            currentBestNode = node;
		            smallestDistanceToGoal = currentBestNode.mDistanceToGoal;
	            }
            }
			return currentBestNode;
        }

        /// <summary>
        /// Finds optimal path using A*-Algorithm.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns> A List of 2D vectors marking the optimal path. </returns>
        public LinkedList<Vector2> FindPath(Vector2 startPosition, Vector2 endPosition)
        {
            if (startPosition == endPosition)
            {
				//Console.WriteLine("start = ende");
                return new LinkedList<Vector2>();
            }

			// directLine between start and endPosition, no need for pathfinding
			bool directLine = true;
	        foreach (Tuple<Vector2, Vector2, float> edge in mNavigationGraph.GetBorderEdges())
	        {
		        if (Intersection(startPosition, endPosition, edge.Item1, edge.Item2))
		        {
			        directLine = false;
		        }
	        }
	        if (directLine)
	        {
		        return  new LinkedList<Vector2>(new[] {endPosition});
	        }

	        // clear and reset for new search
            ResetNodes();

	        Node startNode = new Node {mPosition = startPosition};
	        Node endNode = new Node {mPosition = endPosition};
	        //float nearestNodeDistance = (float) Math.Sqrt(Math.Pow(Math.Abs(startPoint.X - endPoint.X), 2) +
			//									   Math.Pow(Math.Abs(startPoint.Y - endPoint.Y), 2));
	        float nearestNodeDistance = float.MaxValue; 
	        Node nearestNode = endNode;
			foreach (Node cand in mNodes)
			{
				float distance = Vector2.Distance(startPosition, cand.mPosition);
				//float distance = (float) Math.Sqrt(Math.Pow(Math.Abs(startPosition.X - cand.mPosition.X), 2) +
				//								   Math.Pow(Math.Abs(startPosition.Y - cand.mPosition.Y), 2));
				if (distance < nearestNodeDistance)
				{
					nearestNodeDistance = distance;
					nearestNode = cand;
				}	
			}
            if (nearestNode == endNode)
            {
				//Console.WriteLine("nearestNode = endNode");
	            return new LinkedList<Vector2>();
            }
	        startNode.mNeighbours = new List<Tuple<Node, float>> {new Tuple<Node, float>(nearestNode, nearestNodeDistance)};
	        nearestNode.mNeighbours.Add(new Tuple<Node, float>(startNode, nearestNodeDistance));
	        nearestNodeDistance = float.MaxValue; 
	        nearestNode = startNode;
			foreach (Node cand in mNodes)
			{
				float distance = Vector2.Distance(endPosition, cand.mPosition);
				//float distance = (float) Math.Sqrt(Math.Pow(Math.Abs(endPosition.X - cand.mPosition.X), 2) +
				//								   Math.Pow(Math.Abs(endPosition.Y - cand.mPosition.Y), 2));
				if (distance < nearestNodeDistance)
				{
					nearestNodeDistance = distance;
					nearestNode = cand;
				}	
			}
	        endNode.mNeighbours = new List<Tuple<Node, float>> {new Tuple<Node, float>(nearestNode, nearestNodeDistance)};
	        nearestNode.mNeighbours.Add(new Tuple<Node, float>(endNode, nearestNodeDistance));

            // startNode
            startNode.mDistanceTravelled = 0;
            startNode.mDistanceToGoal = Vector2.Distance(startPosition, endPosition);
            startNode.mInOpenNodeList = true;
            mOpenNodeList.Add(startNode);

            // while there are nodes not yet searched but uncovered
            while (mOpenNodeList.Count > 0)
            {
                // get node with smallest F value
                Node currentNode = GetBestNode();

                // no path can be found
                if (currentNode == null)
                {
                    break;
                }

                // a path has been found
                if (currentNode == endNode)
                {
                    return GetPath(startNode, endNode);
                }

                // update currentNodes neighbours
                foreach (Tuple<Node, float> neighbourTuple in currentNode.mNeighbours)
                {
	                Node neighbour = neighbourTuple.Item1;
	                float distanceToNeighbour = neighbourTuple.Item2;

	                //if (neighbour == null || neighbour.mIsFree == false)
	                //{
	                //	continue;
	                //}

	                // new G value
	                float distanceTravelled = currentNode.mDistanceTravelled + distanceToNeighbour;
	                // H value
	                float estimatedDistanceToGoal = Vector2.Distance(neighbour.mPosition, endPosition);

	                // if neighbour has not yet been uncovered
	                if (!neighbour.mInOpenNodeList && !neighbour.mInClosedNodeList)
	                {
		                neighbour.mDistanceTravelled = distanceTravelled;
		                neighbour.mDistanceToGoal = distanceTravelled + estimatedDistanceToGoal;
		                neighbour.mParent = currentNode;
		                neighbour.mInOpenNodeList = true;
		                mOpenNodeList.Add(neighbour);
	                }
	                // update uncovered neighbours values if necessary
	                else if (neighbour.mInOpenNodeList || neighbour.mInClosedNodeList)
	                {
		                if (!(neighbour.mDistanceTravelled > distanceTravelled))
		                {
			                continue;
		                }
		                neighbour.mDistanceTravelled = distanceTravelled;
		                neighbour.mDistanceToGoal = distanceTravelled + estimatedDistanceToGoal;
		                neighbour.mParent = currentNode;
	                }
                }
                // currentNode has been searched
                mOpenNodeList.Remove(currentNode);
                currentNode.mInClosedNodeList = true;
                // Show all searched nodes.
                //mClosedNodeList.Add(currentNode);
            }
			//Console.WriteLine("no path found");
            // no path found
            return new LinkedList<Vector2>();
        }

	    private bool Intersection(Vector2 s1, Vector2 s2, Vector2 e1, Vector2 e2)
	    {
		    float ua = (e2.X - e1.X) * (s1.Y - e1.Y) - (e2.Y - e1.Y) * (s1.X - e1.X);
		    float ub = (s2.X - s1.X) * (s1.Y - e1.Y) - (s2.Y - s1.Y) * (s1.X - e1.X);
		    float denominator = (e2.Y - e1.Y) * (s2.X - s1.X) - (e2.X - e1.X) * (s2.Y - s1.Y);

		    if (Math.Abs(denominator) <= 0.00001f)
		    {
			    if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
			    {
				    return true;
			    }
		    }
		    else
		    {
			    ua /= denominator;
			    ub /= denominator;

			    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
			    {
				    return true;
			    }
		    }
		    return false;
	    }
    }
}