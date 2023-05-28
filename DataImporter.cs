using Newtonsoft.Json;

namespace DataAnalysisTool
{
    class DataImporter
    {
        public static Dataset ImportData(string filePath, char separator = ',')
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new ImportDatasetException($"File {filePath} not found.");
            }

            Dataset dataset = new();

            // Perform data import logic based on the specific file format
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".csv":
                    ImportCSVData(filePath, dataset, separator);
                    break;
                case ".json":
                    ImportJSONData(filePath, dataset);
                    break;
                default:
                    throw new ImportDatasetException($"Data import from file format '{fileExtension}' is not supported.");
            }

            return dataset;
        }

        private static void ImportCSVData(string filePath, Dataset dataset, char separator)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                string[]? headers = reader.ReadLine()?.Split(separator);

                if (headers != null)
                {
                    dataset.SetHeader(headers);
                    Parallel.ForEach(File.ReadLines(filePath), (line, _, lineNumber) =>
                    {
                        if (lineNumber > 0)
                        {
                            string[]? values = line.Split(separator);
                            if (values.Any())
                            {
                                DataObject dataObject = new((int)lineNumber);

                                for (int i = 0; i < headers.Length; i++)
                                {
                                    string column = headers[i];
                                    string value = values[i];

                                    dataObject.SetColumnValue(column, value);
                                }

                                lock(dataset)
                                    dataset.AddData(dataObject);
                            }
                        }
                    });
                    dataset.SortDataset();
                }
            }
            catch (IOException ex)
            {
                throw new ImportDatasetException($"Error importing CSV data: {ex.Message}");
            }
        }

        private static void ImportJSONData(string filePath, Dataset dataset)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var dataObjects = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, string>>>(jsonContent);

                if (dataObjects != null && dataObjects.Any())
                {
                    Parallel.ForEach(dataObjects, (value, _, lineId) =>
                    {
                        DataObject newDataObject = new();

                        foreach (var columnValue in value)
                        {
                            newDataObject.SetColumnValue(columnValue.Key, columnValue.Value);

                            lock (dataset)
                            {
                                if (!dataset.GetColumnsNames().Contains(columnValue.Key))
                                {
                                    dataset.AddHeaderName(columnValue.Key);
                                }
                            }                            
                        }

                        dataset.AddData(newDataObject);
                    });
                    dataset.SortDataset();
                }
            }
            catch (IOException ex)
            {
                throw new ImportDatasetException($"Error importing JSON data: {ex.Message}");
            }
            catch (JsonSerializationException)
            {
                throw new ImportDatasetException("Unsupport structure of JSON file. More in documentation of project.");
            }
        }
    }

    class ImportDatasetException : DataAnalysisException
    {
        public ImportDatasetException()
        {
        }

        public ImportDatasetException(string message)
            : base(message)
        {
        }

        public ImportDatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
