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
        public void PerformCalculations(string columnName, string calculation)
        {
            // Perform calculations based on the specified operations
            switch (calculation.ToLower())
            {
                case "mean":
                    double mean = CalculateMean(columnName);
                    Console.WriteLine($"Mean of column {columnName}: {mean}");
                    break;
                case "median":
                    double median = CalculateMedian(columnName);
                    Console.WriteLine($"Median of column {columnName}: {median}");
                    break;
                case "deviation":
                    double deviation = CalculateStandardDeviation(columnName);
                    Console.WriteLine($"Standard deviation of column {columnName}: {deviation}");
                    break;
                case "entropy":
                    double entropy = CalculateColumnEntropy(columnName);
                    Console.WriteLine($"Entropy of column {columnName}: {entropy}");
                    break;
                case "all":
                    var values = CalculateColumnStatistics(columnName);
                    foreach (var value in values)
                    {
                        Console.WriteLine($"{value.Key} of column {columnName}: {value.Value}");
                    }
                    break;
                default:
                    throw new DataAnalysisException($"Calculation '{calculation}' is not supported.");
            }
        }

        private double CalculateMean(string columnName)
        {
            List<double> values = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    values.Add(value);
                }
            }
            // Convert the string values to double and calculate the mean
            double mean = values.Average();
            return mean;
        }

        private double CalculateMedian(string columnName)
        {
            List<double> values = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    values.Add(value);
                }
            }

            if (values.Count < 2)
            {
                return 0;
            }
            // Convert the string values to double and calculate the median
            double mid = (values.Count - 1) / 2.0;
            double median = (values.ElementAt((int)(mid)) + values.ElementAt((int)(mid + 0.5))) / 2;
            return median;
        }

        private double CalculateStandardDeviation(string columnName)
        {
            List<double> values = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    values.Add(value);
                }
            }

            if (values.Count < 2)
            {
                return 0;
            }

            double mean = values.Average();
            double sumOfSquaredDifferences = values.Sum(value => Math.Pow(value - mean, 2));
            double variance = sumOfSquaredDifferences / (values.Count - 1);

            return Math.Sqrt(variance);
        }

        private double CalculateColumnEntropy(string column)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            Dictionary<string, int> valueCounts = new();
            int totalCount = 0;

            string? value;
            foreach (DataObject dataObject in dataObjects)
            {
                if ((value = dataObject.GetColumnValue(column)) != null)
                {
                    if (valueCounts.ContainsKey(value))
                    {
                        valueCounts[value]++;
                    }
                    else
                    {
                        valueCounts[value] = 1;
                    }

                    totalCount++;
                }
            }

            double entropy = 0;

            foreach (int count in valueCounts.Values)
            {
                double probability = (double)count / totalCount;
                entropy -= probability * Math.Log(probability, 2);
            }

            return entropy;
        }

        public void CalculateColumnCorrelation(string column1Name, string column2Name)
        {
            List<double> values1 = new();
            List<double> values2 = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(column1Name, out double value1) && dataObject.TryGetNumericValue(column2Name, out double value2))
                {
                    values1.Add(value1);
                    values2.Add(value2);
                }
            }

            double correlation = CalculatePearsonCorrelation(values1, values2);
            Console.WriteLine($"Pearson Correlation of column {column1Name} and column {column2Name}: {correlation}");
        }

        private static double CalculatePearsonCorrelation(List<double> values1, List<double> values2)
        {
            if (values1.Count != values2.Count)
            {
                throw new DataAnalysisException("The number of values in both lists must be the same.");
            }

            int n = values1.Count;
            double sumX = values1.Sum();
            double sumY = values2.Sum();
            double sumXY = values1.Zip(values2, (x, y) => x * y).Sum();
            double sumXSquare = values1.Select(x => x * x).Sum();
            double sumYSquare = values2.Select(y => y * y).Sum();

            double numerator = (n * sumXY) - (sumX * sumY);
            double denominator = Math.Sqrt(((n * sumXSquare) - (sumX * sumX)) * ((n * sumYSquare) - (sumY * sumY)));

            if (denominator == 0)
            {
                return 0;
            }

            return numerator / denominator;
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

        public void FindOutliers(string column)
        {
            Dictionary<int, double> outliers = new();

            double mean = CalculateMean(column);
            double standardDeviation = CalculateStandardDeviation(column);

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(column, out double value))
                {
                    double zScore = (value - mean) / standardDeviation;

                    if (Math.Abs(zScore) > 1)
                    {
                        outliers.Add(dataObject.Id, value);
                    }
                }
            }
            foreach (var o in outliers)
            {
                Console.WriteLine($"line {o.Key}: {o.Value}");
            }
        }

        private Dictionary<string, double> CalculateColumnStatistics(string columnName)
        {
            Dictionary<string, double> statistics = new();

            double mean = CalculateMean(columnName);
            double median = CalculateMedian(columnName);
            double stdDev = CalculateStandardDeviation(columnName);
            double entropy = CalculateColumnEntropy(columnName);
            double min = double.MaxValue;
            double max = double.MinValue;

            List<DataObject> dataObjects = _dataset.GetData();

            foreach (DataObject dataObject in dataObjects)
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    min = Math.Min(min, value);
                    max = Math.Max(max, value);
                }
            }

            statistics["Mean"] = mean;
            statistics["Meadian"] = median;
            statistics["Standard Deviation"] = stdDev;
            statistics["Entropy"] = entropy;
            statistics["Min"] = min;
            statistics["Max"] = max;

            return statistics;
        }

        public void AppendNewData(string filePath)
        {
            Dataset new_data = DataImporter.ImportData(filePath);
            _dataset.AddDataset(new_data);
        }

        public void SortColumn(string columnName)
        {
            _dataset.SortByColumn(columnName);
        }
    }
}
