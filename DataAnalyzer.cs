namespace DataAnalysisTool
{
    class DataAnalyzer
    {
        readonly Dictionary<string, AnalyzeFunc> commands;
        Dataset inputDataset;

        public DataAnalyzer()
        {
            this.commands = new Dictionary<string, AnalyzeFunc>();
            this.inputDataset = new Dataset();
        }

        public void Run(string filePath)
        {
            if (LoadDataset(filePath))
            {
                SetCommand();
                ProcessCommand();
            }
        }

        private bool LoadDataset(string filePath)
        {
            try
            {
                inputDataset = DataImporter.ImportData(filePath);
            } catch (Exception ex) when (ex is NotSupportedException || ex is IOException)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private void SetCommand()
        {
            DataExporter exporter = new(inputDataset);
            DataProcessor processor = new(inputDataset);
            DataVisualizer visualizer = new(inputDataset);
            DatasetExplorer explorer = new(inputDataset);

            commands.Add("explore", new AnalyzeFunc(new Action(explorer.ExploreDataset), 0));
            commands.Add("export", new AnalyzeFunc(new Action<string>(exporter.ExportData), 1));
            commands.Add("show", new AnalyzeFunc(new Action(visualizer.PrintAllData), 0));
            commands.Add("filter", new AnalyzeFunc(new Action<string, string>(processor.ApplyFilters), 2));
            commands.Add("clean", new AnalyzeFunc(new Action(processor.CleanAndPreprocessData), 0));
            

            commands.Add("help", new AnalyzeFunc(new Action(PrintHelp), 0));
        }

        private void ProcessCommand()
        {
            Console.WriteLine("Dataset was loaded. You can write commands.");

            string? line = string.Empty;
            while (line != "exit")
            {
                line = Console.ReadLine()?.ToLower()?.Trim();

                if (line != null)
                {
                    string command = line.Split(" ")[0];
                    if (commands.ContainsKey(command))
                    {
                        string[] args = line.Split(" ")[1..];
                        if (!commands[command].StartFunc(args))
                        {
                            Console.WriteLine($"Wrong number of parametrs for command {command}: {args.Length} instead of {commands[command].NumberParams}. Please try again.");
                        }
                    }
                    else if (command.Equals("exit"))
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Please try again or use 'help' command for more information.");
                    }
                }
            }
        }

        private void PrintHelp()
        {
            Console.WriteLine("Enter a command (explore, analyze <column name>, exit):");
        }
    }

    class AnalyzeFunc
    {
        private readonly Delegate func;
        public int NumberParams { get; private set; }

        public AnalyzeFunc(Delegate func, int numberParams)
        {
            this.func = func;
            this.NumberParams = numberParams;
        }

        public bool StartFunc(params string[] args)
        {
            if (args.Length == NumberParams)
            {
                func.DynamicInvoke(args);
                return true;
            }
            return false;
        }
    }
}
