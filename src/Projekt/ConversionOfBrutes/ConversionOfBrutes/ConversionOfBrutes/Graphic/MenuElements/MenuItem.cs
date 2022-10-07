/**
 * Author: David Spisla 
 * 
 * Abstract Class for Buttons and Texts, Pictures
 * Usage: see above 
 * Missing: nothing
 *
 **/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	public abstract class MenuItem
	{
		protected Texture2D mTexture2D;
		protected Rectangle mRectangle;
		protected MenuIdentifier mMenuAction;
		protected bool mIsMouseOver;
		/// <summary>
		/// Enum for Identification of a MenuItem
		/// </summary>
		public enum MenuIdentifier
		{
			Achievements,
			AchievementScreen,
			Attack,
			AttackMove,
			Apply,
			Background,
			Back,
			BlankoScreen,
			Convert,
			Credits,
			CreditsScreen,
			Difficulty,
			DifficultyScreen,
			DifficultyLabel,
			Easy,
			Medium,
			Hard,
			Exit,
			Fullscreen,
			GameOptions,
			Hotkeys,
			HotkeyScreen,
			InGameHotkeyScreen,
			InGameLoadGameScreen,
			InGameSaveGameScreen,
			InGameSaveMapScreen,
			InGameLoadMapScreen,
			InGameMenu,
			InGameMenuScreen,
			InGameOptions,
			Label,
			Load,
			LoadingScreen,
			LoadGameScreen,
			MainMenu,
			NewGame,
			Map,
			Move,
			MusicDown,
			MusicOff,
			MusicOn,
			MusicUp,
			OptionScreen,
			InGameOptionsScreen,
			Patrol,
			Play,
			PlayerName,
			Quit,
			Resolution,
			ResolutionLabel,
			Resolution1024,
			Resolution1280,
			Resolution1400,
			Resolution1920,
			Resolution800,
			ResolutionScreen,
			InGameResolutionScreen,
			Resume,
			Save,
			Sound,
			SoundDown,
			SoundOff,
			SoundOn,
			SoundScreen,
			SoundLabel,
			InGameSoundScreen,
			SoundUp,
			SpawnShieldGuard,
			Statistics,
			StatisticScreen,
			Stop,
			Taunt,
			Windowed,
			VictoryScreen,
			DefeatScreen,
			AchievementIcon,
			Yes,
			No,
			TechDemo,
			Tutorial
		}

		/// <summary>
		/// This methods sets the bool true or false in dependency of the mouse position
		/// </summary>
		/// <param name="point"></param>
		public void CheckIsMouseOver(Point point)
		{
			IsMouseOver = mRectangle.Contains(point);
		}

		public MenuIdentifier Identifier { get { return mMenuAction; } }

		public Rectangle Rectangle { get { return mRectangle; } set { mRectangle = value; }}

		public Texture2D FirstTexture { get { return mTexture2D; } set { mTexture2D = value; } }

		private bool IsMouseOver
		{
			set { mIsMouseOver = value; }
		}
	}
}
