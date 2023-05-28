using Newtonsoft.Json;

namespace DataAnalysisTool
{
    /// <summary>
    /// Class responsible for importing data from various file formats into a dataset.
    /// </summary>
    class DataImporter
    {
        /// <summary>
        /// Imports data from the specified file path into a dataset.
        /// </summary>
        /// <param name="filePath">The path of the file to import data from.</param>
        /// <param name="separator">The character used as a separator in the file (default is comma).</param>
        /// <returns>The dataset containing the imported data.</returns>
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

        /// <summary>
        /// Imports data from a CSV file into a dataset.
        /// </summary>
        /// <param name="filePath">The path of the CSV file to import data from.</param>
        /// <param name="dataset">The dataset to import the data into.</param>
        /// <param name="separator">The character used as a separator in the CSV file.</param>
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

        /// <summary>
        /// Imports data from a JSON file into a dataset.
        /// </summary>
        /// <param name="filePath">The path of the JSON file to import data from.</param>
        /// <param name="dataset">The dataset to import the data into.</param>
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

    /// <summary>
    /// Exception class for import dataset errors.
    /// </summary>
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
