/**
 * Author: David Luibrand, Julian Löffler
 * 
 * Shield Guard
 * Missing: 
 * - 
 **/

using System;
using System.Collections.Generic;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{   
    [Serializable]
	sealed class ShieldGuard : Unit
    {
	    private readonly int mTauntRange;
		public ShieldGuard(Vector2 position, Fraction fraction, float attackSpeed, int healthPoints, int attackDamage, int attackRange, int speed, int tauntRange) :
			base(Ident.ShieldGuard, position, fraction, attackSpeed, healthPoints, attackDamage, attackRange, speed, 5.5f)
		{
			mTauntRange = tauntRange;
		}

	    public override void Draw3DStuff()
	    {
		    base.Draw3DStuff();
		    if (GameScreen.Hud.TauntToolTip && mIsSelected)
		    {
				GameScreen.GraphicsManager.DrawWorldObject(Ident.ZoneCircle, Position);
		    }
	    }

	    /// <summary>
		/// Taunt Function, all Units surrounding will move to the Casting Shieldguard
		/// </summary>
		public void Taunt()
		{
			Main.Audio.PlayUnitSound(AudioManager.Sound.Taunt, PointPosition);
			var unitsToTaunt = GetUnitsInRange();
			foreach (var unit in unitsToTaunt)
			{
				unit.Attack(this);
			}
		}

		// helper function for the Taunt() Function to calculate Surrounding enemies 
		private IEnumerable<Unit> GetUnitsInRange()
		{
			Rectangle rec = new Rectangle((int)(Position.X - mTauntRange / 2f), (int)(Position.Y - mTauntRange / 2f), mTauntRange, mTauntRange);
			var unitsInRange = new LinkedList<Unit>();

			foreach (var unit in GameScreen.Map.GetObjects(rec))
			{
				if (unit is Unit && unit != this && unit.Fraction != Fraction.Player)
				{
					if (Vector2.Distance(Position, unit.Position) < mTauntRange)
					{
						unitsInRange.AddFirst((Unit)unit);
					}
				}
			}
			
			return unitsInRange;
		}
	}
}
