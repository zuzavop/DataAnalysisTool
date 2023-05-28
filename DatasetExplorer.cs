namespace DataAnalysisTool
{
    /// <summary>
    /// Represents a dataset explorer for analyzing dataset information and columns.
    /// </summary>
    class DatasetExplorer
    {
        private readonly Dataset _dataset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatasetExplorer"/> class.
        /// </summary>
        /// <param name="dataset">The dataset to explore.</param>
        public DatasetExplorer(Dataset dataset)
        {
            _dataset = dataset;
        }

        /// <summary>
        /// Explores the dataset, providing an overview of the dataset information.
        /// </summary>
        /// <param name="columnName">The optional column name to explore.</param>
        public void ExploreDataset(string columnName = "")
        {
            if (columnName == "")
            {
                ExploreDataset();
            }
            else
            {
                ExploreColumn(columnName);
            }
        }

        /// <summary>
        /// Explores the dataset, providing an overview of the dataset information.
        /// </summary>
        private void ExploreDataset()
        {
            int totalObjects = _dataset.GetData().Count;
            Console.WriteLine($"Total objects in the dataset: {totalObjects}");

            Console.Write("Available Columns:");
            var columnsNames = GetAvailableColumns();
            if (columnsNames.Length > 0)
            {
                foreach (string column in columnsNames)
                {
                    Console.Write(column + " ");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("[]");
            }
        }

        /// <summary>
        /// Explores a specific column in the dataset, providing analysis and statistics.
        /// </summary>
        /// <param name="columnName">The name of the column to explore.</param>
        private void ExploreColumn(string columnName)
        {
            if (!_dataset.GetColumnsNames().Contains(columnName))
            {
                throw new ExploreDatasetException($"Current dataset doesn't contain column with name {columnName}.");
            }

            Console.WriteLine($"Analyzing Column: {columnName}");

            var occuration = CountColumnValueOccurrences(columnName);
            Console.WriteLine($"Number of unique values: {occuration.Count}");

            Console.WriteLine($"{"Value", -20}Count");
            foreach (var entry in occuration)
            {
                Console.Write("{0, -20}", entry.Key);
                Console.WriteLine($"{entry.Value}");
            }
        }

        /// <summary>
        /// Retrieves the available columns in the dataset.
        /// </summary>
        /// <returns>An array of available column names.</returns>
        private string[] GetAvailableColumns()
        {
            if (_dataset.GetData().Any())
            {
                return _dataset.GetColumnsNames().ToArray();
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Counts the occurrences of each value in the specified column.
        /// </summary>
        /// <param name="column">The name of the column.</param>
        /// <returns>A dictionary containing the value occurrences.</returns>
        private Dictionary<string, int> CountColumnValueOccurrences(string column)
        {
            // Count the occurrences of each value in the specified column
            Dictionary<string, int> valueOccurrences = new();

            string? value;
            foreach (DataObject dataObject in _dataset.GetData())
            {
                if ((value = dataObject.GetColumnValue(column)) != null)
                {
                    if (valueOccurrences.ContainsKey(value))
                    {
                        valueOccurrences[value]++;
                    }
                    else
                    {
                        valueOccurrences[value] = 1;
                    }
                }
            }

            return valueOccurrences;
        }
    }

    /// <summary>
    /// Represents an exception related to exploring datasets.
    /// </summary>
    class ExploreDatasetException : DataAnalysisException
    {
        public ExploreDatasetException()
        {
        }

        public ExploreDatasetException(string message)
            : base(message)
        {
        }

        public ExploreDatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
