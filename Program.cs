namespace DataAnalysisTool
{
    /// <summary>
    /// Represents the entry point of the Data Analysis Tool program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main method of the program.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the input filename as a command-line argument.");
                return;
            }

            DataAnalyzer analyzer = new();
            analyzer.Run(args[0], true, args[1..]);
        }
    }
}