/**
 * Author: David Spisla 
 * 
 * Concrete Class for the GameStatistic 
 * Usage: This Class should be initialized when a new GameScreen is started 
 * Missing: nothing
 * 
 **/

using System;

namespace ConversionOfBrutes.Misc.Statistics
{
    [Serializable]
    public sealed class GameStatistic
    {
	    /// <summary>
	    /// Here are the statistic values for one Game according to the GDD
	    /// </summary>
	    public int RecruitedUnits { get; set; }
		public int ConvertedUnits { get; set; }
		public int KilledUnits { get; set; }
		public int LostUnits { get; set; }
		public double OverallReceivedWinpoints { get; set; }
		public int OverallLostWinpoints { get; set; }
		public int OccupiedAreas { get; set; }
		public int LostAreas { get; set; }
		public double DurationOfGame { get; set; }

		/// <summary>
		/// This values is one of the achievements
		/// </summary>
		public int PauseOver15Times { get; set; }
		/*
		public void LoadOldValues()
		{
			try
			{
				GameStatistic obj = SaveAndLoad.LoadGameStatistic();
				RecruitedUnits = obj.RecruitedUnits;
				ConvertedUnits = obj.ConvertedUnits;
				KilledUnits = obj.KilledUnits;
				LostUnits = obj.LostUnits;
				OverallReceivedWinpoints = obj.OverallReceivedWinpoints;
				OverallLostWinpoints = obj.OverallLostWinpoints;
				OccupiedAreas = obj.OccupiedAreas;
				LostAreas = obj.LostAreas;
				DurationOfGame = obj.DurationOfGame;
				PauseOver15Times = obj.PauseOver15Times;
			}
			catch (FileNotFoundException){}
			catch (NullReferenceException){}
			
		   
		}
		*/
    }
}
