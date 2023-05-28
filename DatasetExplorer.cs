namespace DataAnalysisTool
{
    class DatasetExplorer
    {
        private readonly Dataset _dataset;
        public DatasetExplorer(Dataset dataset)
        {
            _dataset = dataset;
        }

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
