/**
 * Author: David Spisla 
 * 
 * Concrete Class for the Achievements
 * Usage: This class is at the moment initialized in the Main.cs  
 * Missing: nothing
 * 
 **/

using System;

namespace ConversionOfBrutes.Misc.Statistics
{   
	[Serializable]
	class Achievements
	{
		
		public int Rampage { get; set; }
		public int FasterLight { get; set; }
		public int OverAllConverted { get; set; }
		public int OverAllKilled { get; set; }
		public int PauseOver15Times { get; set; }
	}
}
