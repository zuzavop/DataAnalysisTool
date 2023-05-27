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
            ControlColumnName(columnName);

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
                case "mode":
                    string mode = CalculateColumnMode(columnName);
                    Console.WriteLine($"Mode od column {columnName}: {mode}");
                    break;
                case "all":
                    var values = CalculateColumnStatistics(columnName);
                    foreach (var value in values)
                    {
                        Console.WriteLine($"{value.Key} of column {columnName}: {value.Value}");
                    }
                    break;
                default:
                    throw new ProcessDatasetException($"Calculation '{calculation}' is not supported.");
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

            if (values.Count == 0)
            {
                return 0;
            } else if (values.Count == 1)
            {
                return values[0];
            }

            // Convert the string values to double and calculate the median
            double mid = (values.Count - 1) / 2.0;
            double median = (values.ElementAt((int)(mid)) + values.ElementAt((int)(mid + 0.5))) / 2;
            return median;
        }

        private string CalculateColumnMode(string columnName)
        {
            List<double> values = new();
            Dictionary<string, int> valueCounts = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                string? value = dataObject.GetColumnValue(columnName);
                if (value != null)
                {
                    if (valueCounts.ContainsKey(value))
                        valueCounts[value]++;
                    else
                        valueCounts[value] = 1;
                }
            }

            int maxCount = valueCounts.Values.Max();
            List<string> modes = valueCounts.Where(kv => kv.Value == maxCount).Select(kv => kv.Key).ToList();

            if (modes.Count == 0)
                throw new ProcessDatasetException("No mode found.");

            if (modes.Count > 1)
                throw new ProcessDatasetException("Multiple modes found.");

            return modes[0];
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
            ControlColumnName(column1Name);
            ControlColumnName(column2Name);

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
                throw new ProcessDatasetException("The number of values in both lists must be the same.");
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

        public void ApplyFilters(string columnName, string value)
        {
            // Filter the Dataset based on the specified column and value
            _dataset.FilterByColumnValue(columnName, value);
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

        public void RemoveDuplicates()
        {
            _dataset.RemoveDuplicates();
        }

        public void FindOutliers(string columnName)
        {
            ControlColumnName(columnName);
            Dictionary<int, double> outliers = new();

            double mean = CalculateMean(columnName);
            double standardDeviation = CalculateStandardDeviation(columnName);

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
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

        private void ControlColumnName(string columnName)
        {
            if (!_dataset.GetColumnsNames().Contains(columnName))
            {
                throw new ProcessDatasetException($"Current dataset doesn't contain column with name {columnName}.");
            }
        }
    }

    class ProcessDatasetException : DataAnalysisException
    {
        public ProcessDatasetException()
        {
        }

        public ProcessDatasetException(string message)
            : base(message)
        {
        }

        public ProcessDatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
