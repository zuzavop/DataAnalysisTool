﻿using Newtonsoft.Json;

namespace DataAnalysisTool
{
    interface IDataImporter
    {
        Dataset ImportData(string filePath);
    }
    class DataImporter : IDataImporter
    {
        public Dataset ImportData(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            Dataset dataset = new();

            // Perform data import logic based on the specific file format
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".csv":
                    ImportCSVData(filePath, dataset);
                    break;
                case ".json":
                    ImportJSONData(filePath, dataset);
                    break;
                default:
                    throw new NotSupportedException($"Data import from file format '{fileExtension}' is not supported.");
            }

            return dataset;
        }

        private static void ImportCSVData(string filePath, Dataset dataset)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                string[]? headers = reader.ReadLine()?.Split(',');

                while (!reader.EndOfStream)
                {
                    string[]? values = reader.ReadLine()?.Split(',');

                    if (values?.Length != headers?.Length)
                    {
                        Console.WriteLine("Invalid data format. Skipping row.");
                        continue;
                    }

                    DataObject dataObject = new DataObject();

                    for (int i = 0; i < headers?.Length; i++)
                    {
                        string column = headers[i];
                        string? value = values?[i];

                        dataObject.AddColumnValue(column, value);
                    }

                    dataset.Add(dataObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing CSV data: {ex.Message}");
            }
        }

        private static void ImportJSONData(string filePath, Dataset dataset)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                DataObject[]? dataObjects = JsonConvert.DeserializeObject<DataObject[]>(jsonContent);

                foreach (DataObject dataObject in dataObjects)
                {
                    dataset.Add(dataObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing JSON data: {ex.Message}");
            }
        }
    }
}