using MathNet.Numerics;
using System.Collections.Concurrent;
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
            ControlNumericalColumn(columnName);
            List<double> values = _dataset.GetNumericColumnValues(columnName);

            // Convert the string values to double and calculate the mean
            double mean = values.Average();
            return mean;
        }

        private double CalculateMedian(string columnName)
        {
            ControlNumericalColumn(columnName);
            List<double> values = _dataset.GetNumericColumnValues(columnName);
            values.Sort();

            if (values.Count == 0)
            {
                return 0;
            }
            else if (values.Count == 1)
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
            ConcurrentDictionary<string, int> valueCounts = new();

            Parallel.ForEach(_dataset.GetData(), row =>
            {
                string? value = row.GetColumnValue(columnName);
                if (value != null)
                {
                    if (valueCounts.ContainsKey(value))
                        valueCounts[value]++;
                    else
                        valueCounts[value] = 1;
                }
            });

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
            ControlNumericalColumn(columnName);
            List<double> values = _dataset.GetNumericColumnValues(columnName);

            if (values.Count < 2)
            {
                return 0;
            }

            double mean = values.Average();
            double sumOfSquaredDifferences = values.Sum(value => Math.Pow(value - mean, 2));
            double variance = sumOfSquaredDifferences / (values.Count - 1);

            return Math.Sqrt(variance);
        }

        private double CalculateColumnEntropy(string columnName)
        {
            List<DataObject> dataObjects = _dataset.GetData();
            ConcurrentDictionary<string, int> valueCounts = new();
            int totalCount = 0;

            Parallel.ForEach(_dataset.GetData(), row =>
            {
                string? value;
                if ((value = row.GetColumnValue(columnName)) != null)
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
            });

            double entropy = 0;

            foreach (int count in valueCounts.Values)
            {
                double probability = (double)count / totalCount;
                entropy -= probability * Math.Log(probability, 2);
            }

            return entropy;
        }

        public void PerformRegressionAnalysis(string columnName1, string columnName2)
        {
            ControlNumericalColumn(columnName1);
            ControlNumericalColumn(columnName2);

            List<double> xData = new();
            List<double> yData = new();

            Parallel.ForEach(_dataset.GetData(), row =>
            {
                if (row.TryGetNumericValue(columnName1, out double value1) && row.TryGetNumericValue(columnName2, out double value2))
                {
                    lock(xData)
                        xData.Add(value1);
                    lock(yData)
                        yData.Add(value2);
                }
            });

            var (A, B) = Fit.Line(xData.ToArray(), yData.ToArray());

            double slope = A;
            double intercept = B;

            Console.WriteLine($"Regression equation: y = {slope} * x + {intercept}");

            double[] predictedY = xData.Select(x => slope * x + intercept).ToArray();
            double rSquared = GoodnessOfFit.RSquared(yData, predictedY);

            Console.WriteLine($"R-squared: {rSquared}");
        }

        public void CalculateColumnCorrelation(string column1Name, string column2Name)
        {
            ControlNumericalColumn(column1Name);
            ControlNumericalColumn(column2Name);

            List<double> values1 = new();
            List<double> values2 = new();

            Parallel.ForEach(_dataset.GetData(), row =>
            {
                if (row.TryGetNumericValue(column1Name, out double value1) && row.TryGetNumericValue(column2Name, out double value2))
                {
                    lock(values1)
                        values1.Add(value1);
                    lock(values2)
                        values2.Add(value2);
                }
            });

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

        public void ApplyFilters(string columnName, string condition, string value)
        {
            ControlColumnName(columnName);

            Func<DataObject, bool> filter;

            if (_dataset.GetNumericColumns().Contains(columnName) && double.TryParse(value, out double numericValue))
            {
                filter = condition switch
                {
                    "=" => row => row.HasColumnNumericValue(columnName, numericValue),
                    "!=" => row => !row.HasColumnNumericValue(columnName, numericValue),
                    ">" => row => row.TryGetNumericValue(columnName, out double val) && val > numericValue,
                    "<" => row => row.TryGetNumericValue(columnName, out double val) && val < numericValue,
                    "=>" => row => row.TryGetNumericValue(columnName, out double val) && val >= numericValue,
                    ">=" => row => row.TryGetNumericValue(columnName, out double val) && val >= numericValue,
                    "=<" => row => row.TryGetNumericValue(columnName, out double val) && val <= numericValue,
                    "<=" => row => row.TryGetNumericValue(columnName, out double val) && val <= numericValue,
                    "in" => row => row.GetColumnValue(columnName)?.Contains(value) == true,
                    _ => throw new ProcessDatasetException($"Condition {condition} is not allowed. For possible choices of conditions use the `help` command.")
                };
            }
            else
            {
                filter = condition switch
                {
                    "=" => row => row.HasColumnValue(columnName, value),
                    "!=" => row => !row.HasColumnValue(columnName, value),
                    ">" => row => row.GetColumnValue(columnName)?.Length > value.Length,
                    "<" => row => row.GetColumnValue(columnName)?.Length < value.Length,
                    "=>" => row => row.GetColumnValue(columnName)?.Length >= value.Length,
                    "=<" => row => row.GetColumnValue(columnName)?.Length <= value.Length,
                    ">=" => row => row.GetColumnValue(columnName)?.Length >= value.Length,
                    "<=" => row => row.GetColumnValue(columnName)?.Length <= value.Length,
                    "in" => row => row.GetColumnValue(columnName)?.Contains(value) == true,
                    _ => throw new ProcessDatasetException($"Condition {condition} is not allowed. For possible choices of conditions use the `help` command.")
                };
            }

            int filtred = _dataset.FilterByColumnValue(filter);
            Console.WriteLine($"Filtered data: { filtred } rows.");

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
            Console.WriteLine("Dataset was cleaned.");
        }

        public void RemoveDuplicates()
        {
            _dataset.RemoveDuplicates();
            Console.WriteLine("Duplicates removed.");
        }

        public void FindOutliers(string columnName)
        {
            ControlNumericalColumn(columnName);
            ConcurrentDictionary<int, double> outliers = new();

            double mean = CalculateMean(columnName);
            double standardDeviation = CalculateStandardDeviation(columnName);

            Parallel.ForEach(_dataset.GetData(), row =>
            {
                if (row.TryGetNumericValue(columnName, out double value))
                {
                    double zScore = (value - mean) / standardDeviation;

                    if (Math.Abs(zScore) > 3)
                    {
                        outliers.TryAdd(row.Id, value);
                    }
                }
            });

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
            Console.WriteLine("New dataset was added.");
        }

        public void SortColumn(string columnName)
        {
            _dataset.SortByColumn(columnName);
            Console.WriteLine("Dataset was sorted.");
        }

        private void ControlColumnName(string columnName)
        {
            if (!_dataset.GetColumnsNames().Contains(columnName))
            {
                throw new ProcessDatasetException($"Current dataset doesn't contain column with name {columnName}.");
            }
        }

        private void ControlNumericalColumn(string columnName)
        {
            if (!_dataset.GetNumericColumns().Contains(columnName))
            {
                throw new ProcessDatasetException($"Current dataset doesn't contain numerical column with name {columnName}.");
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
