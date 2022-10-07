/**
 * Author: David Spisla 
 * 
 * Concrete Class for a Button
 * Usage: You can implement a button with this class  
 * Missing: nothing 
 * 
 **/

using System;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.MenuElements
{
	public class MenuButton : MenuItem
	{

		private SpriteBatch mMSpriteBatch;
		private Texture2D mTexture2DSecond;
		private Texture2D mTextureOriginal;
		private bool mStillHovering;

		/// <summary>
		/// Constructor which creates a button for a ActionIdent specified by the internal Enum MenuIdentifier
        /// </summary>
        /// <param name="action"></param>
        /// <param name="rectangle"></param>
		public MenuButton(MenuIdentifier action, Rectangle rectangle)
        {

			GraphicsDeviceManager graphics = Main.Graphics;
			mMSpriteBatch = new SpriteBatch(graphics.GraphicsDevice);

			String qualifier;
			switch (action)
			{
				case MenuIdentifier.Attack:
					qualifier = "Button\\attack";
					break;
				case MenuIdentifier.Convert:
					qualifier = "Button\\convert";
					break;
				case MenuIdentifier.Taunt:
					qualifier = "Button\\taunt";
					break;
				case MenuIdentifier.AttackMove:
					qualifier = "Button\\attackmove";
					break;
				case MenuIdentifier.Move:
					qualifier = "Button\\move";
					break;
				case MenuIdentifier.Patrol:
					qualifier = "Button\\patrol";
					break;
				case MenuIdentifier.Stop:
					qualifier = "Button\\stop";
					break;
				case MenuIdentifier.SpawnShieldGuard:
					qualifier = "Button\\spawnSG";
					break;
				case MenuIdentifier.Play:
					qualifier = "Button\\play";
					break;
				case MenuIdentifier.Save:
					qualifier = "Button\\save";
					break;
				case MenuIdentifier.Load:
					qualifier = "Button\\load";
					break;
				case MenuIdentifier.Map:
					qualifier = "Button\\mapeditor";
					break;
				case MenuIdentifier.Achievements:
					qualifier = "Button\\achievements";
					break;
				case MenuIdentifier.Statistics:
					qualifier = "Button\\statistics";
					break;
				case MenuIdentifier.GameOptions:
					qualifier = "Button\\options";
					break;
				case MenuIdentifier.Exit:
					qualifier = "Button\\button_texture";
					break;
				case MenuIdentifier.Resume:
					qualifier = "Button\\resume";
					break;
				case MenuIdentifier.InGameOptions:
					qualifier = "Button\\options";
					break;
				case MenuIdentifier.Quit:
					qualifier = "Button\\quit";
					break;
				case MenuIdentifier.InGameMenu:
					qualifier = "Button\\menu";
					break;
				case MenuIdentifier.MainMenu:
					qualifier = "Button\\mainMenu";
					break;
				case MenuIdentifier.PlayerName:
					qualifier = "Button\\playerName";
					break;
				case MenuIdentifier.Difficulty:
					qualifier = "Button\\difficulty";
					break;
				case MenuIdentifier.Sound:
					qualifier = "Button\\sound";
					break;
				case MenuIdentifier.Resolution:
					qualifier = "Button\\resolution";
					break;
				case MenuIdentifier.Hotkeys:
					qualifier = "Button\\hotkeys";
					break;
				case MenuIdentifier.MusicUp:
					qualifier = "Button\\Plus";
					break;
				case MenuIdentifier.MusicDown:
					qualifier = "Button\\Minus";
					break;
				case MenuIdentifier.MusicOn:
					qualifier = "Button\\on";
					break;
				case MenuIdentifier.MusicOff:
					qualifier = "Button\\off";
					break;
				case MenuIdentifier.SoundUp:
					qualifier = "Button\\Plus";
					break;
				case MenuIdentifier.SoundDown:
					qualifier = "Button\\Minus";
					break;
				case MenuIdentifier.SoundOn:
					qualifier = "Button\\on";
					break;
				case MenuIdentifier.SoundOff:
					qualifier = "Button\\off";
					break;
				case MenuIdentifier.Resolution1920:
					qualifier = "Button\\resolution1920x1080";
					break;
				case MenuIdentifier.Resolution1400:
					qualifier = "Button\\resolution1400x900";
					break;
				case MenuIdentifier.Resolution1280:
					qualifier = "Button\\resolution1280x720";
					break;
				case MenuIdentifier.Resolution1024:
					qualifier = "Button\\resolution1024x768";
					break;
				case MenuIdentifier.Resolution800:
					qualifier = "Button\\resolution800x600";
					break;
				case MenuIdentifier.Fullscreen:
					qualifier = "Button\\fullscreen";
					break;
				case MenuIdentifier.Windowed:
					qualifier = "Button\\windowed";
					break;
				case MenuIdentifier.Credits:
					qualifier = "Button\\credits";
					break;
				case MenuIdentifier.NewGame:
					qualifier = "Button\\NewGame";
					break;
				case MenuIdentifier.Yes:
					qualifier = "Button\\Yes";
					break;
				case MenuIdentifier.No:
					qualifier = "Button\\No";
					break;
				case MenuIdentifier.Back:
					qualifier = "Button\\Back";
					break;
				case MenuIdentifier.Easy:
					qualifier = "Button\\DifficultyEasy";
					break;
				case MenuIdentifier.Medium:
					qualifier = "Button\\DifficultyMedium";
					break;
				case MenuIdentifier.Hard:
					qualifier = "Button\\DifficultyHard";
					break;
				case MenuIdentifier.Apply:
					qualifier = "Button\\Apply";
					break;
				case MenuIdentifier.TechDemo:
					qualifier = "Button\\Techdemo";
					break;
				case MenuIdentifier.Tutorial:
					qualifier = "Button\\Tutorial";
					break;
				default:
					throw new ArgumentOutOfRangeException("action", action, null);
			}
			mTexture2D = Main.Content.Load<Texture2D>(qualifier);
			mTextureOriginal = Main.Content.Load<Texture2D>(qualifier);
			mMenuAction = action;
			mRectangle = rectangle;
	        mIsMouseOver = false;
        }

		public void DrawButton()
		{
			if (mIsMouseOver)
			{
				if (!mStillHovering)
				{
					Main.Audio.PlaySound(AudioManager.Sound.Click3);
					mStillHovering = true;
				}
				mMSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
				mMSpriteBatch.Draw(mTexture2D, mRectangle, Color.White);
				mMSpriteBatch.End();
			}
			else
			{
				if (mStillHovering)
				{
					mStillHovering = false;
				}
				mMSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mMSpriteBatch.Draw(mTexture2D, mRectangle, Color.White);
				mMSpriteBatch.End();
			}
		}

		public Texture2D SecondTexture {
			private get { return mTexture2DSecond; } set { mTexture2DSecond = value; } }
		public Texture2D TextureOriginal { get { return mTextureOriginal; }
		}

		//Just use for the DifficultyButtons!!!
		public void SwapTexture()
		{
			Texture2D temp = FirstTexture;
			FirstTexture = SecondTexture;
			SecondTexture = temp;
		}
	}
}
