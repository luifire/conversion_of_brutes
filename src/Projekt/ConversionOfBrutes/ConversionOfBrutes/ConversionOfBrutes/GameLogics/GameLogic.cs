using System;
using ConversionOfBrutes.GameObjects;
using ConversionOfBrutes.Graphic.Screen;
using ConversionOfBrutes.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

// Authors: buerklij

namespace ConversionOfBrutes.GameLogics
{
	sealed class GameLogic
	{
		[Serializable]
		public class GameLogicSavings
		{
			public double mAtlantisWinPoints;
			public double mBarbWinPoints;
			public int mAtlantisContingent;
			public int mBarbContingent;
		}

		private double mAtlantisWinPoints;
		private double mBarbWinPoints;
		private int mAtlantisContingent;
		private int mBarbContingent;
		private int mZonesAtl;

		public GameLogic()
		{
			mAtlantisWinPoints = 200;
			mBarbWinPoints = 200;

			mAtlantisContingent = 200;
			switch (GameScreen.GameDifficulty)
			{
				case GameDifficulty.Easy:
					mBarbContingent = 300;
					break;
				case GameDifficulty.Normal:
					mBarbContingent = 400;
					break;
				case GameDifficulty.Hard:
					mBarbContingent = 500;
					break;
			}	
			
			mZonesAtl = 0;

			foreach (Zone zone in GameScreen.ObjectManager.Zones)
			{
				if (zone.Fraction == Fraction.Player)
					mZonesAtl++;
			}
		}

		public GameLogicSavings GetSaveInformation()
		{
			return new GameLogicSavings()
			{
				mAtlantisWinPoints = mAtlantisWinPoints,
				mAtlantisContingent = mAtlantisContingent,
				mBarbContingent = mBarbContingent,
				mBarbWinPoints = mBarbWinPoints
			};
		}

		public void SetSaveInformation(GameLogicSavings saveings)
		{
			mAtlantisWinPoints = saveings.mAtlantisWinPoints;
			mAtlantisContingent = saveings.mAtlantisContingent;
			mBarbContingent = saveings.mBarbContingent;
			mBarbWinPoints = saveings.mBarbWinPoints;
		}

		public void Update()
		{
			HandleZones();
			UpdateSpawn();
			OccupiedLostAreas();
		}

		/// <summary>
		/// This lines are for the GameStatistic to get the numer of lost and received areas
		/// </summary>
		private void OccupiedLostAreas()
		{
			int zonesAtl = 0;

			foreach (Zone zone in GameScreen.ObjectManager.Zones)
			{
				if (zone.Fraction == Fraction.Player)
					zonesAtl++;
			}

			if (zonesAtl - mZonesAtl > 0)
			{
				if (!GameScreen.MapEditorMode)
					Main.mGameStatistic.OccupiedAreas++;
				mZonesAtl = zonesAtl;
			}
			else if (zonesAtl - mZonesAtl < 0)
			{
				if (!GameScreen.MapEditorMode)
					Main.mGameStatistic.LostAreas--;
				mZonesAtl = zonesAtl;
			}
		}

		private void HandleZones()
		{
			double winPointGain = Main.GameTime.ElapsedGameTime.Milliseconds*0.04;

			foreach (var zone in GameScreen.ObjectManager.Zones)
			{
				var multiplier = 0;
				bool anyUnitInZone = false; 
				var objectsInZone = GameScreen.Map.GetObjects(zone.ConquerRect);

				if (!(zone is HomeZone))
				{

					foreach (var gameObj in objectsInZone)
					{
						if (gameObj is Unit)
						{
							// check if unit is in the circle and cap multiplier
							if (Vector2.Distance(zone.Position, gameObj.Position) > zone.ConquerCircleSize)
								continue;

							anyUnitInZone = true;
							Unit unit = (Unit) gameObj;
							if (unit.Fraction == Fraction.Ai && zone.Fraction == Fraction.Ai)
							{
								((AttackUnit)unit).Regenerate();
							}
							if (unit.Fraction == Fraction.Player)
							{
								// true -> there is at least one Ai-unit in the Zone. Capturing impossible
								if (multiplier < 0)
								{
									multiplier = 0;
									break;
								}
								if (multiplier < 7)
									multiplier++;
							}
							else
							{
								// true -> there is at least one player-unit in the Zone. Capturing impossible
								if (multiplier > 0)
								{
									multiplier = 0;
									break;
								}
								if (multiplier > -7)
									multiplier--;
							}
						}
					}

					if (!anyUnitInZone)
					{
						switch (zone.Fraction)
						{
							case Fraction.Ai:
								multiplier = -1;
								break;
							case Fraction.Player:
								multiplier = 1;
								break;
							case Fraction.Gaia:
								multiplier = (zone.FractionPoints > 0) ? -1 : 1;
								break;
						}
					}
					zone.FractionPoints += multiplier * Main.GameTime.ElapsedGameTime.Milliseconds * 0.005;
					if (zone.FractionPoints >= 500)
					{
						zone.FractionPoints = 500;
						zone.Fraction = Fraction.Player;
					}
					else if (zone.FractionPoints <= 50 && zone.FractionPoints >= -50)
					{
						zone.Fraction = Fraction.Gaia;
					}
					else if (zone.FractionPoints <= -500)
					{
						zone.FractionPoints = -500;
						zone.Fraction = Fraction.Ai;

					}
					zone.HealthBar.Update();
				}

				if (zone.Ident != Ident.Spawnzone)
				{
					switch (zone.Fraction)
					{
						case Fraction.Ai:
							mBarbWinPoints += winPointGain;
							break;
						case Fraction.Player:
							mAtlantisWinPoints += winPointGain;
							if (!GameScreen.MapEditorMode)
								Main.mGameStatistic.OverallReceivedWinpoints += winPointGain;
							break;

					}
				}
			}
		}

		private void UpdateSpawn()
		{

			foreach (var zone in GameScreen.ObjectManager.Zones)
			{
				if (!(zone is SpawnZone) || ((SpawnZone) zone).SpawnJobs.Count == 0)
					continue;


				var spawnJob = ((SpawnZone) zone).SpawnJobs.First.Value;
				spawnJob.TimeRemaining -= Main.GameTime.ElapsedGameTime.Milliseconds;

				if (spawnJob.TimeRemaining <= 0)
				{
					Unit u = (Unit)GameScreen.ObjectManager.CreateWorldObject(spawnJob.UnitIdent,
						spawnJob.SpawnZone.SpawnPosition, spawnJob.UnitFraction);
					u.WalkToPosition(spawnJob.SpawnZone.UnitDestination, spawnJob.SpawnZone.ObjectNumber);

					((SpawnZone)zone).SpawnJobs.RemoveFirst();
				}
			}
		}

		/// <summary>
		/// Adds a Unit to the spawn queue
		/// </summary>
		/// <param name="unitToSpawn"></param>
		/// <param name="spawnZone"></param>
		/// <param name="fraction"></param>
		/// <returns></returns>
		public void SpawnUnit(Ident unitToSpawn, SpawnZone spawnZone, Fraction fraction)
		{
			double points=0;
			int cont = 0;
			switch(fraction)
			{
				case Fraction.Player:
					points = mAtlantisWinPoints;
					cont = mAtlantisContingent;
					break;
				case Fraction.Ai:
					points = mBarbWinPoints;
					cont = mBarbContingent;
					break;
			}

			if (spawnZone.SpawnJobs.Count >= 21 || cont == 0)
				return;


			int time = GetTimeCost(unitToSpawn);
			int winPointCost = GetWinPointCost(unitToSpawn);

			if (points - winPointCost < 0)
				return;

			spawnZone.SpawnJobs.AddLast(new SpawnJob(time, winPointCost, spawnZone, unitToSpawn, fraction));

			switch (fraction)
			{
				case Fraction.Player:
					mAtlantisWinPoints -= winPointCost;
					if (!GameScreen.MapEditorMode)
					   Main.mGameStatistic.OverallLostWinpoints -= winPointCost;
					mAtlantisContingent--;
					break;
				case Fraction.Ai:
					mBarbWinPoints -= winPointCost;
					mBarbContingent--;
					break;
			}
		}

		public int GetTimeCost(Ident unitToSpawn)
		{
			switch (unitToSpawn)
			{
				//Atlantic
				case Ident.ShieldGuard:
					return 5;
				case Ident.Priest:
					return 3;
				case Ident.PriestRanged:
					return 5;
				case Ident.EliteAtlantic:
					return 10;
				case Ident.Witch:
					return 10;
				case Ident.Beast:
					return 15;

				//Barbarian
				case Ident.EliteBarbarian:
					return 10;
				case Ident.Archer:
					return 3;
				case Ident.ArcherMounted:
					return 5;
				case Ident.Axeman:
					return 2;
				case Ident.Knight:
					return 4;

				default:
					throw new ArgumentOutOfRangeException("unitToSpawn", unitToSpawn, null);
			}
		}

		public int GetWinPointCost(Ident unitToSpawn)
		{
			switch (unitToSpawn)
			{
				//Atlantic
				case Ident.ShieldGuard:
					return 150;
				case Ident.Priest:
					return 150;
				case Ident.PriestRanged:
					return 150;
				case Ident.EliteAtlantic:
					return 500;
				case Ident.Witch:
					return 400;
				case Ident.Beast:
					return 800;

				//Barbarian
				case Ident.EliteBarbarian:
					return 500;
				case Ident.Archer:
					return 120;
				case Ident.ArcherMounted:
					return 150;
				case Ident.Axeman:
					return 80;
				case Ident.Knight:
					return 180;

				default:
					throw new ArgumentOutOfRangeException("unitToSpawn", unitToSpawn, null);
			}
		}

		public void CancelSpawn(SpawnJob job)
		{
			switch (job.UnitFraction)
			{
				case Fraction.Ai:
					mBarbWinPoints += job.Cost;
					mBarbContingent++;
					break;
				case Fraction.Player:
					mAtlantisWinPoints += job.Cost;
					mAtlantisContingent++;
					break;
			}
			job.SpawnZone.SpawnJobs.Remove(job);
			
			if (!GameScreen.MapEditorMode)
			   Main.mGameStatistic.RecruitedUnits--;
		}


		public double BarbPoints { get { return mBarbWinPoints; } }
		public double AtlantisPoints { get { return mAtlantisWinPoints; } }
		public int AtlantisContingent { get { return mAtlantisContingent;} }
		public int BarbContingent { get { return mBarbContingent; } }
	}
}
