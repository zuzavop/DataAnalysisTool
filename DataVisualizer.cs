using System.Data;
using System.Diagnostics;

namespace DataAnalysisTool
{
    class DataVisualizer
    {
        private readonly Dataset _dataset;
        private readonly string inputPath;
        public DataVisualizer(Dataset dataset, string filePath)
        {
            this._dataset = dataset;
            this.inputPath = filePath;
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
            Console.WriteLine(string.Join("\t", values));
        }

        public void CreateAndSaveLinePlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, columnName1, columnName2, "line_plot");
        }

        public void CreateAndSaveBarPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, columnName1, columnName2, "bar_plot");
        }

        public void CreateAndSaveScatterPlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, columnName1, columnName2, "scatter_plot");
        }

        public void CreateAndSavePiePlot(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, columnName1, columnName2, "pie_plot");
        }

        public void CreateAndSaveHistogram(string outputFilePath, string columnName1, string columnName2)
        {
            CreateAndSavePlot(outputFilePath, columnName1, columnName2, "histogram");
        }

        private void CreateAndSavePlot(string outputFilePath, string columnName1, string columnName2, string plotName)
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

            // Invoke the Python script
            ProcessStartInfo psi = new()
            {
                FileName = "python", // Assumes Python is in the system PATH
                Arguments = $"plot.py {plotName} {inputPath} {columnName1} {columnName2} {outputFilePath}",
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
