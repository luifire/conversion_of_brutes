/**
 * Original author: David Spisla 
 * 
 * Concrete Class for the Game
 * Usage: This Class Implements the Game ActionIdent. At the moment all students edit this file  
 * Missing: 
 * - full functionality of the game
 **/

#define AI
//#define MUSIC

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConversionOfBrutes.AI;
using ConversionOfBrutes.GameLogics;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Library;
using ConversionOfBrutes.Map;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using RVO;

namespace ConversionOfBrutes.Graphic.Screen
{
	sealed class GameScreen : Screen
	{
		public enum Type
		{
			Game,
			MapEditor,
			NavMeshEditor
		}

		#region Private member variables
		private static ObjectManager sObjectManager;
		// Camera
		private static Camera sCamera;
		// GraphicsManager
		private static GraphicsManager sGraphicsManager;

		private static HotKeys sHotKeys;

		private static QuadTree<WorldObject> sMap;

		private static GameLogic sGameLogic;

		private static Hud sHud;

		private static Rectangle sGameScreenRectangle;

		private static AiStateMachine sAiStateMachine;

		private static GameStatistic sGameStatistic;

		private static Stopwatch sTimeSpan;
		//This bool is for the counter of the game statistic
		private static bool sGameFinished;

		// FPS Stuff
		private static int sFps;
		private int mFrameCount;
		private TimeSpan mFrameCountStart = new TimeSpan(0);

		//Editor Mode
		private readonly Type mType;

		// Map Editor
		private MapEditor mMapEditor;
		#endregion
		public GameScreen(Type type = Type.Game)
		{
			mType = type;
			mTranslucent = false;
			sGraphicsManager = new GraphicsManager();
			Main.mGameStatistic.InitializeWithZero();
			sGameFinished = true;
			// TODO anpassen und schöner machen
			Area worldArea = new Area(0, 0, 2000, 2000);
			sMap = new QuadTree<WorldObject>(worldArea.Rectangle);
			var border = worldArea.GetBordersAsVectorArray();
			// has to be clockwise so the units can't leave the mapd
			Array.Reverse(border);
			Simulator.Instance.addObstacle(border);


			sCamera = new Camera(new Vector2(500f, 500f));
			sTimeSpan = new Stopwatch();
			sTimeSpan.Start();
			Main.mAllOverGameStatistic.GamesStarted++;
			sHotKeys = new HotKeys();

			Initialize();
			// should be done after everything is loaded into the map
			sGameLogic = new GameLogic();
			sMapEditorMode = false;
		}
		

		public override void Initialize()
		{
#if MUSIC
			//Main.Audio.PlaySound(AudioManager.Sound.BackgroundMusic);
#else
			Main.Audio.SetVolume(AudioManager.Category.Units, 0);
			Main.Audio.SetVolume(AudioManager.Category.Music, 0);
#endif
			
			sObjectManager = new ObjectManager();
			/*
			#region Create units and world objects while serialization is not working
			sObjectManager.CreateWorldObject(Ident.EliteBarbarian, new Vector2(1800, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Archer, new Vector2(1900, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1720, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1740, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1760, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1780, 1000), Fraction.Player);

			sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(100, 100), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.ShieldGuard, new Vector2(130, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.ShieldGuard, new Vector2(150, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Priest, new Vector2(170, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Priest, new Vector2(190, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.PriestRanged, new Vector2(210, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Knight, new Vector2(220, 170), Fraction.Player);

			// while serilization is not working
			sObjectManager.CreateWorldObject(Ident.Spawnzone, new Vector2(100, 100), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(300, 100), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Spawnzone, new Vector2(1700, 1400), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(1900, 1400), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Spawnzone, new Vector2(700, 700), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Spawnzone, new Vector2(1300, 800), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(950, 1200), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(1500, 400), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(400, 1000), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(1800, 200), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(650, 200), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(1650, 900), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Zone, new Vector2(150, 800), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Castle, new Vector2(200, 400), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Castle, new Vector2(230, 500), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Castle, new Vector2(400, 350), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Castle, new Vector2(550, 450), Fraction.Gaia);
			sObjectManager.CreateWorldObject(Ident.Castle, new Vector2(150, 250), Fraction.Gaia);
			Random random = new Random();
			for(int i = 1; i <= 150; i++)
			{
				sObjectManager.CreateWorldObject(Ident.Tree, new Vector2(random.Next(0,2000),random.Next(0,1500)), Fraction.Gaia);
			}
			#endregion
			*/
		}

		/// <summary>
		/// called, after all map objects are loaded
		/// </summary>
		public void MapLoaded()
		{
#if AI
			// no AI in Editor
			if (mType == Type.Game)
				sAiStateMachine = new AiStateMachine();
#endif
		}

		public void InitializeMapEditor()
		{
			mMapEditor = new MapEditor();
			// Questionable...
			LinkedList<Unit> unitsDummy = new LinkedList<Unit>();
			foreach (Unit unit in ObjectManager.Units)
			{
				unitsDummy.AddLast(unit);
			}
			foreach (Unit unit in unitsDummy)
			{
				ObjectManager.UnitDied(unit);
			}
		}

		public override void ScreenAdded()
		{
			sHud = new Hud();
			mManager.AddScreen(sHud);

			sGameScreenRectangle = new Rectangle(0, sHud.UpperHudRectangle.Height, Main.Graphics.PreferredBackBufferWidth,
				Main.Graphics.PreferredBackBufferHeight - sHud.UpperHudRectangle.Height - sHud.LowerHudRectangle.Height);
			// has to be done here, because otherweise the screen rect is not known
			sCamera.CalcVisibleRectange();
		}
		private void WinOrLooseScreen()
		{
			if (GameLogic.AtlantisPoints >= 2500 || (GameLogic.BarbContingent == 0 && ObjectManager.CountsUnitBarbarian == 0))
			{
				mManager.GetPeek().UpdateDeactivate();
				mManager.GetSecondPeek().UpdateDeactivate();
				mManager.AddScreen(new VictoryScreen());
				Main.mAllOverGameStatistic.GamesWon++;
			}
			else if (GameLogic.BarbPoints >= 2500 || (GameLogic.AtlantisContingent == 0 && ObjectManager.CountsUnitAtlantis == 0))
			{
				mManager.GetPeek().UpdateDeactivate();
				mManager.GetSecondPeek().UpdateDeactivate();
				mManager.AddScreen(new DefeatScreen());
				Main.mAllOverGameStatistic.GamesLost++;
			}
		}

		public override void Update(GameTime gameTime)
		{
			if (mType == Type.Game)
			{
				WinOrLooseScreen();
				sGameLogic.Update();
#if AI
				sAiStateMachine.Update(gameTime);
#endif
			}
			else
			{
				mMapEditor.Update();
			}
			Camera.Update();
			sObjectManager.Update();
		}

		public override void Draw()
		{
			mFrameCount++;
			// Draw the MainMap
			sGraphicsManager.DrawMap();
			sObjectManager.Draw();

			if (mType == Type.MapEditor)
			{
				mMapEditor.Draw();
			}

			TimeSpan diff = Main.GameTime.TotalGameTime - mFrameCountStart;
			if (diff.Seconds > 1)
			{
				mFrameCountStart = Main.GameTime.TotalGameTime;
				sFps = mFrameCount;
				mFrameCount = 0;
			}
		}

		#region Getter/Setter
		public static Camera Camera { get { return sCamera; } }
		public static ObjectManager ObjectManager { get { return sObjectManager; } }
		public static AiStateMachine AiStateMachine { get { return sAiStateMachine; } }
		public static GraphicsManager GraphicsManager { get { return sGraphicsManager; } }
		public static HotKeys HotKey { get { return sHotKeys; } }
		public static QuadTree<WorldObject> Map { get { return sMap; } }
		public static GameLogic GameLogic { get { return sGameLogic; } }
		public static Hud Hud { get { return sHud; } }
		public static Rectangle VisibleRectangle { get { return sGameScreenRectangle; } }
		public static int Fps { get { return sFps; } }
		public static Stopwatch Stopwatch { get { return sTimeSpan; } }
		public static bool GameFinished { get { return sGameFinished; } set { sGameFinished = value; } }
		public static bool MapEditorMode { get { return sMapEditorMode; } }
		#endregion
	}
}
