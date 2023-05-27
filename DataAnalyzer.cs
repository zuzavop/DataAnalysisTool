using System;
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
            if (LoadDataset(filePath, options))
            {
                SetCommand();
                ProcessCommand();
            }
        }

        private bool LoadDataset(string filePath, params string[] options)
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

            commands.Add("explore", new AnalyzeFunc(explorer.ExploreDataset, 1, true)
            {
                HelpText = "Show overall statistic about whole dataset of about one column.",
                HelpParams = new string[] {"column name"}
            });
            commands.Add("export", new AnalyzeFunc(exporter.ExportData, 1)
            {
                HelpText = "Export the dataset to a file.",
                HelpParams = new string[] { "file path" }
            });
            commands.Add("show", new AnalyzeFunc(visualizer.PrintAllData, 0)
            {
                HelpText = "Display all data in the dataset.",
                HelpParams = Array.Empty<string>()
            });
            commands.Add("filter", new AnalyzeFunc(processor.ApplyFilters, 2)
            {
                HelpText = "Filter column by value.",
                HelpParams = new string[] { "column name", "value"}
            });
            commands.Add("clean", new AnalyzeFunc(processor.CleanAndPreprocessData, 0)
            {
                HelpText = "Clean dataset.",
                HelpParams = Array.Empty<string>()
            });
            commands.Add("append", new AnalyzeFunc(processor.AppendNewData, 1)
            {
                HelpText = "Append data from another file to end of dataset. Pick only column that are already in dataset.",
                HelpParams = new string[] {"file path"}
            });
            commands.Add("statistic", new AnalyzeFunc(processor.PerformCalculations, 2)
            {
                HelpText = "Show statistic of column.",
                HelpParams = new string[] { "column name", "mean|median|deviation|entropy|all" }
            });
            commands.Add("bar_plot", new AnalyzeFunc(visualizer.CreateAndSaveBarPlot, 1)
            {
                HelpText = "Export bar plot created from data of columns.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("line_plot", new AnalyzeFunc(visualizer.CreateAndSaveLinePlot, 1)
            {
                HelpText = "Export line plot created from data of columns.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("scatter_plot", new AnalyzeFunc(visualizer.CreateAndSaveScatterPlot, 1)
            {
                HelpText = "Export scatter plot created from data of columns.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("histogram", new AnalyzeFunc(visualizer.CreateAndSaveHistogram, 1)
            {
                HelpText = "Export histogram created from data of columns.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("pie_plot", new AnalyzeFunc(visualizer.CreateAndSavePiePlot, 1)
            {
                HelpText = "Export pie plot created from data of columns.",
                HelpParams = new string[] { "column name" }
            });
            commands.Add("sort", new AnalyzeFunc(processor.SortColumn, 1)
            {
                HelpText = "Sort dataset by column.",
                HelpParams = new string[] { "column name" }
            });

            commands.Add("help", new AnalyzeFunc(PrintHelp, 0)
            {
                HelpText = "Print help information for available commands.",
                HelpParams = Array.Empty<string>()
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
                            Console.WriteLine($"Wrong number of parametrs for command `{command}`: {args.Length} instead of {commands[command].NumberParams}. For more information use command `help`.");
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
        }

        private class AnalyzeFunc
        {
            private readonly Delegate func;
            public readonly bool canBeOptional;
            public int NumberParams { get; private set; }
            public string? HelpText { get; set; }
            public string[]? HelpParams { get; set; }

            public AnalyzeFunc(Delegate func, int numberParams, bool optinal=false)
            {
                this.func = func;
                this.canBeOptional = optinal;
                this.NumberParams = numberParams;
            }

            public bool StartFunc(params string[] args)
            {
                if (args.Length == NumberParams || (canBeOptional && args.Length == (NumberParams - 1)))
                {
                    try
                    {
                        if (canBeOptional && args.Length == 0)
                        {
                            func.DynamicInvoke("");
                        } 
                        else if (canBeOptional && args.Length == (NumberParams - 1))
                        {
                            func.DynamicInvoke(args, "");
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
                        } else
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
