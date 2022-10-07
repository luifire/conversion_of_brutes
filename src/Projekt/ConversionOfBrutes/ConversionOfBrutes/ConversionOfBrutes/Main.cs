//#define AUTO_START

using System;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Misc.Statistics;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ConversionOfBrutes
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Main : Game
	{
		private static GraphicsDeviceManager sGraphics;
		private static AudioManager sAudioManager;
		private static ContentManager sContent;
		private static InputManager sInputManager;
		private static GameTime sGameTime;
		private static Game sGame;
		private readonly static Random sRandom = new Random();
		internal static AllOverGameStatistic mAllOverGameStatistic;
		private static HotKeys sSHotKeys;
		internal static GameStatistic mGameStatistic;
		internal static Achievements mAchievements;
		private ScreenManager mScreenManager;
		private MainMenuScreen mMainMenuScreen;

		//Values for Achievent
		public static float AxRampage { get; private set; }
		public static float AxWololo { get; private set; }
		public static float AxUnstobbable { get; private set; }
		public static float AxFasterLight { get; private set; }
		public static float Pause15Times { get; private set; }

		public Main()
		{
			sGraphics = new GraphicsDeviceManager(this);
			sAudioManager = new AudioManager();
			sInputManager = new InputManager();
			sSHotKeys = new HotKeys();
			sGame = this;
			sContent = base.Content;

			AxRampage = 10;
			AxFasterLight = 600;
			AxWololo = 1000;
			AxUnstobbable = 1000;
			Pause15Times = 15;

			sContent.RootDirectory = "Content";
			IsMouseVisible = true;

			InitializeSettings();
		}

		private void InitializeSettings()
		{
			//Full HD 1920 x 1080
			//1280x720 default
			//800x600
			// Screen Res
			OptionScreen.Settings settings = SaveAndLoad.LoadSettings();
			if (settings == null)
			{
				System.Windows.Forms.Screen scr = System.Windows.Forms.Screen.PrimaryScreen;
				sGraphics.PreferredBackBufferWidth = scr.Bounds.Width;  // set this value to the desired width of your window
				sGraphics.PreferredBackBufferHeight = scr.Bounds.Height;   // set this value to the desired height of your window
				sGraphics.IsFullScreen = false;
			}
			else
			{
				sGraphics.PreferredBackBufferWidth = settings.mScreenWidth;
				sGraphics.PreferredBackBufferHeight = settings.mScreenHeight;
				sGraphics.IsFullScreen = settings.mFullScreen;

				sAudioManager.SetVolume(AudioManager.Category.Units, settings.mSoundVolume);
				sAudioManager.SetVolume(AudioManager.Category.Music, settings.mMusicVolume);
				if (settings.mMusicOn == false)
					sAudioManager.ToggleMusic(false);

				if (settings.mSoundOn == false)
					sAudioManager.ToogleSound(false);
			}
			sGraphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Achievements and stuff
			mGameStatistic = SaveAndLoad.LoadGameStatistic();
			mAllOverGameStatistic = SaveAndLoad.LoadOverallGameStatistic();
			mAchievements = SaveAndLoad.LoadAchievements();


			mMainMenuScreen = new MainMenuScreen();
			mScreenManager = new ScreenManager(mMainMenuScreen);
			base.Initialize();
#if AUTO_START
			SaveAndLoad.LoadNewGame("0");
#endif
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			sGameTime = gameTime;
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				MainExit();

			sInputManager.Update(Keyboard.GetState(), Mouse.GetState());
			mScreenManager.Update();
			sAudioManager.Update();

			base.Update(gameTime);
		}

		public static void MainExit()
		{
			SaveAndLoad.SaveOverallGameStatistic(mAllOverGameStatistic);
			SaveAndLoad.SaveGameStatistic(mGameStatistic);
			Game.Exit();
		}


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			sGameTime = gameTime;
			GraphicsDevice.Clear(Color.Black);
			mScreenManager.Draw();
			base.Draw(gameTime);
		}

		public static GraphicsDeviceManager Graphics { get { return sGraphics; } }
		public static AudioManager Audio { get { return sAudioManager; } }
		public new static ContentManager Content { get { return sContent; } }
		public static InputManager Input { get { return sInputManager; } }
		public static GameTime GameTime { get { return sGameTime; } }
		public static Game Game { get { return sGame; } }
		public static Random Random { get { return sRandom; } }
		public static HotKeys HotKey { get { return sSHotKeys; } }
	}
}
