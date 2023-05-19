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
            List<double> values = new();

            foreach (DataObject dataObject in _dataset.GetData())
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    values.Add(value);
                }
            }

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
                        case "deviation":
                            double deviation = CalculateStandardDeviation(values);
                            Console.WriteLine($"Standard deviation of {columnName}: {deviation}");
                            break;
                        default:
                            Console.WriteLine($"Calculation '{calculation}' is not supported.");
                            return;
                    }
                }
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

        public double CalculateColumnEntropy(string column)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            Dictionary<string, int> valueCounts = new Dictionary<string, int>();
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

        public double CalculateColumnDistance(string column, string value1, string value2)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            List<double> distances = new();

            string? value;
            foreach (DataObject dataObject in dataObjects)
            {
                if ((value = dataObject.GetColumnValue(column)) != null)
                {
                    if (value == value1 || value == value2)
                    {
                        double distance = CalculateObjectDistance(dataObject);
                        distances.Add(distance);
                    }
                }
            }

            return distances.Average();
        }

        private static double CalculateObjectDistance(DataObject dataObject)
        {
            List<double> values = new();

            // Assuming the dataObject has numeric columns
            foreach (double value in dataObject.GetNumericValues())
            {
                values.Add(value);
            }

            // Euclidean distance calculation
            double squaredSum = 0;

            foreach (double value in values)
            {
                squaredSum += Math.Pow(value, 2);
            }

            return Math.Sqrt(squaredSum);
        }

        public Dictionary<string, double> CalculateColumnCorrelation(string column1, string column2)
        {
            Dictionary<string, double> correlationResult = new();

            List<DataObject> dataObjects = _dataset.GetData();
            List<double> values1 = new();
            List<double> values2 = new();

            foreach (DataObject dataObject in dataObjects)
            {
                if (dataObject.TryGetNumericValue(column1, out double value1) && dataObject.TryGetNumericValue(column2, out double value2))
                {
                    values1.Add(value1);
                    values2.Add(value2);
                }
            }

            double correlation = CalculatePearsonCorrelation(values1, values2);
            correlationResult["Pearson Correlation"] = correlation;

            return correlationResult;
        }

        private static double CalculatePearsonCorrelation(List<double> values1, List<double> values2)
        {
            if (values1.Count != values2.Count)
            {
                throw new ArgumentException("The number of values in both lists must be the same.");
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

        private static double CalculateMean(List<double> values)
        {
            // Convert the string values to double and calculate the mean
            double mean = values.Average();
            return mean;
        }

        private static double CalculateMedian(List<double> values)
        {
            if (values.Count < 2)
            {
                return 0;
            }
            // Convert the string values to double and calculate the median
            double mid = (values.Count - 1) / 2.0;
            double median = (values.ElementAt((int)(mid)) + values.ElementAt((int)(mid + 0.5))) / 2;
            return median;
        }

        private static double CalculateStandardDeviation(List<double> values)
        {
            if (values.Count < 2)
            {
                return 0;
            }

            double mean = values.Average();
            double sumOfSquaredDifferences = values.Sum(value => Math.Pow(value - mean, 2));
            double variance = sumOfSquaredDifferences / (values.Count - 1);

            return Math.Sqrt(variance);
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

        public List<int> FindOutliers(string column, double threshold)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            List<int> outliers = new();

            if (!dataObjects.Any())
            {
                return outliers;
            }

            double mean = CalculateMean(column);
            double standardDeviation = CalculateStandardDeviation(column);

            foreach (DataObject dataObject in dataObjects)
            {
                if (dataObject.TryGetNumericValue(column, out double value))
                {
                    double zScore = (value - mean) / standardDeviation;

                    if (Math.Abs(zScore) > threshold)
                    {
                        outliers.Add(dataObject.Id);
                    }
                }
            }

            return outliers;
        }

        public Dictionary<string, int> CountOccurrences(string column)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            Dictionary<string, int> occurrences = new();

            string? value;
            foreach (DataObject dataObject in dataObjects)
            {
                if ((value = dataObject.GetColumnValue(column)) != null)
                {
                    if (occurrences.ContainsKey(value))
                    {
                        occurrences[value]++;
                    }
                    else
                    {
                        occurrences[value] = 1;
                    }
                }
            }

            return occurrences;
        }

        public Dictionary<string, double> CalculateColumnStatistics(string column)
        {
            Dictionary<string, double> statistics = new();

            double mean = CalculateMean(column);
            double stdDev = CalculateStandardDeviation(column);
            double min = double.MaxValue;
            double max = double.MinValue;

            List<DataObject> dataObjects = _dataset.GetData();

            foreach (DataObject dataObject in dataObjects)
            {
                if (dataObject.TryGetNumericValue(column, out double value))
                {
                    min = Math.Min(min, value);
                    max = Math.Max(max, value);
                }
            }

            statistics["Mean"] = mean;
            statistics["Standard Deviation"] = stdDev;
            statistics["Min"] = min;
            statistics["Max"] = max;

            return statistics;
        }
    }
}
