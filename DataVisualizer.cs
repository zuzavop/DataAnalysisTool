using System.Diagnostics;

namespace DataAnalysisTool
{
    class DataVisualizer
    {
        private readonly Dataset _dataset;
        private readonly string inputPath;
        private readonly char seperator;
        public DataVisualizer(Dataset dataset, string filePath, char seperator)
        {
            this._dataset = dataset;
            this.inputPath = filePath;
            this.seperator = seperator;
        }

        public void PrintAllData()
        {
            string[] columns = _dataset.GetColumnsNames().ToArray();

            PrintColumns(columns);
        }

        private void PrintColumns(string[] columns)
        {
            // Print column headers
            PrintRow(columns.ToList());

            string? value;
            // Print data rows
            foreach (var dataObject in _dataset.GetData())
            {
                List<string> values = new();

                foreach (var column in columns)
                {
                    if ((value = dataObject.GetColumnValue(column)) != null)
                    {
                        values.Add(value);
                    }
                    else
                    {
                        values.Add("");
                    }
                }

                PrintRow(values);
            }
        }

        private static void PrintRow(List<string> values)
        {
            foreach(var value in values)
            {
                Console.Write("{0,10}", value);
            }
            Console.WriteLine();
        }

        public void CreateAndSaveLinePlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "line_plot", columnName1, columnName2);
        }

        public void CreateAndSaveBarPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "bar_plot", columnName1, columnName2);
        }

        public void CreateAndSaveScatterPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "scatter_plot", columnName1, columnName2);
        }

        public void CreateAndSavePiePlot(string outputFilePath, string columnName1)
        {
            CreateAndSavePlot(outputFilePath, "pie_plot", columnName1);
        }

        public void CreateAndSaveHistogram(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "histogram", columnName1, columnName2);
        }

        public void CreateAndSaveBoxPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "box_plot", columnName1, columnName2);
        }

        private void CreateAndSavePlot(string outputFilePath, string plotName, string columnName1, string columnName2 ="")
        {
            string fileExtension = Path.GetExtension(inputPath);
            if (!fileExtension.Equals(".csv"))
            {
                throw new VisualizationDatasetException($"Input file should be with extension .csv. Extension {fileExtension} is not yet supported. You can use export command.");
            }

            if (!_dataset.GetColumnsNames().Contains(columnName1) || !_dataset.GetColumnsNames().Contains(columnName2))
            {
                throw new VisualizationDatasetException("Dataset doesn't contains column of name you pick.");
            }

            fileExtension = Path.GetExtension(outputFilePath);
            if (!fileExtension.Equals(".png"))
            {
                throw new VisualizationDatasetException($"Output file should be with extension .png. Extension {fileExtension} is not supported.");
            }

            string workingDirectory = Environment.CurrentDirectory;
            string? projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
            if (projectDirectory == null) projectDirectory = "";
            else projectDirectory += "\\";

            // Invoke the Python script
            ProcessStartInfo psi = new()
            {
                FileName = "python", // Assumes Python is in the system PATH
                Arguments = $"\"{projectDirectory}plot.py\" {plotName} {inputPath} {seperator} {columnName1} {columnName2} {outputFilePath}",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            Process? process = Process.Start(psi);
            process?.WaitForExit();
        }
    }

    class VisualizationDatasetException : DataAnalysisException
    {
        public VisualizationDatasetException()
        {
        }

        public VisualizationDatasetException(string message)
            : base(message)
        {
        }

        public VisualizationDatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
