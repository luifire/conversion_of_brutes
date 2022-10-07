using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ConversionOfBrutes.AI;
using ConversionOfBrutes.AI.Pathfinding;
using ConversionOfBrutes.GameLogics;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Map;
using ConversionOfBrutes.Misc.Statistics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using RVO;

namespace ConversionOfBrutes.Misc
{
	static class SaveAndLoad
	{
		[Serializable]
		private class MapSave
		{
			public String mMapTextureName;
			public String mMapModellName;
			public LinkedList<WorldObject> mMapObjects;
			public NavigationGraph mNavigationGraph;
		}

		[Serializable]
		private class GameStateSave
		{
			public string mDate;

			public MapSave mMapSave;
			public LinkedList<Unit> mUnits;
			public LinkedList<Zone> mZones;
			public GameStatistic mGameStatistic;

			public SelectionHandler.SelectionSave mSelectionSave;
			public AiAgent.AiSave mAiSave;
			public GameDifficulty mGameDifficulty;
			public GameLogic.GameLogicSavings mGameLogicSavings;
			public Vector3 mCameraPosition;
		}

#if DEBUG
		private const string Root = "..\\..\\..\\..\\ConversionOfBrutesContent\\";
#else
		private const string Root = "";
#endif
		private const string SavePath = Root + "Saves\\";
		private const string MapPath = Root + "Maps\\";
		private const string SaveEnding = ".sav";
		private const string MapEnding = ".map";
		//private const string MapTemplate = "template_";

		private const string OverallStatisticPath = Root + "AllOverGameStatistic.sav";
		private const string GameStatisticPath = Root + "GameStatistic.sav";
		private const string AchievementsPath = Root + "Achievements.sav";
		private const string SettingsPath = Root + "Settings.sav";

		private static GameStateSave sGameStateSave;
		private static MapSave sMapSave;
		private static GameScreen sGameScreen;
		private static bool sEditorMod;

		// old unit, new unit
		private static Dictionary<Unit, Unit> sOldNewUnitRelation;
		private static Dictionary<WorldObject, WorldObject> sOldNewWorldObjRelation;

		public const int WorldSize = 2000;

		private static MapSave CreateMapSave(bool editorMode)
		{
			return new MapSave()
			{
				mMapModellName = GameScreen.GameSaveInfo.mMapModellName,
				mMapTextureName = GameScreen.GameSaveInfo.mMapTextureName,
				mMapObjects = GameScreen.ObjectManager.MapObjects,
				mNavigationGraph = (editorMode) ? NavigationMeshEditor.Instance.GetNavigationGraphSaveInfo() : GameScreen.GameSaveInfo.mNavigationGraph
			};
		}

		/// <summary>
		/// only invoked from editor
		/// </summary>
		public static void SaveMap(String fileName)
		{
			SaveObject(MapPath + fileName + MapEnding, CreateMapSave(true));
		}

		public static void SaveGame(int saveIdx)
		{
			foreach (var unit in GameScreen.ObjectManager.Units)
			{
				unit.PrepareToBeSaved();
			}

			GameStateSave gameState = new GameStateSave()
			{
				mDate = DateTime.Now.ToString("dd.MM. HH-mm-ss"),
				mMapSave = CreateMapSave(false),
				mUnits = GameScreen.ObjectManager.Units,
				mZones = GameScreen.ObjectManager.Zones,
				mSelectionSave = GameScreen.ObjectManager.SelectionHandler.GetSaveInformation(),
				mAiSave = GameScreen.AiAgent.GetSaveInformation(),
				mGameDifficulty = GameScreen.GameDifficulty,
				mGameLogicSavings = GameScreen.GameLogic.GetSaveInformation(),
				mCameraPosition = GameScreen.Camera.Position,
				mGameStatistic = Main.mGameStatistic
			};

			SaveObject(SavePath + saveIdx + SaveEnding, gameState);
		}

		public static void LoadSaveGame(int saveIdx)
		{
			GameStateSave gss = (GameStateSave)LoadObject(SavePath + saveIdx + SaveEnding);

			InitializeGameStep1(gss.mMapSave, gss, false);
		}

		private static void LoadUnits(LinkedList<Unit> units)
		{
			sOldNewUnitRelation = new Dictionary<Unit, Unit>();
			
			// Create the objects
			foreach (var oldUnit in units)
			{
				Unit newUnit = (Unit)GameScreen.ObjectManager.CreateWorldObject(oldUnit.Ident, oldUnit.Position, oldUnit.Fraction);

				sOldNewUnitRelation.Add(oldUnit, newUnit);
			}

			// make the units copy themselves
			foreach (var unit in sOldNewUnitRelation)
			{
				unit.Value.CopyFromUnit(unit.Key);
			}
		}


		/// <summary>
		/// get the new referenze to the old unit
		/// </summary>
		/// <param name="oldUnit"></param>
		/// <returns></returns>
		public static Unit GetNewUnit(Unit oldUnit)
		{
			return sOldNewUnitRelation.ContainsKey(oldUnit) ? sOldNewUnitRelation[oldUnit] : null;
		}

		private static WorldObject GetNewWorldObj(WorldObject oldObj)
		{
			return sOldNewWorldObjRelation.ContainsKey(oldObj) ? sOldNewWorldObjRelation[oldObj] : null;
		}

		/// <summary>
		/// creates the same list but with correct references
		/// </summary>
		/// <param name="oldList"></param>
		/// <returns></returns>
		public static LinkedList<WorldObject> GenerateNewList(LinkedList<WorldObject> oldList)
		{
			var newList = new LinkedList<WorldObject>();

			foreach (var oldObj in oldList)
			{
				WorldObject newObj = (oldObj is Unit) ? GetNewUnit((Unit)oldObj) : GetNewWorldObj(oldObj);
				if (newObj != null)
					newList.AddLast(newObj);
			}

			return newList;
		}

		public static LinkedList<Unit> GenerateNewList(LinkedList<Unit> oldList)
		{
			var newList = new LinkedList<Unit>();

			foreach (var oldObj in oldList)
			{
				Unit newObj = GetNewUnit(oldObj);
				if (newObj != null)
					newList.AddLast(newObj);
			}

			return newList;
		}

		private static void InitializeGameStep1(MapSave map, GameStateSave gameStateSave, bool editMode)
		{
			sMapSave = map;
			sEditorMod = editMode;
			sGameStateSave = gameStateSave;

			ScreenManager.Instance.AddScreen(new LoadScreen());
		}

		private class Line
		{
			public Vector2 m1;
			public Vector2 m2;

			public Line(Vector2 v1, Vector2 v2)
			{
				m1 = v1;
				m2 = v2;
			}
		}

		private static Vector2? FindNodeAndRemove(LinkedList<Line> borders, Vector2 find)
		{
			Vector2? found = null;
			foreach (var node in borders)
			{
				if (node.m1 == find)
					found = node.m2;
				else if (node.m2 == find)
					found = node.m1;

				if (found != null)
				{
					borders.Remove(node);
					return found;
				}
			}

			return null;
		}

		private static void LoadObstacles(NavigationGraph navGraph)
		{
			LinkedList<Line> borders = new LinkedList<Line>();

			foreach (var node in navGraph.GetBorderEdges())
			{
				borders.AddLast(new Line(node.Item1, node.Item2));
			}

			var currentNode = borders.First;
			while (currentNode != null)
			{
				bool loopFound = false;
				var startNode = currentNode;
				LinkedList<Vector2> circle = new LinkedList<Vector2>();

				circle.AddFirst(startNode.Value.m1);
				borders.Remove(startNode);

				var currentCircleNode = startNode.Value.m1;
				// first all left nodes
				while (true)
				{
					Vector2? nextPoint = FindNodeAndRemove(borders, currentCircleNode);
					if (nextPoint != null)
					{
						if (circle.Last.Value != nextPoint)
						{
							circle.AddFirst((Vector2) nextPoint);
							currentCircleNode = (Vector2) nextPoint;
						}
						else
						{
							loopFound = true;
						}
					}
					else
						break;
				}
				
				circle.AddLast(startNode.Value.m2);
				currentCircleNode = startNode.Value.m2;

				// first all left nodes
				while (loopFound == false)
				{
					Vector2? nextPoint = FindNodeAndRemove(borders, currentCircleNode);
					if (nextPoint != null)
					{
						if (circle.First.Value != nextPoint)
						{
							circle.AddLast((Vector2) nextPoint);
							currentCircleNode = (Vector2) nextPoint;
						}
						else
						{
							loopFound = true;
						}
					}
					else
						break;
				}

				// when it's the circle that sourrounds the world, skip it
				bool notAndObstacle = false;
				Vector2 worldSize = new Vector2(WorldSize, WorldSize);
				foreach (var node in circle)
				{
					if (Vector2.Distance(node, Vector2.Zero) < 10 || Vector2.Distance(node, worldSize) < 10)
					{
						notAndObstacle = true;
						break;
					}
				}

				// copy to simulator
				if(notAndObstacle == false)
				{ 
					Vector2[] ccircle = new Vector2[circle.Count];
					circle.CopyTo(ccircle, 0);
					Simulator.Instance.addObstacle(ccircle);

					Array.Reverse(ccircle);
					Simulator.Instance.addObstacle(ccircle);
				}

				// because next might be erased
				currentNode = borders.First;
			}
		}

		public static void InitializeGameStep2()
		{
			// starts the game - NavMesh is loaded within Gamescreen, access by getter
			sGameScreen = new GameScreen(sEditorMod, sGameStateSave == null ? null : sGameStateSave.mGameStatistic);
			if (GameScreen.AiDebug)
			{
				GameScreen.FogOfWar = true;
			}

			sOldNewWorldObjRelation = new Dictionary<WorldObject, WorldObject>();
			
			// Loads Objects in game
			foreach (var oldObj in sMapSave.mMapObjects)
			{
				WorldObject newObj = GameScreen.ObjectManager.CreateWorldObject(oldObj.Ident, oldObj.Position, oldObj.Fraction);
				// zone have different states, when ingame
				if (oldObj is Zone && sGameStateSave != null && sGameStateSave.mZones.Contains((Zone)oldObj))
				{
					var linkedListNode = sGameStateSave.mZones.Find((Zone)oldObj);
					if (linkedListNode != null)
					{
						sOldNewWorldObjRelation.Add(linkedListNode.Value, newObj);
						((Zone)newObj).CopyFromZone(linkedListNode.Value);
					}
				}
				else
					sOldNewWorldObjRelation.Add(oldObj, newObj);
			}

			LoadObstacles(sMapSave.mNavigationGraph);
			Simulator.Instance.processObstacles();

			sGameScreen.MapLoaded();
			if (sEditorMod)
			{
				sGameScreen.InitializeMapEditor();
				NavigationMeshEditor.Instance.SetNavigationGraphInfo();
			}

			// if a saved game should be loaded
			if (sGameStateSave != null)
			{ 
				LoadUnits(sGameStateSave.mUnits);

				// invoke all Objects that have lists
				GameScreen.ObjectManager.SelectionHandler.SetSaveInformation(sGameStateSave.mSelectionSave);
				GameScreen.AiAgent.SetSaveInformation(sGameStateSave.mAiSave);
				GameScreen.GameDifficulty = sGameStateSave.mGameDifficulty;
				GameScreen.GameLogic.SetSaveInformation(sGameStateSave.mGameLogicSavings);
				GameScreen.Camera.Position = sGameStateSave.mCameraPosition;
			}

			// remove Load Screen
			ScreenManager.Instance.RemoveScreen();
			// add gamescreen
			ScreenManager.Instance.AddScreen(sGameScreen);
		}

		/// <summary>
		/// Actually starts and loads the game
		/// </summary>
		private static void LoadMap(String mapName, bool editMode)
		{
			MapSave map = (MapSave)LoadObject(MapPath + mapName + MapEnding);
			InitializeGameStep1(map, null, editMode);
		}

		public static void LoadMapEditor(String mapName)
		{
			LoadMap(mapName, true);
		}

		public static void LoadNewGame(String mapName)
		{
			LoadMap(mapName, false);
		}

		public static List<string> GetMapList()
		{
			List<string> result = new List<string>();
			var maps = Directory.GetFiles(MapPath, "*" + MapEnding);

			for (int i = 0; i < 10; i++)
			    result.Add("empty");
		
			
			foreach (var map in maps)
			{
				string fileName = Path.GetFileNameWithoutExtension(map);
				if (fileName == null) continue;

				int idx = -1;
				try
				{
					idx = int.Parse(fileName);
				}
				catch
				{
					// ignored
				}

				if (idx >= 0 && idx < 10)
				{
					if (idx == 0 || idx == 1)
					{
						result[idx] = "Map" + Path.GetFileNameWithoutExtension(fileName) + " (default)";
					}
					else
					{
						result[idx] = "Map" + Path.GetFileNameWithoutExtension(fileName);
					}
					
				}

				//Old Version: Changed at 20.07.2015 16:51 Uhr
				// normal => without template
				/*if ((editorMode == false) || fileName.StartsWith(MapTemplate) == false)
					result[temp] = Path.GetFileNameWithoutExtension(fileName);*/

				
			}

			return result;
		}

		public static List<string> GetSaveList()
		{
			List<string> result = new List<string>();
			var saves = Directory.GetFiles(SavePath, "*" + SaveEnding);

			for (int i = 0; i < 10; i++)
				result.Add("empty");

			foreach (var save in saves)
			{
				string fileName = Path.GetFileNameWithoutExtension(save);
				if(fileName == null || Char.IsLetter(fileName[0])) continue;

				int idx = -1;
				try
				{
					idx = int.Parse(fileName);
				}
				catch
				{
					// ignored
				}
				
				if (idx >= 0 && idx < 10)
				{
					GameStateSave state = (GameStateSave)LoadObjectSavely(save);
					if (state != null)
						result[idx] = state.mDate;
				}
			}

			return result;
		} 

		/// <summary>
		/// may throw exceptions
		/// </summary>
		/// <param name="path"></param>
		/// <param name="obj"></param>
		private static void SaveObject(String path, object obj)
		{
			string path1 = Path.GetFullPath(path);
			FileStream fs = new FileStream(path1, FileMode.Create);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, obj);

			fs.Close();
		}

		/// <summary>
		/// may throw exceptions
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static object LoadObject(String path)
		{
			string path1 = Path.GetFullPath(path);
			FileStream fs = new FileStream(path1, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			object obj = bf.Deserialize(fs);
			fs.Close();

			return obj;
		}

		/// <summary>
		/// returns null, if faild
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static object LoadObjectSavely(String path)
		{
			object obj;
			try
			{
				obj = LoadObject(path);
			}
			catch (Exception)
			{
				obj = null;
			}

			return obj;
		}

		/// <summary>
		/// Saving the Achievements
		/// </summary>
		/// <param name="achievements"></param>
		public static void SaveAchievements(Achievements achievements)
		{
			SaveObject(AchievementsPath, achievements);
		}

		/// <summary>
		/// Saving the GameStatistic
		/// </summary>
		/// <param name="gameStat"></param>
		public static void SaveGameStatistic(GameStatistic gameStat)
		{
			SaveObject(GameStatisticPath, gameStat);
		}

		/// <summary>
		/// Saving the OverallGameStatistic
		/// </summary>
		/// <param name="gameSat"></param>
		public static void SaveOverallGameStatistic(AllOverGameStatistic gameSat)
		{
			SaveObject(OverallStatisticPath, gameSat);
		}

		/// <summary>
		/// sound, resolution etc
		/// </summary>
		/// <param name="options"></param>
		public static void SaveSettings(OptionScreen.Settings options)
		{
			SaveObject(SettingsPath, options);
		}

		/// <summary>
		/// Loading the Achievements
		/// </summary>
		/// <returns></returns>
		public static Achievements LoadAchievements()
		{
			return (Achievements)LoadObjectSavely(AchievementsPath) ?? new Achievements();
		}

		/// <summary>
		/// Loading the GameStatistic
		/// </summary>
		/// <returns></returns>
		public static GameStatistic LoadGameStatistic()
		{
			return (GameStatistic)LoadObjectSavely(GameStatisticPath) ?? new GameStatistic();
		}

		/// <summary>
		/// Loading the OverallGameStatistic
		/// </summary>
		/// <returns></returns>
		public static AllOverGameStatistic LoadOverallGameStatistic()
		{
			return (AllOverGameStatistic)LoadObjectSavely(OverallStatisticPath) ?? new AllOverGameStatistic();
		}

		public static OptionScreen.Settings LoadSettings()
		{
			return (OptionScreen.Settings) LoadObjectSavely(SettingsPath);
		}


		public static String MapTextureName { get { return sMapSave.mMapTextureName; } }
		public static String MapModellName { get { return sMapSave.mMapModellName; } }
		public static NavigationGraph NavigationGraph { get { return sMapSave.mNavigationGraph; } }
	}
}
