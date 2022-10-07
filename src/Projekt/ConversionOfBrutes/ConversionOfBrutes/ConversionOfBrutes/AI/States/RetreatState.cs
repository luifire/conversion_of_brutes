/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.States
{
	[Serializable]
	sealed class RetreatState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private readonly LinkedList<Zone> mZones = new LinkedList<Zone>();
		private readonly Zone mRetreatZone;
		public RetreatState(AiSquad aiSquad, Unit attackedSquadMember)
		{
			mAiSquad = aiSquad;
			mRetreatZone = GameScreen.AiAgent.HomeZone;
			foreach (WorldObject obj in GameScreen.ObjectManager.MapObjects)
			{
				if (obj is Zone &&
				    obj.Fraction == Fraction.Ai)
				{
					mZones.AddFirst((Zone) obj);
				}
			}
			mRetreatZone = GetZone(attackedSquadMember.Position);
			foreach (Unit squadMember in mAiSquad.SquadMembers)
			{
				squadMember.Stop();	
			}
			GameScreen.ObjectManager.ApplyPulkWalk(new LinkedList<WorldObject>(mAiSquad.SquadMembers), 
				(mRetreatZone is HomeZone) ? mRetreatZone.Position - new Vector2(50, 50) : 
				mRetreatZone.Position);
		}

		public override void Update()
		{
			float distanceToRetreatZone = 0;
			foreach (Unit squadMember in mAiSquad.SquadMembers)
			{
				if (!squadMember.IsDead && !squadMember.IsConverted &&
					!(squadMember.DestinationReached && Vector2.Distance(squadMember.Position, mRetreatZone.Position) > squadMember.SightRange))
					//!squadMember.DestinationReached && !squadMember.Attacking)
				{
					distanceToRetreatZone = distanceToRetreatZone + Vector2.Distance(squadMember.Position, mRetreatZone.Position);
				}
			}
			int distance = (mRetreatZone is HomeZone) ? 400 : 250;
			if (mAiSquad.SquadMembers.Count == 0 || distanceToRetreatZone < mAiSquad.SquadMembers.Count * distance)
			{
				List<WorldObject> surrroundingWorldObjects =
					GameScreen.Map.GetObjects(new Rectangle((int) (mRetreatZone.Position.X - 100),
						(int) (mRetreatZone.Position.Y - 100),
						2 * 100,
						2 * 100));
				List<Unit> surrroundingPlayerUnits = new List<Unit>();
				List<Unit> surrroundingAiUnits = new List<Unit>();
				foreach (WorldObject obj in surrroundingWorldObjects)
				{
					if (obj is Unit)
					{
						if (obj.Fraction == Fraction.Ai)
						{
							surrroundingAiUnits.Add((Unit) obj);
						}
						if (obj.Fraction == Fraction.Player)
						{
							surrroundingPlayerUnits.Add((Unit) obj);
						}
					}
				}
				mAiSquad.States.Pop();
				if (surrroundingAiUnits.Count < surrroundingPlayerUnits.Count)
				{
					mAiSquad.States.Push(new AttackState(mAiSquad, surrroundingPlayerUnits.First()));
				}
				if (mAiSquad.SquadMembers.FirstOrDefault(unit => unit.LastAttacker != null && !unit.LastAttacker.IsDead) != null &&
					mAiSquad.SquadMembers.Count > 0 && mRetreatZone is HomeZone)
				{
					mAiSquad.States.Push(new AttackState(mAiSquad,
						mAiSquad.SquadMembers.First(unit => unit.LastAttacker != null && !unit.LastAttacker.IsDead).LastAttacker));
				}
			}
		}
		private Zone GetZone(Vector2 position)
		{
			float distance = Vector2.Distance(position, mZones.First().Position);
			Zone cand = mZones.First();
			foreach (Zone zone in mZones)
			{
				float d2 = Vector2.Distance(position, zone.Position);
				if (d2 < distance)
				{
					distance = d2;
					cand = zone;
				}
			}
			return cand;
		}
	}
}
