using MathNet.Numerics;
using System.Collections.Concurrent;
using System.Data;

namespace DataAnalysisTool
{
    /// <summary>
    /// Class responsible for processing data and performing various calculations and operations on a dataset.
    /// </summary>
    class DataProcessor
    {
        private readonly Dataset _dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessor"/> class with the specified dataset.
        /// </summary>
        /// <param name="dataset">The dataset to be processed.</param>
        public DataProcessor(Dataset dataset)
        {
            _dataset = dataset;
        }

        /// <summary>
        /// Performs calculations on a specified column of the dataset based on the specified operation.
        /// </summary>
        /// <param name="columnName">The name of the column to perform calculations on.</param>
        /// <param name="calculation">The type of calculation to perform (e.g., mean, median, deviation, entropy, mode, all).</param>
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

        /// <summary>
        /// Calculates the mean value of a numerical column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the numerical column.</param>
        /// <returns>The mean value of the specified column.</returns>
        private double CalculateMean(string columnName)
        {
            ControlNumericalColumn(columnName);
            List<double> values = _dataset.GetNumericColumnValues(columnName);

            double mean = values.Average();
            return mean;
        }

        /// <summary>
        /// Calculates the median value of a numerical column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the numerical column.</param>
        /// <returns>The median value of the specified column.</returns>
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

            double mid = (values.Count - 1) / 2.0;
            double median = (values.ElementAt((int)(mid)) + values.ElementAt((int)(mid + 0.5))) / 2;
            return median;
        }

        // <summary>
        /// Calculates the mode (most frequent value) of a categorical column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the categorical column.</param>
        /// <returns>The mode of the specified column.</returns>
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

        /// <summary>
        /// Calculates the standard deviation of a numerical column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the numerical column.</param>
        /// <returns>The standard deviation of the specified column.</returns>
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

        /// <summary>
        /// Calculates the entropy of a categorical column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the categorical column.</param>
        /// <returns>The entropy of the specified column.</returns>
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

        /// <summary>
        /// Performs regression analysis on two specified columns in the dataset.
        /// </summary>
        /// <param name="columnName1">The name of the first column for regression analysis.</param>
        /// <param name="columnName2">The name of the second column for regression analysis.</param>
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

        /// <summary>
        /// Calculates the Pearson correlation coefficient between two specified columns in the dataset.
        /// </summary>
        /// <param name="column1Name">The name of the first column for correlation calculation.</param>
        /// <param name="column2Name">The name of the second column for correlation calculation.</param>
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

        /// <summary>
        /// Calculates the Pearson correlation coefficient between two lists of values.
        /// </summary>
        /// <param name="values1">The first list of values.</param>
        /// <param name="values2">The second list of values.</param>
        /// <returns>The Pearson correlation coefficient between the two lists of values.</returns>
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

        /// <summary>
        /// Applies filters to the dataset based on a specified column, condition, and value.
        /// </summary>
        /// <param name="columnName">The name of the column to apply the filter to.</param>
        /// <param name="condition">The filter condition.</param>
        /// <param name="value">The value to compare against.</param>
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

        /// <summary>
        /// Cleans and preprocesses the data in the dataset by removing rows with missing values
        /// and normalizing numerical columns.
        /// </summary>
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

        /// <summary>
        /// Removes duplicate rows from the dataset.
        /// </summary>
        public void RemoveDuplicates()
        {
            _dataset.RemoveDuplicates();
            Console.WriteLine("Duplicates removed.");
        }

        /// <summary>
        /// Finds outliers in the specified numerical column based on the z-score.
        /// </summary>
        /// <param name="columnName">The name of the numerical column to find outliers in.</param>
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

        /// <summary>
        /// Calculates various statistics for the specified column in the dataset.
        /// </summary>
        /// <param name="columnName">The name of the column to calculate statistics for.</param>
        /// <returns>A dictionary containing the calculated statistics.</returns>
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

        /// <summary>
        /// Appends new data from the specified file to the dataset.
        /// </summary>
        /// <param name="filePath">The path of the file containing the new data.</param>
        public void AppendNewData(string filePath)
        {
            Dataset new_data = DataImporter.ImportData(filePath);
            _dataset.AddDataset(new_data);
            Console.WriteLine("New dataset was added.");
        }

        /// <summary>
        /// Sorts the dataset based on the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the column to sort by.</param>
        public void SortColumn(string columnName)
        {
            _dataset.SortByColumn(columnName);
            Console.WriteLine("Dataset was sorted.");
        }

        /// <summary>
        /// Checks if the dataset contains a column with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column to check.</param>
        private void ControlColumnName(string columnName)
        {
            if (!_dataset.GetColumnsNames().Contains(columnName))
            {
                throw new ProcessDatasetException($"Current dataset doesn't contain column with name {columnName}.");
            }
        }

        /// <summary>
        /// Checks if the dataset contains a numerical column with the specified name.
        /// </summary>
        /// <param name="columnName">The name of the column to check.</param>
        private void ControlNumericalColumn(string columnName)
        {
            if (!_dataset.GetNumericColumns().Contains(columnName))
            {
                throw new ProcessDatasetException($"Current dataset doesn't contain numerical column with name {columnName}.");
            }
        }
    }

    /// <summary>
    /// Exception class for dataset processing errors.
    /// </summary>
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
