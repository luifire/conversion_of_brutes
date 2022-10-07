//using System.IO;

namespace ConversionOfBrutes
{
#if WINDOWS || XBOX
    static class Program
    {
	    private static Main sGame;

	    /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
			using (sGame = new Main())
            {
                sGame.Run();
            }
        }
    }
#endif
}

