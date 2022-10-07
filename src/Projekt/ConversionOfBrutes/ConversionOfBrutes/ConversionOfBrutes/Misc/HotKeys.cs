/**
 * author: Julian Löffler 
 * 
 * Concrete Class to manage Hotkeys
 * Usage: This class holds all Hotkeys of all Actions that can be made.
 * 
 **/

using System;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Misc
{
	public class HotKeys
	{
		private Keys[] mHotKeys;

		public enum HotKey
		{
			CenterCamera,
			CameraUp,
			CameraLeft,
			CameraDown,
			CameraRight,
			CameraZoomIn,
			CameraZoomOut,
			ShowGameMenu,
			AttackMove,
			Move,
			Stop,
			Patrol,
			Taunt,
			Attack,
			SpawnShieldGuard,
			SpawnPriest,
			SpawnRangedPriest,
			SpawnEliteAtlantican,
			SpawnWitch,
			SpawnBeast,
			Group0,
			Group1,
			Group2,
			Group3,
			Group4,
			Group5,
			Group6,
			Group7,
			Group8,
			Group9,// t. b. c. 
			// just for the Editor, does not necessarily have to be changable
			SwitchMode, 
			EditorMove,
			EditorDelete,
			EditorAlt,
			EditorSafety,
			Zone,
			SpawnZone,
			Tree,
			MountainSmall,
			Mountain,
			MountainBig,
			Pond,
			HomeZonePlayer,
			HomeZoneAi,
			HideObjects
		}
		public HotKeys()
		{
			mHotKeys = new Keys[Enum.GetNames(typeof(HotKey)).Length];
			InitHotkeysForGame();
		}

		public void InitHotkeysForGame()
		{
			//Player Keys
			mHotKeys[(int)HotKey.CenterCamera] = Keys.Space;
			mHotKeys[(int)HotKey.CameraUp] = Keys.W;
			mHotKeys[(int)HotKey.CameraLeft] = Keys.A;
			mHotKeys[(int)HotKey.CameraDown] = Keys.S;
			mHotKeys[(int)HotKey.CameraRight] = Keys.D;
			mHotKeys[(int)HotKey.CameraZoomIn] = Keys.Q;
			mHotKeys[(int)HotKey.CameraZoomOut] = Keys.E;
			mHotKeys[(int)HotKey.ShowGameMenu] = Keys.Escape;
			
			// Unit keys
			mHotKeys[(int)HotKey.AttackMove] = Keys.R;
			mHotKeys[(int)HotKey.Patrol] = Keys.F;
			mHotKeys[(int)HotKey.Taunt] = Keys.T;
			mHotKeys[(int)HotKey.Attack] = Keys.G;
			mHotKeys[(int)HotKey.Move] = Keys.Z;
			mHotKeys[(int)HotKey.Stop] = Keys.V;
			
			//Grouping Keys
			mHotKeys[(int)HotKey.Group0] = Keys.D0;
			mHotKeys[(int)HotKey.Group1] = Keys.D1;
			mHotKeys[(int)HotKey.Group2] = Keys.D2;
			mHotKeys[(int)HotKey.Group3] = Keys.D3;
			mHotKeys[(int)HotKey.Group4] = Keys.D4;
			mHotKeys[(int)HotKey.Group5] = Keys.D5;
			mHotKeys[(int)HotKey.Group6] = Keys.D6;
			mHotKeys[(int)HotKey.Group7] = Keys.D7;
			mHotKeys[(int)HotKey.Group8] = Keys.D8;
			mHotKeys[(int)HotKey.Group9] = Keys.D9;

			
			// Spawnzone
			mHotKeys[(int)HotKey.SpawnShieldGuard] = Keys.X;
			mHotKeys[(int)HotKey.SpawnPriest] = Keys.Y;
			mHotKeys[(int)HotKey.SpawnRangedPriest] = Keys.C;
			mHotKeys[(int)HotKey.SpawnEliteAtlantican] = Keys.V;
			mHotKeys[(int)HotKey.SpawnWitch] = Keys.N;
			mHotKeys[(int)HotKey.SpawnBeast] = Keys.B;
			
		}

		public void ChangeToMapEditorMapping()
		{
			for(int i=0; i<mHotKeys.Length; i++)
			{
				mHotKeys[i] = Keys.F24;
			}
			mHotKeys[(int) HotKey.SwitchMode] = Keys.Tab;
			mHotKeys[(int) HotKey.EditorMove] = Keys.LeftShift;
			mHotKeys[(int) HotKey.EditorDelete] = Keys.LeftControl;
			mHotKeys[(int) HotKey.EditorAlt] = Keys.LeftAlt;
			mHotKeys[(int) HotKey.Zone] = Keys.D1;
			mHotKeys[(int) HotKey.SpawnZone] = Keys.D2;
			mHotKeys[(int) HotKey.Tree] = Keys.D3;
			mHotKeys[(int) HotKey.MountainSmall] = Keys.D4;
			mHotKeys[(int) HotKey.Mountain] = Keys.D5;
			mHotKeys[(int) HotKey.MountainBig] = Keys.D6;
			mHotKeys[(int) HotKey.Pond] = Keys.D7;
			mHotKeys[(int) HotKey.HomeZonePlayer] = Keys.D8;
			mHotKeys[(int) HotKey.HomeZoneAi] = Keys.D9;

			mHotKeys[(int) HotKey.HideObjects] = Keys.Space;
			mHotKeys[(int) HotKey.EditorSafety] = Keys.X;

			//Player Keys
			mHotKeys[(int)HotKey.CenterCamera] = Keys.Space;
			mHotKeys[(int)HotKey.CameraUp] = Keys.W;
			mHotKeys[(int)HotKey.CameraLeft] = Keys.A;
			mHotKeys[(int)HotKey.CameraDown] = Keys.S;
			mHotKeys[(int)HotKey.CameraRight] = Keys.D;
			mHotKeys[(int)HotKey.CameraZoomIn] = Keys.Q;
			mHotKeys[(int)HotKey.CameraZoomOut] = Keys.E;
			mHotKeys[(int)HotKey.ShowGameMenu] = Keys.Escape;
		}

		/// <summary>
		/// return the Hotkey of an Action
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Keys GetHotkey(HotKey id)
		{
			return mHotKeys[(int) id];
		}

		/// <summary>
		/// Set the Hotkey of an Action
		/// </summary>
		/// <param name="id"></param>
		/// <param name="key"></param>
		public void SetHotkey(HotKey id,Keys key)
		{
			mHotKeys[(int)id] = key;
		}

		public Keys[] GetArrayOfKeys { get { return mHotKeys; } }
	}
}
