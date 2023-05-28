using System.Diagnostics;

namespace DataAnalysisTool
{
    /// <summary>
    /// Represents a data visualizer for generating plots and printing data.
    /// </summary>
    class DataVisualizer
    {
        private readonly Dataset _dataset;
        private readonly string inputPath;
        private readonly char seperator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataVisualizer"/> class.
        /// </summary>
        /// <param name="dataset">The dataset to visualize.</param>
        /// <param name="filePath">The path to the input file.</param>
        /// <param name="seperator">The column separator character.</param>
        public DataVisualizer(Dataset dataset, string filePath, char seperator)
        {
            this._dataset = dataset;
            this.inputPath = filePath;
            this.seperator = seperator;
        }

        /// <summary>
        /// Prints all data from the dataset.
        /// </summary>
        public void PrintAllData()
        {
            string[] columns = _dataset.GetColumnsNames().ToArray();

            PrintColumns(columns);
        }

        /// <summary>
        /// Prints the specified columns to the console.
        /// </summary>
        /// <param name="columns">The columns to print.</param>
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

        /// <summary>
        /// Prints the specified columns to the console.
        /// </summary>
        /// <param name="columns">The columns to print.</param>
        private static void PrintRow(List<string> values)
        {
            foreach(var value in values)
            {
                Console.Write("{0,10}", value);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Creates and saves a line plot based on the specified columns.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the first column.</param>
        /// <param name="columnName2">The name of the second column.</param>
        public void CreateAndSaveLinePlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "line_plot", columnName1, columnName2);
        }

        /// <summary>
        /// Creates and saves a bar plot based on the specified columns.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the first column.</param>
        /// <param name="columnName2">The name of the second column.</param>
        public void CreateAndSaveBarPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "bar_plot", columnName1, columnName2);
        }

        /// <summary>
        /// Creates and saves a scatter plot based on the specified columns.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the first column.</param>
        /// <param name="columnName2">The name of the second column.</param>
        public void CreateAndSaveScatterPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "scatter_plot", columnName1, columnName2);
        }

        /// <summary>
        /// Creates and saves a pie plot based on the specified column.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the column.</param>
        public void CreateAndSavePiePlot(string outputFilePath, string columnName1)
        {
            CreateAndSavePlot(outputFilePath, "pie_plot", columnName1);
        }

        /// <summary>
        /// Creates and saves a histogram based on the specified columns.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the first column.</param>
        /// <param name="columnName2">The name of the second column.</param>
        public void CreateAndSaveHistogram(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "histogram", columnName1, columnName2);
        }

        /// <summary>
        /// Creates and saves a box plot based on the specified columns.
        /// </summary>
        /// <param name="outputFilePath">The path to save the plot image file.</param>
        /// <param name="columnName1">The name of the first column.</param>
        /// <param name="columnName2">The name of the second column.</param>
        public void CreateAndSaveBoxPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, "box_plot", columnName1, columnName2);
        }

        /// <summary>
        /// Creates and saves a data plot based on the specified parameters.
        /// </summary>
        /// <param name="outputFilePath">The output file path to save the plot.</param>
        /// <param name="plotName">The name of the plot.</param>
        /// <param name="columnName1">The name of the first column for plotting.</param>
        /// <param name="columnName2">The name of the second column for plotting (optional).</param>
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

    /// <summary>
    /// Represents an exception related to visualization of datasets.
    /// </summary>
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
