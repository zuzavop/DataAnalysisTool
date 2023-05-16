using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisTool
{
    interface IDataProcessor
    {
        void PerformCalculations(Dataset dataSet, string variable, string[] calculations);
        void ApplyFilters(Dataset dataSet, string column, string value);
        void CleanAndPreprocessData(Dataset dataSet);
    }

    class DataProcessor : IDataProcessor
    {
        public void PerformCalculations(Dataset dataset, string variable, string[] calculations)
        {
            // Retrieve the values for the specified variable from the DataSet
            IEnumerable<string> values = dataset.GetObjects().Select(dataObject => dataObject.Value);

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
                    // Add more cases for additional calculations if needed
                    default:
                        throw new NotSupportedException($"Calculation '{calculation}' is not supported.");
                }
            }
        }

        private double CalculateMean(IEnumerable<string> values)
        {
            // Convert the string values to double and calculate the mean
            IEnumerable<double> numericValues = values.Select(double.Parse);
            double mean = numericValues.Average();
            return mean;
        }

        private double CalculateMedian(IEnumerable<string> values)
        {
            // Convert the string values to double and calculate the median
            IEnumerable<double> numericValues = values.Select(double.Parse);
            // double median = numericValues.Median();
            return median;
        }

        public void ApplyFilters(Dataset dataset, string column, string value)
        {
            // Filter the DataSet based on the specified column and value
            dataset.FilterByColumnValue(column, value);
        }

        public void CleanAndPreprocessData(Dataset dataSet)
        {
            // Implementation to clean and preprocess the data
        }
    }
}
