/*
 * Author: Pius Meinert 
 * ? 
 * */

using System;
using ConversionOfBrutes.GameObjects;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.AI.States
{
	[Serializable]
	sealed class AttackState : BaseState
	{
		private readonly AiSquad mAiSquad;
		private int mDummyTimer;
		private readonly Unit mTargetUnit;

		public AttackState(AiSquad aiSquad, Unit targetUnit)
		{
			mAiSquad = aiSquad;
			mTargetUnit = targetUnit;
		}

		public override void Update()
		{
			if (mTargetUnit.IsDead || mDummyTimer++ > 50)
			{
				mDummyTimer = 0;
				mAiSquad.States.Pop();
			}
			else
			{
				foreach (Unit squadMember in mAiSquad.SquadMembers)
				{
					if ((squadMember.Ident == Ident.Archer || squadMember.Ident == Ident.ArcherMounted) &&
					    squadMember.Attacking &&
					    Vector2.Distance(squadMember.TargetUnit.Position, squadMember.Position) < squadMember.AttackRange - 10)
					{
						Vector2 kiteDirection = squadMember.Position - squadMember.TargetUnit.Position;
						kiteDirection.Normalize();
						squadMember.WalkToPosition(kiteDirection + squadMember.Position, 0);
					}
					if (!squadMember.Attacking && !squadMember.IsInAttackMove)
					{
						squadMember.Attack(mTargetUnit);
					}
					else if (squadMember.Attacking && !squadMember.IsInAttackMove &&
					         squadMember.LastAttack + TimeSpan.FromMilliseconds(3 * 1000 / squadMember.AttackSpeed) <
					         Main.GameTime.TotalGameTime)
					{
						//Console.WriteLine("asd");
						//squadMember.SightRange = 20;
						squadMember.Attacking = false;
						squadMember.AttackMove(squadMember.Position);
					}
				}
			}
		}
	}
}
