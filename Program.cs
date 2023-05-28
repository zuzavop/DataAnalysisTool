namespace DataAnalysisTool
{
    internal class Program
    {
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