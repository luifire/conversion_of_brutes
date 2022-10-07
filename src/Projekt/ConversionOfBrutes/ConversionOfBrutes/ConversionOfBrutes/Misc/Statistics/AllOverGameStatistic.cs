/**
 * Author: David Spisla 
 * 
 * Concrete Class for the OverAllGameStatistic
 * Usage: This class is at the moment initialized in the Main.cs  
 * Missing: nothing
 * 
 **/

using System;

namespace ConversionOfBrutes.Misc.Statistics
{
	[Serializable]
	public class AllOverGameStatistic
	{
		/// <summary>
		/// Here are the statistic values for all Games according to the GDD
		/// </summary>
		public int GamesStarted { get; set; }
		public int GamesWon { get; set; }
		public int GamesLost { get; set; }
		public int GamesNotFinished { get; set; }
	}
}
