namespace DataAnalysisTool
{
    interface IDataExporter
    {
        void ExportData(string filePath, string format);
    }
    class DataExporter : IDataExporter
    {
        private Dataset _dataset;

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
                    throw new NotSupportedException("Export format not supported.");
            }
        }

        private void ExportToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {

            }
        }

        private void ExportToJSON(string filePath)
        {

        }
    }
}
