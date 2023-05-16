namespace DataAnalysisTool
{
    class DataAnalyzer
    {
        IDataImporter dataImporter;
        IDataExporter dataExporter;
        IDataProcessor dataProcessor;
        IDataVisualizer dataVisualizer;
        IDatasetExplorer datasetExplorer;

        public DataAnalyzer()
        {
            this.dataImporter = new DataImporter();
            this.dataExporter = new DataExporter();
            this.dataProcessor = new DataProcessor();
            this.dataVisualizer = new DataVisualizer();
            this.datasetExplorer = new DatasetExplorer();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DataAnalyzer analysis = new();
        }
    }
}