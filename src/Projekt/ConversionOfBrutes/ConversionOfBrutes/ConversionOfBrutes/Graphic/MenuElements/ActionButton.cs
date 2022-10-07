using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	class ActionButton
	{
		private const int ButtonSize = 219;

		private Texture2D mSelectionButtons;
		private SpriteBatch mSpriteBatch;
		private Rectangle mDestRect;
		private Rectangle mSourceRect;
		private ActionIdent mActionIdent;
		private float mHoverTime;
		private string mToolTip;

		public enum ActionIdent
		{
			Attack,
			Move,
			AttackMove,
			Patrol,
			Stop,
			Taunt,
			SpawnPriest,
			SpawnShieldguard,
			SpawnRangedPriest,
			SpawnEliteAtlantican,
			SpawnWitch,
			SpawnBeast
		}
		public ActionButton(Texture2D buttons, SpriteBatch spriteBatch, ActionIdent actionIdent)
		{
			mSelectionButtons = buttons;
			mSpriteBatch = spriteBatch;
			mActionIdent = actionIdent;
			mHoverTime = 0;

			Ident spawnUnit = Ident.Mountain;		// Needs dummy value because "null" not possible
			switch(mActionIdent)
			{
				case ActionIdent.Attack:
					mSourceRect = new Rectangle(2*ButtonSize, 0, ButtonSize, ButtonSize);
					mToolTip = "Attack. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.Attack) + ") \nAttack a hostile unit";
					break;
				case ActionIdent.AttackMove:
					mSourceRect = new Rectangle(3*ButtonSize, 0, ButtonSize, ButtonSize);
					mToolTip = "Attackmove. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.AttackMove) + ") \nMove to a certain position\nand attack every enemy on\nyour way";
					break;
				case ActionIdent.Move:
					mSourceRect = new Rectangle(4*ButtonSize, 0, ButtonSize, ButtonSize);
					mToolTip = "Move. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.Move) + ") \nMove to a certain position";
					break;
				case ActionIdent.Patrol:
					mSourceRect = new Rectangle(0, 0, ButtonSize, ButtonSize);
					mToolTip = "Patrol. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.Patrol) + ") \nPatrol between two points";
					break;
				case ActionIdent.Stop:
					mSourceRect = new Rectangle(ButtonSize, 0, ButtonSize, ButtonSize);
					mToolTip = "Stop. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.Stop) + ") \nStops all actions";
					break;
				case ActionIdent.Taunt:
					mSourceRect = new Rectangle(ButtonSize, 3*ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Taunt. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.Taunt) + ") \nTaunt all nearby enemies in\norder to protect your priests";
					break;
				case ActionIdent.SpawnPriest:
					mSourceRect = new Rectangle(0, ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn priest. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnPriest) + ") \nCan convert enemies\n at close range";
					spawnUnit = Ident.Priest;
					break;
				case ActionIdent.SpawnShieldguard:
					mSourceRect = new Rectangle(ButtonSize, ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn shield guard. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnShieldGuard) + ") \nCan taunt enemies to\nprotect your priests";
					spawnUnit = Ident.ShieldGuard;
					break;
				case ActionIdent.SpawnRangedPriest:
					mSourceRect = new Rectangle(0, 2*ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn ranged priest. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnRangedPriest) + ") \nCan convert enemies from\ndistance";
					spawnUnit = Ident.PriestRanged;
					break;
				case ActionIdent.SpawnEliteAtlantican:
					mSourceRect = new Rectangle(ButtonSize, 2*ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn elite atlantican. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnEliteAtlantican) + ") \nElite warrior with high\nhealth and damage";
					spawnUnit = Ident.EliteAtlantic;
					break;
				case ActionIdent.SpawnWitch:
					mSourceRect = new Rectangle(3 * ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn Witch. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnWitch) + ") \nCan convert enemies from\ndistance\nhigh conversion damage";
					spawnUnit = Ident.Witch;
					break;
				case ActionIdent.SpawnBeast:
					mSourceRect = new Rectangle(4*ButtonSize, 3 * ButtonSize, ButtonSize, ButtonSize);
					mToolTip = "Spawn Beast. (" + Main.HotKey.GetHotkey(HotKeys.HotKey.SpawnBeast) + ") \nMelee Unit \nHigh damage but slow";
					spawnUnit = Ident.Beast;
					break;
			}

			if (spawnUnit != Ident.Mountain)
			{
				mToolTip += "\n" + "Costs: " + GameScreen.GameLogic.GetWinPointCost(spawnUnit) + " WP";
				mToolTip += " / " + GameScreen.GameLogic.GetTimeCost(spawnUnit) + " sec";
			}
		}

		public void Draw(bool activated)
		{
			mSpriteBatch.Draw(mSelectionButtons, mDestRect, mSourceRect, activated ? Color.Red : Color.White);
		}

		public Rectangle DestRect{set { mDestRect = value; } get{return mDestRect;}}
		public ActionIdent Action { get { return mActionIdent; } }
		public float HoverTime { get { return mHoverTime; } set { mHoverTime = value; } }
		public string ToolTip { get { return mToolTip; } }
	}
}
