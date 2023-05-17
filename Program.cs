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
            else
            {
                DataAnalyzer analyzer = new(args);
                analyzer.Run();
            }
        }
    }
}