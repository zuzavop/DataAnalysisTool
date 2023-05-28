using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;

namespace DataAnalysisTool
{
    /// <summary>
    /// Class responsible for exporting dataset to different file formats.
    /// </summary>
    class DataExporter
    {
        private readonly Dataset _dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataExporter"/> class.
        /// </summary>
        /// <param name="dataset">The dataset to be exported.</param>
        public DataExporter(Dataset dataset)
        {
            _dataset = dataset;
        }

        /// <summary>
        /// Exports the dataset to the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the file to export the dataset to.</param>
        public void ExportData(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".csv":
                    ExportToCSV(filePath);
                    break;
                case ".json":
                    ExportToJSON(filePath);
                    break;
                default:
                    throw new ExporterDatasetException($"Data export to file format '{fileExtension}' is not supported.");
            }
        }

        /// <summary>
        /// Exports the dataset to a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the CSV file to export the dataset to.</param>
        private void ExportToCSV(string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                var columns = _dataset.GetData().First().GetColumns().Keys;

                foreach (var column in columns)
                {
                    csv.WriteField(column);
                }
                csv.NextRecord();

                string? value;
                foreach (var dataObject in _dataset.GetData())
                {
                    foreach (var column in columns)
                    {
                        if ((value = dataObject.GetColumnValue(column)) != null)
                        {
                            csv.WriteField(value);
                        }
                    }

                    csv.NextRecord();
                }
            }
            catch (Exception ex)
            {
                throw new ExporterDatasetException($"Error exporting data to CSV: {ex.Message}");
            }
        }

        /// <summary>
        /// Exports the dataset to a JSON file.
        /// </summary>
        /// <param name="filePath">The path of the JSON file to export the dataset to.</param>
        private void ExportToJSON(string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                writer.WriteLine("[");
                int i = 0;
                foreach (var dataObject in _dataset.GetData())
                {
                    if (i > 0)
                    {
                        writer.WriteLine(",");
                    }
                    else
                    {
                        ++i;
                    }
                    string jsonContent = JsonConvert.SerializeObject(dataObject.columnValuePairs, Formatting.Indented);
                    writer.Write(jsonContent);
                }
                writer.WriteLine();
                writer.WriteLine("]");
            }
            catch (Exception ex)
            {
                throw new ExporterDatasetException($"Error exporting data to JSON: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Exception thrown for exporter dataset errors.
    /// </summary>
    class ExporterDatasetException : DataAnalysisException
    {
        public ExporterDatasetException()
        {
        }

        public ExporterDatasetException(string message)
            : base(message)
        {
        }

        public ExporterDatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
