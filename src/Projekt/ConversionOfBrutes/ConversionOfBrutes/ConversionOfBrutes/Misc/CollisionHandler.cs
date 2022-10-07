#if DEBUG
using System;
#endif
using System.Collections.Generic;
using ConversionOfBrutes.GameObjects;
using Microsoft.Xna.Framework;
using RVO;

//Author: luibrand

namespace ConversionOfBrutes.Misc
{
	static class CollisionHandler
	{
		public static void Update(LinkedList<Unit> listOfUnits)
		{
			Simulator.Instance.doStep();
			foreach (var unit in listOfUnits)
			{
				Vector2 pos = Simulator.Instance.getAgentPosition(unit.mCollisionId);
#if DEBUG
				if (float.IsNaN(pos.X) || float.IsNaN(pos.Y))
					throw new Exception("sags Lui #10");
#endif
				
				if (pos != unit.Position)
					unit.ChangePosition(pos);
			}
		}

		public static Vector2 RandomVector2 {
			get
			{ return new Vector2((float) Main.Random.NextDouble(), (float) Main.Random.NextDouble()); } }
		}
}
