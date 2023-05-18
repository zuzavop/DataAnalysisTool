using System.Data;

namespace DataAnalysisTool
{
    class DataProcessor
    {
        private readonly Dataset _dataset;

        public DataProcessor(Dataset dataset)
        {
            _dataset = dataset;
        }
        public void PerformCalculations(string columnName, string[] calculations)
        {
            // Retrieve the values for the specified variable from the DataSet
            IEnumerable<string> values = _dataset.GetData().Select(dataObject => dataObject.GetColumnValue(columnName)).Where(value => value != null)!;

            if (values != null)
            {
                // Perform calculations based on the specified operations
                foreach (string calculation in calculations)
                {
                    switch (calculation.ToLower())
                    {
                        case "mean":
                            double mean = CalculateMean(values);
                            Console.WriteLine($"Mean of {columnName}: {mean}");
                            break;
                        case "median":
                            double median = CalculateMedian(values);
                            Console.WriteLine($"Median of {columnName}: {median}");
                            break;
                        default:
                            throw new NotSupportedException($"Calculation '{calculation}' is not supported.");
                    }
                }
            }
        }

        private static double CalculateMean(IEnumerable<string> values)
        {
            // Convert the string values to double and calculate the mean
            IEnumerable<double> numericValues = values.Select(double.Parse);
            double mean = numericValues.Average();
            return mean;
        }

        private static double CalculateMedian(IEnumerable<string> values)
        {
            // Convert the string values to double and calculate the median
            IEnumerable<double> numericValues = values.Select(double.Parse).OrderBy(x => x);
            double mid = (numericValues.Count() - 1) / 2.0;
            double median = (numericValues.ElementAt((int)(mid)) + numericValues.ElementAt((int)(mid + 0.5))) / 2;
            return median;
        }

        public void ApplyFilters(string column, string value)
        {
            // Filter the Dataset based on the specified column and value
            _dataset.FilterByColumnValue(column, value);
        }

        public void CleanAndPreprocessData()
        {
            // Clean data by removing any rows with missing values
            _dataset.RemoveRowsWithMissingValues();

            // Preprocess data by normalizing numeric columns
            List<string> numericColumns = _dataset.GetNumericColumns();

            foreach (string column in numericColumns)
            {
                _dataset.NormalizeColumn(column);
            }
        }
    }
}
