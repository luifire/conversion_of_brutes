/**
 * Author: David Spisla 
 * 
 * Concrete Class for the InGame Options
 * Usage: This Class Implements the InGame Options for the game  
 * Missing: nothing
 * 
 **/

using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes.Graphic.Screen
{
	class InGameOptionsScreen : OptionScreen
	{
		
		
		/// <summary>
		/// Basic constructor for the InGameOptionScreen
		/// </summary>
		public InGameOptionsScreen()
		{
			
			InitializeOption();
		}


		private void InitializeOption()
		{

			InitializeAbstr();
			mDrawScreensBelow = true;
			mUpdateScreensBelow = false;
			mIsEscClosable = true;
			
			//Part for the Resolution
			if (!GameScreen.MapEditorMode)
			{

				mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameOptionsScreen, ScaledRectangle(480, 165, 908, 863), null);
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Hotkeys, ScaledRectangle(855, 855, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(925, 930, 150, 80)));
				
				mLabelRes = new MenuLabel(MenuItem.MenuIdentifier.ResolutionLabel, ScaledRectangle(610, 350, 210, 100), null);
				mLabels.Add(new MenuLabel(MenuItem.MenuIdentifier.ResolutionLabel,
					ScaledRectangle(610, 570, 210, 100),
					"Click to choose!"));
				mSelectableRes = new List<MenuButton>();
				System.Windows.Forms.Screen scr = System.Windows.Forms.Screen.PrimaryScreen;
				if (scr.Bounds.Width >= 1920 && scr.Bounds.Height >= 1080)
				{
					mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1920, ScaledRectangle(640, 600, 150, 80)));
				}

				if (scr.Bounds.Width >= 1400 && scr.Bounds.Height >= 900)
				{
					mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1400, ScaledRectangle(640, 600, 150, 80)));
				}


				mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1280, ScaledRectangle(640, 600, 150, 80)));
				mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution1024, ScaledRectangle(640, 600, 150, 80)));
				mSelectableRes.Add(new MenuButton(MenuItem.MenuIdentifier.Resolution800, ScaledRectangle(640, 600, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Windowed, ScaledRectangle(640, 690, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Fullscreen, ScaledRectangle(640, 780, 150, 80)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Apply, ScaledRectangle(775, 930, 150, 80)));
				mSelectionArea = ScaledRectangle(630, 600, 210, 100);

				foreach (MenuButton button in mSelectableRes)
				{

					if (mScreenWidth == 1920 && button.Identifier == MenuItem.MenuIdentifier.Resolution1920)
					{
						mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(640, 450, 150, 80));
						break;
					}

					if (mScreenWidth == 1400 && button.Identifier == MenuItem.MenuIdentifier.Resolution1400)
					{
						mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(640, 450, 150, 80));
						break;
					}

					if (mScreenWidth == 1280 && button.Identifier == MenuItem.MenuIdentifier.Resolution1280)
					{
						mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(640, 450, 150, 80));
						break;
					}

					if (mScreenWidth == 1024 && button.Identifier == MenuItem.MenuIdentifier.Resolution1024)
					{
						mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(640, 450, 150, 80));
						break;
					}

					if (mScreenWidth == 800 && button.Identifier == MenuItem.MenuIdentifier.Resolution800)
					{
						mCurrentActive = new MenuButton(button.Identifier, ScaledRectangle(640, 450, 150, 80));
						break;
					}

					mCurrentActive = new MenuButton(MenuItem.MenuIdentifier.Resolution1280, ScaledRectangle(640, 450, 150, 80));

				}

				mCurrentSelected = new MenuButton(mCurrentActive.Identifier, mCurrentActive.Rectangle)
				{
					Rectangle = ScaledRectangle(640, 600, 150, 80)
				};

				//Part for the Sound
				mMusicVolume = Main.Audio.CurrentMusicVolume;
				mSoundVolume = Main.Audio.CurrentSoundVolume;
				mLabelSound = new MenuLabel(MenuItem.MenuIdentifier.SoundLabel, ScaledRectangle(1050, 350, 210, 100), null);
				mLabels.Add(AddLabel(950, 470, "Music Volume:"));
				mLabels.Add(AddLabel(950, 630, "Sound Volume:"));

				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicUp, ScaledRectangle(950, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicDown, ScaledRectangle(1050, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOn, ScaledRectangle(1150, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOff, ScaledRectangle(1250, 510, 100, 50)));
				mMusicBar = new Bar(mSpriteBatch, ScaledRectangle(950, 570, 400, 50));

				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundUp, ScaledRectangle(950, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundDown, ScaledRectangle(1050, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOn, ScaledRectangle(1150, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOff, ScaledRectangle(1250, 670, 100, 50)));
				mSoundBar = new Bar(mSpriteBatch, ScaledRectangle(950, 730, 400, 50));
			}
			else if(GameScreen.MapEditorMode) //This is for MapEditorMode because we onlöy want to edit the sound options there
			{

				mBackground = new MenuLabel(MenuItem.MenuIdentifier.InGameOptionsScreen, ScaledRectangle(480, 165, 908, 863), null);
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(855, 870, 150, 80)));
				
				//Part for the Sound
				mMusicVolume = Main.Audio.CurrentMusicVolume;
				mSoundVolume = Main.Audio.CurrentSoundVolume;
				mLabelSound = new MenuLabel(MenuItem.MenuIdentifier.SoundLabel, ScaledRectangle(840, 350, 210, 100), null);
				mLabels.Add(AddLabel(740, 470, "Music Volume:"));
				mLabels.Add(AddLabel(740, 630, "Sound Volume:"));

				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicUp, ScaledRectangle(740, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicDown, ScaledRectangle(840, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOn, ScaledRectangle(940, 510, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.MusicOff, ScaledRectangle(1040, 510, 100, 50)));
				mMusicBar = new Bar(mSpriteBatch, ScaledRectangle(740, 570, 400, 50));

				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundUp, ScaledRectangle(740, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundDown, ScaledRectangle(840, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOn, ScaledRectangle(940, 670, 100, 50)));
				mItems.Add(new MenuButton(MenuItem.MenuIdentifier.SoundOff, ScaledRectangle(1040, 670, 100, 50)));
				mSoundBar = new Bar(mSpriteBatch, ScaledRectangle(740, 730, 400, 50));
			}
		}

		public override void Update()
		{
			if (Main.Input.WasButtonPressed(Keys.Escape))
			{
				mManager.RemoveScreen();
			}

			if (!GameScreen.MapEditorMode)
			{
				TestMouseSlideCurrentSlected();	
			}
			TestMouseSlideOver();


			if (Main.Input.MouseClicked())
			{

				if (!GameScreen.MapEditorMode)
				{
					TestMouseClickedCurrentSelected();
				}

				foreach (MenuButton button in mItems)
				{
					Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
					if (button.Rectangle.Contains(point))
					{
						
						TestSoundControl(button);

						if(!GameScreen.MapEditorMode)
						    mResolutionChanged = TestResolutionControlInGame(button);


						if (button.Identifier == MenuItem.MenuIdentifier.Hotkeys)
						{
							mManager.AddScreen(new HotkeyScreen());
						}

						if (button.Identifier == MenuItem.MenuIdentifier.Back)
						{
							mManager.RemoveScreen();
							
						}

						if (!GameScreen.MapEditorMode)
						{
							if (mResolutionChanged)
						{

							Main.mGameStatistic.DurationOfGame += GameScreen.Stopwatch.Elapsed.TotalSeconds;
							GameScreen.Stopwatch.Reset();	
							SaveAndLoad.SaveGame(1337);
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							mManager.RemoveScreen();
							SaveAndLoad.LoadSaveGame(1337);
						    mManager.AddScreen(new InGameMenuScreen());
							mManager.AddScreen(new InGameOptionsScreen());
						}
						}

					}

				}
			}

		}


		public override void Draw()
		{

			if (!GameScreen.MapEditorMode)
			{
				base.Draw();
			}
			else if (GameScreen.MapEditorMode)
			{
				//Drawing the background in the rectangle
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.Draw(mBackground.FirstTexture, mBackground.Rectangle, Color.White);
				mSpriteBatch.Draw(mLabelSound.FirstTexture, mLabelSound.Rectangle, Color.White);
				mSpriteBatch.End();
			}

			//Drawing each button for sound options
			foreach (MenuButton button in mItems)
			{
				button.DrawButton();
			}
			DrawListOfLabels(mLabels);

			mSoundBar.Draw(Main.Audio.CurrentSoundVolume, 1f);
			mMusicBar.Draw(Main.Audio.CurrentMusicVolume, 1f);
			Handling2DAnd3D();

		}


		private bool TestResolutionControlInGame(MenuButton button)
		{

			bool resolutionChanged = false;
			switch (button.Identifier)
			{
				case MenuItem.MenuIdentifier.Apply:

					switch (mCurrentSelected.Identifier)
					{
						case MenuItem.MenuIdentifier.Resolution1920:
							Main.Graphics.PreferredBackBufferWidth = 1920;
							Main.Graphics.PreferredBackBufferHeight = 1080;
							Main.Graphics.ApplyChanges();
							resolutionChanged = true;
							break;
						case MenuItem.MenuIdentifier.Resolution1400:
							Main.Graphics.PreferredBackBufferWidth = 1400;
							Main.Graphics.PreferredBackBufferHeight = 900;
							Main.Graphics.ApplyChanges();
							resolutionChanged = true;
							break;
						case MenuItem.MenuIdentifier.Resolution1280:
							Main.Graphics.PreferredBackBufferWidth = 1280;
							Main.Graphics.PreferredBackBufferHeight = 720;
							Main.Graphics.ApplyChanges();
							resolutionChanged = true;
							break;
						case MenuItem.MenuIdentifier.Resolution1024:
							Main.Graphics.PreferredBackBufferWidth = 1024;
							Main.Graphics.PreferredBackBufferHeight = 768;
							Main.Graphics.ApplyChanges();
							resolutionChanged = true;
							break;
						case MenuItem.MenuIdentifier.Resolution800:
							Main.Graphics.PreferredBackBufferWidth = 800;
							Main.Graphics.PreferredBackBufferHeight = 600;
							Main.Graphics.ApplyChanges();
							resolutionChanged = true;
							break;
					}


					break;
				case MenuItem.MenuIdentifier.Fullscreen:
					Main.Graphics.IsFullScreen = true;
					Main.Graphics.ApplyChanges();
					InitializeOption();
					break;
				case MenuItem.MenuIdentifier.Windowed:
					Main.Graphics.IsFullScreen = false;
					Main.Graphics.ApplyChanges();
					InitializeOption();
					break;
			}
			return resolutionChanged;

		}

	}
}
