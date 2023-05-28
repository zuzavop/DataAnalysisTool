using System.Reflection;

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

        public void Run(string filePath, params string[] options)
        {
            char seperator = options.Contains("-s") ? options[Array.IndexOf(options, "-s") + 1][0] :
                    (options.Contains("--seperator") ? options[Array.IndexOf(options, "--seperator") + 1][0] : ',');
            if (LoadDataset(filePath, seperator))
            {
                SetCommand(filePath, seperator);
                ProcessCommand();
            }
        }

        private bool LoadDataset(string filePath, char seperator)
        {
            try
            {
                inputDataset = DataImporter.ImportData(filePath, seperator);
            }
            catch (DataAnalysisException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private void SetCommand(string filePath, char seperator)
        {
            DataExporter exporter = new(inputDataset);
            DataProcessor processor = new(inputDataset);
            DataVisualizer visualizer = new(inputDataset, filePath, seperator);
            DatasetExplorer explorer = new(inputDataset);

            commands.Add("explore", new AnalyzeFunc(explorer.ExploreDataset, 1)
            {
                HelpText = "Show overall statistics about the whole dataset or a specific column.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("export", new AnalyzeFunc(exporter.ExportData)
            {
                HelpText = "Export the dataset to a file.",
                HelpParams = new string[] { "file path" }
            });
            commands.Add("show", new AnalyzeFunc(visualizer.PrintAllData)
            {
                HelpText = "Display all data in the dataset."
            });
            commands.Add("filter", new AnalyzeFunc(processor.ApplyFilters)
            {
                HelpText = "Filter column by value. Keeps only rows that match the specified conditions.\n" +
                "If the column contains non-numeric values or if the given `value` is non-numeric, " +
                "less than and greater than comparisons will be based on the lengths of strings. " +
                "The 'in' condition checks if the column value contains the specified value.",
                HelpParams = new string[] { "column name", "=|!=|<|>|=>|=<|in", "value" }
            });
            commands.Add("clean", new AnalyzeFunc(processor.CleanAndPreprocessData)
            {
                HelpText = "Clean the dataset by removing rows with missing values and normalizing columns with numerical values."
            });
            commands.Add("remove_duplicates", new AnalyzeFunc(processor.RemoveDuplicates)
            {
                HelpText = "Remove duplicates from the dataset."
            });
            commands.Add("append", new AnalyzeFunc(processor.AppendNewData)
            {
                HelpText = "Append data from another file to the end of the dataset. " +
                "Only include columns that already exist in the dataset.",
                HelpParams = new string[] { "file path" }
            });
            commands.Add("statistic", new AnalyzeFunc(processor.PerformCalculations)
            {
                HelpText = "Show selected statistics of the column.",
                HelpParams = new string[] { "column name", "mean|median|deviation|entropy|mode|all" }
            });
            commands.Add("correlation", new AnalyzeFunc(processor.CalculateColumnCorrelation)
            {
                HelpText = "Show the Pearson correlation between two columns.",
                HelpParams = new string[] { "column name", "column name" }
            });
            commands.Add("outliers", new AnalyzeFunc(processor.FindOutliers)
            {
                HelpText = "Find outliers in a column if the column contains numerical values.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("regression", new AnalyzeFunc(processor.PerformRegressionAnalysis)
            {
                HelpText = "Perform regression analysis on two columns. " +
                    "The data from the first column will be used as the x-coordinate and the data from the second column as the y-coordinate.",
                HelpParams = new string[] { "column name", "column name" }
            });
            commands.Add("bar_plot", new AnalyzeFunc(visualizer.CreateAndSaveBarPlot)
            {
                HelpText = "Export bar plot created from selected colums in the input file data.",
                HelpParams = new string[] { "output file path", "column name", "column name" }
            });
            commands.Add("line_plot", new AnalyzeFunc(visualizer.CreateAndSaveLinePlot)
            {
                HelpText = "Export line plot created from selected colums in the input file data.",
                HelpParams = new string[] { "output file path", "column name", "column name" }
            });
            commands.Add("scatter_plot", new AnalyzeFunc(visualizer.CreateAndSaveScatterPlot)
            {
                HelpText = "Export scatter plot created from selected colums in the input file data.",
                HelpParams = new string[] { "output file path", "column name", "column name" }
            });
            commands.Add("histogram", new AnalyzeFunc(visualizer.CreateAndSaveHistogram)
            {
                HelpText = "Export histogram created from selected colums in the input file data.",
                HelpParams = new string[] { "output file path", "column name", "column name" }
            });
            commands.Add("pie_plot", new AnalyzeFunc(visualizer.CreateAndSavePiePlot)
            {
                HelpText = "Export pie plot created from selected column in the input file data.",
                HelpParams = new string[] { "output file path", "column name"}
            });
            commands.Add("box_plot", new AnalyzeFunc(visualizer.CreateAndSaveBarPlot)
            {
                HelpText = "Export pie plot created from selected columns in the input file data.",
                HelpParams = new string[] { "output file path", "column name", "column name" }
            });
            commands.Add("sort", new AnalyzeFunc(processor.SortColumn)
            {
                HelpText = "Sort dataset by the column.",
                HelpParams = new string[] { "column name" }
            });

            commands.Add("help", new AnalyzeFunc(PrintHelp)
            {
                HelpText = "Print help information for available commands."
            });
        }

        private void ProcessCommand()
        {
            Console.WriteLine("Dataset was loaded. You can write commands.");

            string? line = string.Empty;
            while (line != "exit")
            {
                Console.Write(">> ");
                line = Console.ReadLine()?.ToLower()?.Trim();

                if (line != null)
                {
                    string command = line.Split(" ")[0];
                    if (commands.ContainsKey(command))
                    {
                        string[] args = line.Split(" ")[1..];
                        if (!commands[command].StartFunc(args))
                        {
                            Console.WriteLine($"Wrong number of parameters for command `{command}`: {args.Length} instead of {commands[command].NumberParams}. For more information use the `help` command.");
                        }
                    }
                    else if (command.Equals("exit"))
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Please try again or use the 'help' command for more information.");
                    }
                }
            }
        }

        private void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            foreach (var command in commands)
            {
                var methodName = command.Key;
                if (methodName.Equals("help")) continue;

                var analyzeFunc = command.Value;

                Console.Write($"  {methodName} ");
                for (int i = 0; i < analyzeFunc.NumberParams; i++)
                {
                    Console.Write($"[{analyzeFunc.HelpParams?[i]}] ");
                }
                Console.WriteLine($" - {analyzeFunc.HelpText}");
                Console.WriteLine();
            }
            Console.WriteLine("  exit - End interactive Data Analysis Tool.");
        }

        private class AnalyzeFunc
        {
            private readonly Delegate func;
            public readonly int canBeOptionalNum;
            public int NumberParams { get; private set; }
            public string HelpText { get; set; }
            public string[] HelpParams { get; set; }

            public AnalyzeFunc(Delegate func, int optinal = 0)
            {
                this.func = func;
                this.canBeOptionalNum = optinal;
                this.NumberParams = func.Method.GetParameters().Length;
                this.HelpText = "";
                this.HelpParams = Array.Empty<string>();
            }

            public bool StartFunc(params string[] args)
            {
                if (args.Length == NumberParams || args.Length == (NumberParams - canBeOptionalNum))
                {
                    try
                    {
                        if (args.Length != NumberParams)
                        {
                            var parameters = func.Method.GetParameters();
                            string?[] new_args = new string[NumberParams];
                            for (int i = 0; i < new_args.Length; i++)
                            {
                                if (i < args.Length)
                                {
                                    new_args[i] = args[i];
                                }
                                else if (parameters[i].HasDefaultValue)
                                {
                                    new_args[i] = parameters[i].DefaultValue?.ToString();
                                }
                            }
                            func.DynamicInvoke(new_args);
                        }
                        else
                        {
                            func.DynamicInvoke(args);
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException is DataAnalysisException)
                        {
                            Console.WriteLine(ex.InnerException.Message);
                        }
                        else
                        {
                            if (ex.InnerException == null)
                                throw ex;
                            throw ex.InnerException;
                        }
                    }
                    return true;
                }
                return false;
            }
        }
    }

    public class DataAnalysisException : Exception
    {
        public DataAnalysisException()
        {
        }

        public DataAnalysisException(string message)
            : base(message)
        {
        }

        public DataAnalysisException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
