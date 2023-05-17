namespace DataAnalysisTool
{
    class DataAnalyzer
    {
        readonly IDataImporter dataImporter;
        readonly IDataExporter dataExporter;
        readonly IDataProcessor dataProcessor;
        readonly IDataVisualizer dataVisualizer;
        readonly IDatasetExplorer datasetExplorer;
        readonly Dataset inputDataset;

        public DataAnalyzer(string[] args)
        {
            this.dataImporter = new DataImporter();

            string inputFilePath = args[0];
            inputDataset = dataImporter.ImportData(inputFilePath);

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
                    else if (command == "explore")
                    {
                        datasetExplorer.ExploreDataset();
                    }
                    else if (command.StartsWith("export "))
                    {
                        string filePath = command[7..].Trim();
                        dataExporter.ExportData(filePath, "csv");
                    }
                    else if (command.StartsWith("show "))
                    {
                        string[] columns = command[5..].Trim().Split(" ");
                        dataVisualizer.VisualizeData(columns, "string");
                    }
                    else if (command.StartsWith("filter "))
                    {
                        string[] values = command[7..].Trim().Split(" ");
                        dataProcessor.ApplyFilters(values[0], values[1]);
                    }
                    else if (command == "help")
                    {
                        Console.WriteLine("Enter a command (explore, analyze <column>, exit):");
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Please try again.");
                    }
                }
            }
        }
    }
}
