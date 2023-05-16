using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisTool
{
    interface IDataExporter
    {
        void ExportData(Dataset dataSet, string filePath, string format);
    }
    class DataExporter : IDataExporter
    {
        public void ExportData(Dataset dataset, string filePath, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    ExportToCsv(dataset, filePath);
                    break;
                case "json":
                    ExportToJson(dataset, filePath);
                    break;
                case "xlsx":
                    ExportToExcel(dataset, filePath);
                    break;
                // Add more cases for additional export formats if needed
                default:
                    throw new NotSupportedException("Export format not supported.");
            }
        }

        private void ExportToCsv(Dataset dataset, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (DataObject dataObject in dataset.GetObjects())
                {
                    string value = dataObject.Value;
                    writer.WriteLine(value);
                }
            }
        }

        private void ExportToJson(Dataset dataset, string filePath)
        {
            // Implement logic to export data to a JSON file
            // Serialize the DataSet or DataObject instances to JSON format and write to the file
        }

        private void ExportToExcel(Dataset dataset, string filePath)
        {
            // Implement logic to export data to an Excel file
            // Use libraries such as EPPlus or NPOI to create an Excel workbook, populate data from the DataSet, and save to the file
        }
    }
}
