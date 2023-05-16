namespace DataAnalysisTool
{
    interface IDataProcessor
    {
        void PerformCalculations(string variable, string[] calculations);
        void ApplyFilters(string column, string value);
        void CleanAndPreprocessData();
    }

    class DataProcessor : IDataProcessor
    {
        private Dataset _dataset;

        public DataProcessor(Dataset dataset)
        {
            _dataset = dataset;
        }
        public void PerformCalculations(string variable, string[] calculations)
        {
            // Retrieve the values for the specified variable from the DataSet
            IEnumerable<string> values = null; // _dataset.GetObjects().Select(dataObject => dataObject.Value);

            // Perform calculations based on the specified operations
            foreach (string calculation in calculations)
            {
                switch (calculation.ToLower())
                {
                    case "mean":
                        double mean = CalculateMean(values);
                        Console.WriteLine($"Mean of {variable}: {mean}");
                        break;
                    case "median":
                        double median = CalculateMedian(values);
                        Console.WriteLine($"Median of {variable}: {median}");
                        break;
                    default:
                        throw new NotSupportedException($"Calculation '{calculation}' is not supported.");
                }
            }
        }

        private double CalculateMean(IEnumerable<string> values)
        {
            return 0;
        }

        private double CalculateMedian(IEnumerable<string> values)
        {
            return 0;
        }

        public void ApplyFilters(string column, string value)
        {
            // Filter the Dataset based on the specified column and value
            _dataset.FilterByColumnValue(column, value);
        }

        public void CleanAndPreprocessData()
        {
            // TODO: Implementation to clean and preprocess the data
        }
    }
}
