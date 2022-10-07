/**
 * Original author: David Spisla 
 * 
 * Concrete Class for the Game
 * Usage: This Class Implements the Game ActionIdent. At the moment all students edit this file  
 * Missing: nothing
 * 
 **/

#define AI
//#define AI_DEBUG
#define MUSIC
//#define HARDWAREINSTANCING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConversionOfBrutes.AI;
using ConversionOfBrutes.AI.Pathfinding;
using ConversionOfBrutes.GameLogics;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.ParticleEffects;
using ConversionOfBrutes.Library;
using ConversionOfBrutes.Map;
using ConversionOfBrutes.Misc;
using ConversionOfBrutes.Misc.Statistics;
using ConversionOfBrutes.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using RVO;

namespace ConversionOfBrutes.Graphic.Screen 
{
	sealed class GameScreen : Screen
	{
		public class GameSaveInformation
		{
			public String mMapModellName;
			public String mMapTextureName;
			public NavigationGraph mNavigationGraph;
		}

		#region Private member variables
		private static ObjectManager sObjectManager;
		private static Camera sCamera;
		private static GraphicsManager sGraphicsManager;
		private static QuadTree<WorldObject> sMap;
		private static GameLogic sGameLogic;
		private static Hud sHud;
		private static Rectangle sGameScreenRectangle;
		private static AiAgent sAiAgent;
		private static Stopwatch sTimeSpan;
		//This bool is for the counter of the game statistic
		private static bool sGameNotFinished;


		// FPS Stuff
		private static int sFps;
		private int mFrameCount;
		private TimeSpan mFrameCountStart = new TimeSpan(0);

		// Editor Stuff
		private static bool sEditorMode;
		private MapEditor mMapEditor;

		// others
		private static GameSaveInformation sGameSaveInformation;
		private static int sVictoryThreshold = 25000;

		private static bool sAiDebug = false;
		private static bool sTechDemo;

		// fog of war 
		private static bool sFogOfWar = true;

		// particlemanager
		private static ParticleManager sParticleManager;

#if HARDWAREINSTANCING
		// GPU Manger for mesh instancing 
		private static GpuManager sGpuManager;
#endif

		#endregion
		public GameScreen(bool editorMode, GameStatistic gameStatistic)
		{
			sEditorMode = editorMode;

			sGraphicsManager = new GraphicsManager();
			Main.mGameStatistic = gameStatistic ?? new GameStatistic();
			sGameNotFinished = true;
			// TODO anpassen und schöner machen
			Area worldArea = new Area(0, 0, SaveAndLoad.WorldSize, SaveAndLoad.WorldSize);
			sMap = new QuadTree<WorldObject>(worldArea.Rectangle);

			// RVO start
			Simulator.Instance.Clear();
			var border = worldArea.GetBordersAsVectorArray();
			Array.Reverse(border); // has to be clockwise so the units can't leave the mapd
			Simulator.Instance.addObstacle(border);

			sCamera = new Camera(new Vector2(500f, 500f));

			sTimeSpan = new Stopwatch();
			sTimeSpan.Start();


			if(!sEditorMode)
			   Main.mAllOverGameStatistic.GamesStarted++;

			// ParticleManager
			sParticleManager = new ParticleManager();
#if HARDWAREINSTANCING
			// GPU manager
			sGpuManager = new GpuManager();
#endif
			Initialize();
		}
		

		public override void Initialize()
		{
			Main.Audio.StopSound(AudioManager.Sound.MainMenuMusic);
			Main.Audio.PlaySound(AudioManager.Sound.InGameMusic);
#if MUSIC
			//Main.Audio.PlaySound(AudioManager.Sound.BackgroundMusic);
#else
			Main.Audio.PauseSound(AudioManager.Category.Music);
			//Main.Audio.PauseSound(AudioManager.Category.Units);
			Main.Audio.SetVolume(AudioManager.Category.Units, 0);
#endif
			
			sObjectManager = new ObjectManager();
			

			#region Debug unit creation
			/*
			// TechDemo Units
			for (int j = 0; j < 8; j++)
			{
				for (int i = 0; i < 46; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoBarb, new Vector2(100 + 40 * i, 1350 + 40 * j), Fraction.Ai);
				}
			}
			for (int j = 0; j < 9; j++)
			{
				for (int i = 0; i < 11; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoBarb, new Vector2(1220 + 40 * i, 1670 + 40 * j), Fraction.Ai);
				}
			}
			for (int i = 0; i < 27; i++)
			{
				sObjectManager.CreateWorldObject(Ident.TechdemoBarb, new Vector2(860 + 40 * i, 1310), Fraction.Ai);
			}
			for (int i = 0; i < 6; i++)
			{
				sObjectManager.CreateWorldObject(Ident.TechdemoBarb, new Vector2(100 + 40 * i, 1310), Fraction.Ai);
			}


			for (int j = 0; j < 7; j++)
			{
				for (int i = 0; i < 15; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoAtlantic, new Vector2(400 + 40 * i, 50 + 40 * j), Fraction.Player);
				}
			}
			for (int j = 0; j < 11; j++)
			{
				for (int i = 0; i < 20; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoAtlantic, new Vector2(400 + 40 * i, 330 + 40 * j), Fraction.Player);
				}
			}
			for (int i = 0; i < 18; i++)
			{
				sObjectManager.CreateWorldObject(Ident.TechdemoAtlantic, new Vector2(400 + 40 * i, 770), Fraction.Player);
			}
			for (int j = 0; j < 11; j++)
			{
				for (int i = 0; i < 11; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoAtlantic, new Vector2(1580 + 40 * i, 330 + 40 * j), Fraction.Player);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 9; i++)
				{
					sObjectManager.CreateWorldObject(Ident.TechdemoAtlantic, new Vector2(40 + 40 * i, 410 + 40 * j), Fraction.Player);
				}
			}
			
			/*
			FogOfWar = false;
			sObjectManager.CreateWorldObject(Ident.EliteBarbarian, new Vector2(1800, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			//sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1300), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1350), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1370), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1390), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1300, 1000), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1320, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1340, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1360, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1380, 1000), Fraction.Player);
			//sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(1400, 1000), Fraction.Player);

			/* standard debug units
			#region Create units and world objects while serialization is not working
			sObjectManager.CreateWorldObject(Ident.EliteBarbarian, new Vector2(1800, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Axeman, new Vector2(1860, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.ArcherMounted, new Vector2(1880, 1330), Fraction.Ai);
			sObjectManager.CreateWorldObject(Ident.Archer, new Vector2(1900, 1330), Fraction.Ai);

			sObjectManager.CreateWorldObject(Ident.EliteAtlantic, new Vector2(100, 100), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.ShieldGuard, new Vector2(130, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.ShieldGuard, new Vector2(150, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Priest, new Vector2(170, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Priest, new Vector2(190, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.PriestRanged, new Vector2(210, 170), Fraction.Player);
			sObjectManager.CreateWorldObject(Ident.Knight, new Vector2(220, 170), Fraction.Player);
			*/
			#endregion


		}

		/// <summary>
		/// called, after all map objects are loaded
		/// </summary>
		public void MapLoaded()
		{
#if AI
			// no AI in Editor
			if (!sEditorMode)
			{
				sAiAgent = new AiAgent();
#if AI_DEBUG
				sAiDebug = true;
				FogOfWar = false;
#endif
			}
#endif
			sGameSaveInformation = new GameSaveInformation()
			{
				mMapModellName = SaveAndLoad.MapModellName,
				mMapTextureName = SaveAndLoad.MapTextureName,
				mNavigationGraph = SaveAndLoad.NavigationGraph
			};

			// should be done after everything is loaded into the map
			sGameLogic = new GameLogic();
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
			FogOfWar = false;
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
			if (GameLogic.AtlantisPoints >= sVictoryThreshold || (GameLogic.BarbContingent == 0 && ObjectManager.CountsUnitBarbarian == 0))
			{
				mManager.AddScreen(new DefeatAndVictoryScreen(true));
				if (!sEditorMode)
				   Main.mAllOverGameStatistic.GamesWon++;
			}
			else if (GameLogic.BarbPoints >= sVictoryThreshold || (GameLogic.AtlantisContingent == 0 && ObjectManager.CountsUnitAtlantis == 0))
			{
				mManager.AddScreen(new DefeatAndVictoryScreen(false));
				if (!sEditorMode)
				   Main.mAllOverGameStatistic.GamesLost++;
			}
		}

		public override void Update()
		{
			base.Update();
			if (sEditorMode == false)
			{
				WinOrLooseScreen();
				sGameLogic.Update();
#if AI
				sAiAgent.Update(Main.GameTime);
#endif
			}
			else
			{
				mMapEditor.Update();
			}
			Camera.Update();
			sObjectManager.Update();
			sParticleManager.Update();
			}

		public override void Draw()
		{
			mFrameCount++;
			// Draw the MainMap
			sGraphicsManager.DrawMap();
			if (!WorldObjectsHidden)
			{
				sObjectManager.Draw();
			}
#if HARDWAREINSTANCING
			sGpuManager.Draw();
#endif
			sParticleManager.Draw();

			
			if (sEditorMode)
			{
				mMapEditor.Draw();
			}

			if (FogOfWar)
			{
				// Draw Fog of War
				sGraphicsManager.DrawFogOfWar();
			}
			
			TimeSpan diff = Main.GameTime.TotalGameTime - mFrameCountStart;
			if (diff.TotalSeconds > 1)
			{
				mFrameCountStart = Main.GameTime.TotalGameTime;
				sFps = mFrameCount;
				mFrameCount = 0;
			}

		}

		#region Getter/Setter
		public static Camera Camera { get { return sCamera; } }
		public static ObjectManager ObjectManager { get { return sObjectManager; } }
		public static AiAgent AiAgent { get { return sAiAgent; } }
		public static GraphicsManager GraphicsManager { get { return sGraphicsManager; } }
		public static QuadTree<WorldObject> Map { get { return sMap; } }
		public static GameLogic GameLogic { get { return sGameLogic; } }
		public static Hud Hud { get { return sHud; } }
		public static Rectangle VisibleRectangle { get { return sGameScreenRectangle; } }
		public static int Fps { get { return sFps; } }
		public static Stopwatch Stopwatch { get { return sTimeSpan; } }
		public static bool GameNotFinished { get { return sGameNotFinished; } set { sGameNotFinished = value; } }
		public static GameDifficulty GameDifficulty { get; set; }
		public static bool MapEditorMode { get { return sEditorMode; } }
		public static bool WorldObjectsHidden { get; set; }
		public static int VictoryThreshold { get { return sVictoryThreshold; } }
		public static GameSaveInformation GameSaveInfo { get { return sGameSaveInformation; } }

		public static bool AiDebug { get { return sAiDebug; } }

		public static bool FogOfWar
		{
			get { return sFogOfWar; }
			set { sFogOfWar = value; }
		}

		public static bool TechDemo
		{
			get { return sTechDemo; }
			set { sTechDemo = value; }
		}
		public static ParticleManager ParticleManager
		{
			get { return sParticleManager; }
		}

		#endregion
	}
}
