using CsvHelper;
using Newtonsoft.Json;
using System.Globalization;

namespace DataAnalysisTool
{
    interface IDataExporter
    {
        void ExportData(string filePath, string format);
    }
    class DataExporter : IDataExporter
    {
        private readonly Dataset _dataset;

        public DataExporter(Dataset dataset)
        {
            _dataset = dataset;
        }

        public void ExportData(string filePath, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    ExportToCSV(filePath);
                    break;
                case "json":
                    ExportToJSON(filePath);
                    break;
                default:
                    throw new NotSupportedException($"Data export to file format '{format}' is not supported.");
            }
        }

        private void ExportToCSV(string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                var columns = _dataset.GetObjects().First().GetColumns().Keys;

                foreach (var column in columns)
                {
                    csv.WriteField(column);
                }
                csv.NextRecord();

                foreach (var dataObject in _dataset.GetObjects())
                {
                    foreach (var column in columns)
                    {
                        csv.WriteField(dataObject.GetColumnValue(column));
                    }

                    csv.NextRecord();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting data to CSV: {ex.Message}");
            }
        }

        private void ExportToJSON(string filePath)
        {
            try
            {
                string jsonContent = JsonConvert.SerializeObject(_dataset.GetObjects(), Formatting.Indented);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting data to JSON: {ex.Message}");
            }
        }
    }
}
