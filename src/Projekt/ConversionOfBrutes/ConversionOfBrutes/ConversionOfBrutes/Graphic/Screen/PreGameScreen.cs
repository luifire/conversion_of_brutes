/**
 * Author: David Spisla 
 * 
 * A kind of "PreGameScreen"
 * Usage: Here the player can choose the difficulty and a map for a new game  
 * Missing: nothing 
 * 
 **/

using System;
using System.Collections.Generic;
using ConversionOfBrutes.Graphic.MenuElements;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace ConversionOfBrutes.Graphic.Screen
{
	class PreGameScreen : Screen
	{
		private MenuLabel mDifficulty;
		private MenuButton mEasy;
		private MenuButton mMedium;
		private MenuButton mHard;

		private MenuButton mTutorial;
		private MenuButton mTechDemo;
		
		//Default Map
		private String mStringForLoad = "0"; //"default";
		private bool mEasyClcked;
		private bool mMediumClcked = true;
		private bool mHardClcked;
		private String mStringIsEmpty;
		private MenuLabel mSlotEmpty;
		private bool mSlotIsEmpty;
		private int mCounter;

		private LoadComponent mLoadComponent;
		public PreGameScreen()
		{
			Initialize();
			mIsEscClosable = true;
		}

		public override sealed void Initialize()
		{
			InitializeAbstr();
			mBackground = new MenuLabel(MenuItem.MenuIdentifier.OptionScreen, ScaledRectangle(0, 0, 1920, 1080), null);

			//Part for Loading the map
			mLabels.Add(AddLabel(1050, 215, "Choose a map!"));
			mSlotEmpty = AddLabel(700, 700, "Empty Map! Please choose another one...");
			List<String> list = SaveAndLoad.GetMapList();
			mLoadComponent = new LoadComponent(list, new Rectangle(1050, 275, 0, 0), new Rectangle(1280, 275, 30, 30));

			//Part for the Difficulty
			mDifficulty = new MenuLabel(MenuItem.MenuIdentifier.DifficultyLabel, ScaledRectangle(670, 215, 210, 55), null);
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Play, ScaledRectangle(750, 910, 210, 100)));
			mItems.Add(new MenuButton(MenuItem.MenuIdentifier.Back, ScaledRectangle(960, 910, 210, 100)));
			mTechDemo = new MenuButton(MenuItem.MenuIdentifier.Tutorial, ScaledRectangle(750, 760, 210, 100));
			mTutorial = new MenuButton(MenuItem.MenuIdentifier.TechDemo, ScaledRectangle(960, 760, 210, 100));

			mEasy = new MenuButton(MenuItem.MenuIdentifier.Easy, ScaledRectangle(670, 280, 210, 100));
			mMedium = new MenuButton(MenuItem.MenuIdentifier.Medium, ScaledRectangle(670, 390, 210, 100));
			mHard = new MenuButton(MenuItem.MenuIdentifier.Hard, ScaledRectangle(670, 500, 210, 100));
			mEasy.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyEasy_clicked");
			mMedium.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyMedium_clicked");
			mHard.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyHard_clicked");
			mItems.Add(mEasy);
			mItems.Add(mMedium);
			mItems.Add(mHard);
			mItems.Add(mTechDemo);
			mItems.Add(mTutorial);

			//Define the default values for the difficulty
			//Default is Medium! Medium BUtton is in clicked position!
			mMedium.SwapTexture();
			GameScreen.GameDifficulty = GameDifficulty.Normal;
		}

		public override void Update()
		{
			base.Update();
			TestMouseSlideOver();
			mLoadComponent.Update();

			if (Main.Input.MouseClicked())
			{

				Point point = new Point((int)Main.Input.MouseClickPosition().X, (int)Main.Input.MouseClickPosition().Y);
				
				//This is the part for choosing a map

				int temp = mLoadComponent.TestLoadSaveMode();
				if (temp != -1)
				{
					mStringForLoad = temp.ToString();
					Console.WriteLine(mStringForLoad);
					mStringIsEmpty = mLoadComponent.StringIsEmpty;
				}


				//This is the part for the difficulty button
				foreach (MenuButton button in mItems)
				{
					if (button.Rectangle.Contains(point))
					{
						switch (button.Identifier)
						{
							case MenuItem.MenuIdentifier.Play:

								if (mStringForLoad != "default" && mStringIsEmpty != "empty")
								{
									Main.Audio.StopSound(AudioManager.Sound.MainMenuMusic);
									Main.Audio.PlaySound(AudioManager.Sound.Click2);
									SaveAndLoad.LoadNewGame(mStringForLoad);
									GameScreen.FogOfWar = true;
									GameScreen.TechDemo = false;
								}
								else
								{
									mSlotIsEmpty = true;
									mCounter = 0;
								}
								
								break;
							case MenuItem.MenuIdentifier.Back:
								mManager.RemoveScreen();
								break;
							case MenuItem.MenuIdentifier.TechDemo:
								SaveAndLoad.LoadSaveGame(4242);
								GameScreen.TechDemo = true;
								break;
							case MenuItem.MenuIdentifier.Tutorial:
								mManager.AddScreen(new TutorialScreen());
								break;
							case MenuItem.MenuIdentifier.Easy:
								if (!mEasyClcked)
								{
									mEasy.SwapTexture();
									mMedium.FirstTexture = mMedium.TextureOriginal;
									mMedium.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyMedium_clicked");
									mHard.FirstTexture = mHard.TextureOriginal;
									mHard.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyHard_clicked");
									mEasyClcked = true;
								}

								mMediumClcked = false;
								mHardClcked = false;

								GameScreen.GameDifficulty = GameDifficulty.Easy;
								break;
							case MenuItem.MenuIdentifier.Medium:
								if (!mMediumClcked)
								{
									mMedium.SwapTexture();
									mEasy.FirstTexture = mEasy.TextureOriginal;
									mEasy.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyEasy_clicked");
									mHard.FirstTexture = mHard.TextureOriginal;
									mHard.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyHard_clicked");
									mMediumClcked = true;
								}

								mEasyClcked = false;
								mHardClcked = false;

								GameScreen.GameDifficulty = GameDifficulty.Normal;
								break;
							case MenuItem.MenuIdentifier.Hard:
								if (!mHardClcked)
								{
									mHard.SwapTexture();
									mEasy.FirstTexture = mEasy.TextureOriginal;
									mEasy.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyEasy_clicked");
									mMedium.FirstTexture = mMedium.TextureOriginal;
									mMedium.SecondTexture = Main.Content.Load<Texture2D>("Button\\DifficultyMedium_clicked");
									mHardClcked = true;
								}

								mEasyClcked = false;
								mMediumClcked = false;

								GameScreen.GameDifficulty = GameDifficulty.Hard;
								break;
						}
					}

				}
			}

		}

		public override void Draw()
		{
			DrawButtonsAndBackground();
			DrawListOfLabels(mLabels);
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			mSpriteBatch.Draw(mDifficulty.FirstTexture, mDifficulty.Rectangle, Color.White);
			mSpriteBatch.End();

			mLoadComponent.Draw();

			if (mSlotIsEmpty && mCounter <= 200)
			{
				mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				mSpriteBatch.DrawString(mSlotEmpty.GetFont(), mSlotEmpty.Text, new Vector2(mSlotEmpty.Rectangle.X, mSlotEmpty.Rectangle.Y), Color.AliceBlue,
				0f, new Vector2(0, 0), 2 * (mScreenWidth / 1920f), SpriteEffects.None, 0f);
				mSpriteBatch.End();
				mCounter++;
			}
		}
	}
}
