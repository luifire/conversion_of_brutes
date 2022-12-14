/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Option Menu
 * Usage: This Class Implements the Option Menu for the game (to achieve from MainMenu)  
 * Missing: nothing 
 * 
 **/


using System;
using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class OptionScreen : Screen
	{
		[Serializable]
		public class Settings
		{
			public float mMusicVolume;
			public float mSoundVolume;
			public bool mSoundOn;
			public bool mMusicOn;
			public bool mFullScreen;
			public int mScreenWidth;
			public int mScreenHeight;
		}

		//For the sound options
		protected float mMusicVolume;
		protected float mSoundVolume;
		protected MenuLabel mLabelRes;
		protected MenuLabel mLabelSound;
		
		// Bars 
		protected Bar mSoundBar;
		protected Bar mMusicBar;

		//For the resolution
		protected MenuButton mCurrentSelected;
		protected MenuButton mCurrentActive;
		protected Rectangle mSelectionArea;
		protected List<MenuButton> mSelectableRes;
		protected bool mResolutionChanged;
		private int mCounter;

		/// <summary>
		/// Basic constructor for the OptionScreen
		/// </summary>
		public OptionScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override void ScreenRemoved()
		{
			Settings op = new Settings()
			{
				mFullScreen = Main.Graphics.IsFullScreen,
				mMusicOn = Main.Audio.IsMusicOn,
				mSoundOn = Main.Audio.IsSoundOn,
				mMusicVolume = Main.Audio.CurrentMusicVolume,
				mSoundVolume = Main.Audio.CurrentSoundVolume,
				mScreenHeight = Main.Graphics.PreferredBackBufferHeight,
				mScreenWidth = Main.Graphics.PreferredBackBufferWidth
			};
			SaveAndLoad.SaveSettings(op);
		}

		public override sealed void Initialize()
		{

			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.OptionScreen, ScaledRectangle(0, 0, 1920, 1080), null);
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Hotkeys, ScaledRectangle(855, 810, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(965, 910, 210, 100)));

			//Part for the Resolution
			mLabelRes = new MenuLabel(MenuItem.MenuIdentifier.ResolutionLabel, ScaledRectangle(330, 220, 210, 100), null);
			mLabels.Add(new MenuLabel(MenuItem.MenuIdentifier.ResolutionLabel, ScaledRectangle(330, 570, 210, 100), "Click to choose!"));
			mSelectableRes = new List<MenuButton>();
			System.Windows.Forms.Screen scr = System.Windows.Forms.Screen.PrimaryScreen;
			if (scr.Bounds.Width >= 1920 && scr.Bounds.Height >= 1080)
			{
				mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1920, ScaledRectangle(330, 600, 210, 100)));
			}

			if (scr.Bounds.Width >= 1400 && scr.Bounds.Height >= 900)
			{
				mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1400, ScaledRectangle(330, 600, 210, 100)));
			}


			mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1280, ScaledRectangle(330, 600, 210, 100)));
			mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1024, ScaledRectangle(330, 600, 210, 100)));
			mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution800, ScaledRectangle(330, 600, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Windowed, ScaledRectangle(330, 700, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Fullscreen, ScaledRectangle(330, 800, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Apply, ScaledRectangle(750, 910, 210,100)));
			mSelectionArea = ScaledRectangle(330, 600, 210, 100);

			foreach (MenuButton button in mSelectableRes)
			{
				if (mScreenWidth == 1920 && button.Identifier == MenuItem.MenuIdentifier.Resolution1920)
				{
					mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(330, 400, 210, 100));
					break;
				}
                
				if (mScreenWidth == 1400 && button.Identifier == MenuItem.MenuIdentifier.Resolution1400)
				{
					mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(330, 400, 210, 100));
					break;
				}

				if (mScreenWidth == 1280 && button.Identifier == MenuItem.MenuIdentifier.Resolution1280)
				{
					mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(330, 400, 210, 100));
					break;
				}

				if (mScreenWidth == 1024 && button.Identifier == MenuItem.MenuIdentifier.Resolution1024)
				{
					mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(330, 400, 210, 100));
					break;
				}

				if (mScreenWidth == 800 && button.Identifier == MenuItem.MenuIdentifier.Resolution800)
				{
					mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(330, 400, 210, 100));
					break;
				}

				mCurrentActive = new MenuButton(MenuItem.MenuIdentifier.Resolution1280, ScaledRectangle(330, 400, 210, 100));

			}

			mCurrentSelected = new MenuButton(mCurrentActive.Identifier, mCurrentActive.Rectangle)
			{
				Rectangle = ScaledRectangle(330, 600, 210, 100)
			};

			//Part for the Sound
			mMusicVolume = Main.Audio.CurrentMusicVolume;
			mSoundVolume = Main.Audio.CurrentSoundVolume;
			mLabelSound = new MenuLabel(MenuItem.MenuIdentifier.SoundLabel, ScaledRectangle(1400, 220, 210, 100), null);
			mLabels.Add(AddLabel(1300, 420, "Music Volume:"));
			mLabels.Add(AddLabel(1300, 580, "Sound Volume:"));

			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicUp, ScaledRectangle(1300, 460, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicDown, ScaledRectangle(1400, 460, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOn, ScaledRectangle(1500, 460, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOff, ScaledRectangle(1600, 460, 100, 50)));
			mMusicBar = new Bar(mSpriteBatch, ScaledRectangle(1300, 520, 400, 50));

			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundUp, ScaledRectangle(1300, 620, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundDown, ScaledRectangle(1400, 620, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOn, ScaledRectangle(1500, 620, 100, 50)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOff, ScaledRectangle(1600, 620, 100, 50)));
			mSoundBar = new Bar(mSpriteBatch, ScaledRectangle(1300, 680, 400, 50));
			
		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();

			TestMouseSlideCurrentSlected();


			if (Main.Input.MouseClicked())
			{

				TestMouseClickedCurrentSelected();

				foreach (MenuButton button in mItems)
				{
					Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
					if (button.Rectangle.Contains(point))
					{
						TestSoundControl(button);
						TestResolutionControlMain(button);
						if (button.Identifier == MenuItem.MenuIdentifier.Hotkeys)
						{
							mManager.AddScreen(new HotkeyScreen());
						}

						if (button.Identifier == MenuItem.MenuIdentifier.Back)
						{
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							mManager.AddScreen(new MainMenuScreen());
						}
					}

				}
			}

		}


		public override void Draw()
		{

			//Drawing the background in the rectangle
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mBackground.FirstTexture, mBackground.Rectangle, Color.White);
			mSpriteBatch.End();

			//Drawing not the hole list of buttons for resolution
			mCurrentSelected.DrawButton();
			mCurrentActive.DrawButton();


			//Drawing each button for sound options
			foreach (MenuButton button in mItems)
			{
				button.DrawButton();
			}
			DrawListOfLabels(mLabels);
			DrawLabelsOption();

			mSoundBar.Draw(Main.Audio.CurrentSoundVolume,1f);
			mMusicBar.Draw(Main.Audio.CurrentMusicVolume, 1f);

		}


		private void TestResolutionControlMain(MenuButton button)
		{
			switch (button.Identifier)
			{
				case MenuItem.MenuIdentifier.Apply:

					switch (mCurrentSelected.Identifier)
					{
						case MenuItem.MenuIdentifier.Resolution1920:
							Main.Graphics.PreferredBackBufferWidth = 1920;
							Main.Graphics.PreferredBackBufferHeight = 1080;
							Main.Graphics.ApplyChanges();
							Initialize();
							break;
						case MenuItem.MenuIdentifier.Resolution1400:
							Main.Graphics.PreferredBackBufferWidth = 1400;
							Main.Graphics.PreferredBackBufferHeight = 900;
							Main.Graphics.ApplyChanges();
							Initialize();
							break;
						case MenuItem.MenuIdentifier.Resolution1280:
							Main.Graphics.PreferredBackBufferWidth = 1280;
							Main.Graphics.PreferredBackBufferHeight = 720;
							Main.Graphics.ApplyChanges();
							Initialize();
							break;
						case MenuItem.MenuIdentifier.Resolution1024:
							Main.Graphics.PreferredBackBufferWidth = 1024;
							Main.Graphics.PreferredBackBufferHeight = 768;
							Main.Graphics.ApplyChanges();
							Initialize();
							break;
						case MenuItem.MenuIdentifier.Resolution800:
							Main.Graphics.PreferredBackBufferWidth = 800;
							Main.Graphics.PreferredBackBufferHeight = 600;
							Main.Graphics.ApplyChanges();
							Initialize();
							break;
					}


					break;
				case MenuItem.MenuIdentifier.Fullscreen:
					Main.Graphics.IsFullScreen = true;
					Main.Graphics.ApplyChanges();
					Initialize();
					break;
				case MenuItem.MenuIdentifier.Windowed:
					Main.Graphics.IsFullScreen = false;
					Main.Graphics.ApplyChanges();
					Initialize();
					break;
			}
		}



		protected void TestSoundControl(MenuButton button)
		{


			switch (button.Identifier)
			{

				case MenuItem.MenuIdentifier.MusicUp:
					if (mMusicVolume > 1) mMusicVolume = 1;
					if (mMusicVolume < 1)
					{
						mMusicVolume += 0.1f;
						Main.Audio.SetVolume(AudioManager.Category.Music, mMusicVolume);

					}

					break;
				case MenuItem.MenuIdentifier.MusicDown:
					if (mMusicVolume < 0) mMusicVolume = 0;
					if (mMusicVolume <= 1 && mMusicVolume > 0)
					{
						mMusicVolume -= 0.1f;
						Main.Audio.SetVolume(AudioManager.Category.Music, mMusicVolume);

					}
					break;
				case MenuItem.MenuIdentifier.MusicOn:
					Main.Audio.ToggleMusic(true);
					break;
				case MenuItem.MenuIdentifier.MusicOff:
					Main.Audio.ToggleMusic(false);
					break;
				case MenuItem.MenuIdentifier.SoundUp:
					if (mSoundVolume > 1) mSoundVolume = 1;

					if (mSoundVolume < 1)
					{
						mSoundVolume += 0.1f;
						Main.Audio.SetVolume(AudioManager.Category.Units, mSoundVolume);
					}
					Main.Audio.PlaySound(AudioManager.Sound.Click3);
					break;
				case MenuItem.MenuIdentifier.SoundDown:
					if (mSoundVolume < 0) mSoundVolume = 0;
					if (mSoundVolume <= 1 && mSoundVolume > 0)
					{
						mSoundVolume -= 0.1f;
						Main.Audio.SetVolume(AudioManager.Category.Units, mSoundVolume);
					}
					Main.Audio.PlaySound(AudioManager.Sound.Click3);
					break;
				case MenuItem.MenuIdentifier.SoundOn:
					Main.Audio.ToogleSound(true);
					Main.Audio.PlaySound(AudioManager.Sound.Convert);
					break;
				case MenuItem.MenuIdentifier.SoundOff:
					Main.Audio.ToogleSound(false);
					Main.Audio.PlaySound(AudioManager.Sound.Convert);
					break;
			}
		}

		private void DrawLabelsOption()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mLabelSound.FirstTexture, mLabelSound.Rectangle, Color.White);
			mSpriteBatch.Draw(mLabelRes.FirstTexture, mLabelRes.Rectangle, Color.White);
			mSpriteBatch.End();
		}

		protected void TestMouseSlideCurrentSlected()
		{
			Point point1 = new Point((int)Main.Input.GetCurrentMousePosition.X, (int)Main.Input.GetCurrentMousePosition.Y);
			if (mCurrentSelected != null)
				mCurrentSelected.CheckIsMouseOver(point1);
		}

		protected void TestMouseClickedCurrentSelected()
		{
			Point p = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
			if (mSelectionArea.Contains(p))
			{

				if (mCounter < mSelectableRes.Count - 1)
				{
					mCurrentSelected = mSelectableRes[++mCounter];

				}
				else
				{
					mCounter = 0;
					mCurrentSelected = mSelectableRes[mCounter];
				}

			}
		}

	}
}



