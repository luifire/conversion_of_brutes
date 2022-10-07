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
	sealed class DefendState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private int mStartMemberCount;
		private Zone mZoneToProtect;
		private List<Unit> mEnemyUnits = new List<Unit>(); 
		private int mUnitCheck;

		public DefendState(AiSquad aiSquad, Zone zoneToProtect)
		{
			mAiSquad = aiSquad;
			mStartMemberCount = mAiSquad.SquadMembers.Count;
			mZoneToProtect = zoneToProtect;

			UpdateEnemyUnitsInZone();
			GameScreen.ObjectManager.AttackMove(mZoneToProtect.Position, new LinkedList<WorldObject>(mAiSquad.SquadMembers));
		}

		public override void Update()
		{
			bool checkForUnits = mUnitCheck++ > 200;
			if (checkForUnits)
			{
				mUnitCheck = 0;
				UpdateEnemyUnitsInZone();
				GameScreen.ObjectManager.AttackMove(mZoneToProtect.Position, new LinkedList<WorldObject>(mAiSquad.SquadMembers));
			}
			if (mAiSquad.SquadMembers.Count > 0 && mAiSquad.SquadMembers.Count < mStartMemberCount / 2)
			{
				mAiSquad.States.Pop();
				mAiSquad.TargetZone = mZoneToProtect;
				mAiSquad.States.Push(new RetreatState(mAiSquad, mAiSquad.SquadMembers.First()));
			}
			if (mEnemyUnits.Count < 1)
			{
				mAiSquad.States.Pop();
			}
		}

		private void UpdateEnemyUnitsInZone()
		{
			mEnemyUnits.Clear();
			List<WorldObject> surrroundingWorldObjects =
				GameScreen.Map.GetObjects(new Rectangle((int) (mZoneToProtect.Position.X - mZoneToProtect.SightRange),
					(int) (mZoneToProtect.Position.Y - mZoneToProtect.SightRange),
					2 * mZoneToProtect.SightRange,
					2 * mZoneToProtect.SightRange));
			foreach (WorldObject obj in surrroundingWorldObjects.Where(obj => obj is Unit && obj.Fraction == Fraction.Player))
			{
				mEnemyUnits.Add((Unit) obj);
			}
		}
	}
}
