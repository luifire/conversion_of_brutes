using System;
using Microsoft.Xna.Framework;

namespace ConversionOfBrutes.GameObjects
{   
	[Serializable]
	class Priest : Unit
	{
		
		public Priest(Ident id,Vector2 position, Fraction fraction, float attackSpeed, int healthPoints,int attackDamage, int attackRange, int speed) :
			base(id, position, fraction, attackSpeed, healthPoints, attackDamage, attackRange, speed, 5.5f)
		{
		}
	}
}
