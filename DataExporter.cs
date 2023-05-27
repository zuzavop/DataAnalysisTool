using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;

namespace DataAnalysisTool
{
    class DataExporter
    {
        private readonly Dataset _dataset;

        public DataExporter(Dataset dataset)
        {
            _dataset = dataset;
        }

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
