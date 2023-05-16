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
                    throw new NotSupportedException("File format not supported.");
            }

            return dataset;
        }

        private void ImportCSVData(string filePath, Dataset dataset)
        {

        }

        private void ImportJSONData(string filePath, Dataset dataset)
        {

        }
    }
}
