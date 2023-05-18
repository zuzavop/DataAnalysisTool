namespace DataAnalysisTool
{
    class DataAnalyzer
    {
        readonly DataExporter dataExporter;
        readonly DataProcessor dataProcessor;
        readonly DataVisualizer dataVisualizer;
        readonly DatasetExplorer datasetExplorer;
        readonly Dataset inputDataset;

        public DataAnalyzer(string[] args)
        {
            string inputFilePath = args[0];
            inputDataset = DataImporter.ImportData(inputFilePath);

            this.dataExporter = new DataExporter(inputDataset);
            this.dataProcessor = new DataProcessor(inputDataset);
            this.dataVisualizer = new DataVisualizer(inputDataset);
            this.datasetExplorer = new DatasetExplorer(inputDataset);
        }

        internal void Run()
        {
            string? command = string.Empty;
            while (command != "exit")
            {
                command = Console.ReadLine()?.ToLower()?.Trim();

                if (command != null)
                {
                    if (command.StartsWith("analyze "))
                    {
                        string column = command[8..].Trim();
                        datasetExplorer.AnalyzeColumn(column);
                    }
                    else if (command.Trim().Equals("explore"))
                    {
                        datasetExplorer.ExploreDataset();
                    }
                    else if (command.StartsWith("export "))
                    {
                        string filePath = command[7..].Trim();
                        dataExporter.ExportData(filePath);
                    }
                    else if (command.Trim().Equals("show"))
                    {
                        dataVisualizer.PrintAllData();
                    }
                    else if (command.StartsWith("filter "))
                    {
                        string[] values = command[7..].Trim().Split(" ");
                        dataProcessor.ApplyFilters(values[0], values[1]);
                    }
                    else if (command.Trim().Equals("help"))
                    {
                        PrintHelp();
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Please try again.");
                    }
                }
            }
        }

        private void PrintHelp()
        {
            Console.WriteLine("Enter a command (explore, analyze <column>, exit):");
        }
    }
}
