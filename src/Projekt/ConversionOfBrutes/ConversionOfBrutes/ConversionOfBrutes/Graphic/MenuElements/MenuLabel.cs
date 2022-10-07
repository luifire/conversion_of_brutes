/**
 * Author: David Spisla 
 * 
 * Concrete Class for texts and pictures printed on the screen
 * Usage: This Class Implements the abstract MenuItem class 
 * Missing: nothing
 * 
 **/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	public sealed class MenuLabel : MenuItem
	{

		private SpriteFont mFont;

		private String mText;

		/// <summary>
		/// Constructor a MenuLabel. A Label is something like pictures or text (nothing clickable)
		/// If a label has no text, so just put null for the parameter
		/// </summary>
		/// <param name="action"></param>
		/// <param name="rectangle"></param>
		/// <param name="text"></param>
		public MenuLabel(MenuIdentifier action, Rectangle rectangle, String text)
		{

			mText = text ?? "";

			if (action.Equals(MenuIdentifier.Background))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\Background");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.LoadingScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\LoadingScreen");
				mMenuAction = MenuIdentifier.LoadingScreen;
			}
			else if (action.Equals(MenuIdentifier.OptionScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\OptionScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.AchievementScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\AchievementScreen");
				mMenuAction = MenuIdentifier.AchievementScreen;
			}
			else if (action.Equals(MenuIdentifier.CreditsScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\CreditsScreen");
				mMenuAction = MenuIdentifier.CreditsScreen;
			}
			else if (action.Equals(MenuIdentifier.DifficultyScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\DifficultyScreen");
				mMenuAction = MenuIdentifier.DifficultyScreen;
			}
			else if (action.Equals(MenuIdentifier.ResolutionScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ResolutionScreen");
				mMenuAction = MenuIdentifier.ResolutionScreen;
			}
			else if (action.Equals(MenuIdentifier.SoundScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\SoundScreen");
				mMenuAction = MenuIdentifier.SoundScreen;
			}
			else if (action.Equals(MenuIdentifier.StatisticScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\StatisticScreen");
				mMenuAction = MenuIdentifier.StatisticScreen;
			}
			else if (action.Equals(MenuIdentifier.HotkeyScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\HotkeyScreen");
				mMenuAction = MenuIdentifier.HotkeyScreen;
			}
			else if (action.Equals(MenuIdentifier.InGameMenuScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ingameMenuScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.InGameHotkeyScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ingameHotKeysScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.InGameOptionsScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ingameOptionsScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.InGameSoundScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ingameSoundScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.InGameResolutionScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\ingameResolutionScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.VictoryScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\VictoryScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.DefeatScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\DefeatScreen");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.AchievementIcon))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Button\\star");
				mMenuAction = MenuIdentifier.Background;
			}
			else if (action.Equals(MenuIdentifier.ResolutionLabel))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Button\\resolution_label");
				mMenuAction = MenuIdentifier.SoundLabel;
			}
			else if (action.Equals(MenuIdentifier.SoundLabel))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Button\\sound_label");
				mMenuAction = MenuIdentifier.SoundLabel;
			}
			else if (action.Equals(MenuIdentifier.BlankoScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\screen_blank");
				mMenuAction = MenuIdentifier.BlankoScreen;
			}
			else if (action.Equals(MenuIdentifier.LoadGameScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\LoadGameScreen");
				mMenuAction = MenuIdentifier.LoadGameScreen;
			}
			else if (action.Equals(MenuIdentifier.InGameLoadGameScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\InGameLoadGameScreen");
				mMenuAction = MenuIdentifier.InGameLoadGameScreen;
			}
			else if (action.Equals(MenuIdentifier.InGameSaveGameScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\InGameSaveGameScreen");
				mMenuAction = MenuIdentifier.InGameSaveGameScreen;
			}
			else if (action.Equals(MenuIdentifier.InGameSaveMapScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\InGameSaveMapScreen");
				mMenuAction = MenuIdentifier.InGameSaveMapScreen;
			}
			else if (action.Equals(MenuIdentifier.InGameLoadMapScreen))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Screen\\InGameLoadMapScreen");
				mMenuAction = MenuIdentifier.InGameSaveMapScreen;
			}
			else if (action.Equals(MenuIdentifier.DifficultyLabel))
			{
				mTexture2D = Main.Content.Load<Texture2D>("Button\\difficulty_label");
				mMenuAction = MenuIdentifier.BlankoScreen;
			}
			else
			{
				mMenuAction = MenuIdentifier.Label;
			}

			mFont = Main.Content.Load<SpriteFont>("Fonts\\Helvetica");
			mRectangle = rectangle;
		}

		public String Text { get { return mText; } set { mText = value; } }

		public SpriteFont GetFont()
		{
			return mFont;
		}

	}
}
