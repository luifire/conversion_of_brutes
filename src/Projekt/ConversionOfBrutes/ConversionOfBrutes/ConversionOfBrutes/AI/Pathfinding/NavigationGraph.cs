/*
 * Author: Pius Meinert
 * TODO: Delete mBorderEdges: It's a relict from an old save, not used any longer but only deletable if completeley new NavGraph is created... 
 * */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.Pathfinding
{
	/// <summary>
	/// Navigation Graph. Stores nodes and their neighbours for pathfinding purposes.
	/// </summary>
	[Serializable]
	public sealed class NavigationGraph 
	{
		private List<Node> mNodes;
		private List<Tuple<Vector2, Vector2, float>> mBorderEdges2;

		/// <summary>
		/// Constructor
		/// </summary>
		public NavigationGraph()
		{
			mNodes = new List<Node>(); 
			mBorderEdges2 = new List<Tuple<Vector2, Vector2, float>>(); 
		}

		/// <summary>
		/// Get all nodes stores in the graph.
		/// </summary>
		/// <returns></returns>
		public List<Node> GetNodes()
		{
			return mNodes;
		}

		/// <summary>
		/// Replace/set all nodes in the graph.
		/// </summary>
		/// <param name="nodes"></param>
		public void SetNodes(List<Node> nodes)
		{
			mNodes = nodes;
		}

		/// <summary>
		/// Replace/set all edges that mark an unwalkable area.
		/// </summary>
		/// <param name="borderEdges"></param>
		public void SetBorderEdges(List<Tuple<Vector2, Vector2, float>> borderEdges)
		{
			mBorderEdges2 = borderEdges;
		}

		/// <summary>
		/// Get all edges that mark an unwalkable area.
		/// </summary>
		/// <returns></returns>
		public List<Tuple<Vector2, Vector2, float>> GetBorderEdges()
		{
			return mBorderEdges2;
		} 

	}
}