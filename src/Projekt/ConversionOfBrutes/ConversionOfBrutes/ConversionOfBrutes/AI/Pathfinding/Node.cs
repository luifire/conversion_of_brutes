/*
 * Author: Pius Meinert
 * TODO: ?
 * */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.Pathfinding
{
	/// <summary>
	/// Part of the navigation graph for pathfinding purposes.
	/// </summary>
	[Serializable]
	public sealed class Node
	{
		internal Vector2 mPosition;
		internal List<Tuple<Node, float>> mNeighbours;
		internal Node mParent;
		internal bool mInOpenNodeList;
		internal bool mInClosedNodeList;
		// G
		internal float mDistanceTravelled;
		// F
		internal float mDistanceToGoal;
	}
}