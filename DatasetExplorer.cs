namespace DataAnalysisTool
{
    class DatasetExplorer
    {
        private readonly Dataset _dataset;
        public DatasetExplorer(Dataset dataset)
        {
            _dataset = dataset;
        }

        public void ExploreDataset()
        {
            int totalObjects = _dataset.GetData().Count;
            Console.WriteLine($"Total objects in the dataset: {totalObjects}");

            Console.Write("Available Columns:");
            var columnsNames = GetAvailableColumns();
            if (columnsNames.Length > 0 )
            {
                foreach (string column in columnsNames)
                {
                    Console.WriteLine(column);
                }
            } else
            {
                Console.WriteLine("[]");
            }
        }

        public void ExploreColumn(string columnName)
        {
            if (_dataset.GetColumnsNames().Contains(columnName))
            {
                Console.WriteLine($"Current dataset doesn't contain column with name {columnName}".);
                return;
            }
            Console.WriteLine($"Analyzing Column: {columnName}");

            int count = CountColumnValues(columnName);
            Console.WriteLine($"Number of unique values: {count}");

            Console.WriteLine("Value\t\tCount");
            foreach (var entry in CountColumnValueOccurrences(columnName))
            {
                Console.WriteLine($"{entry.Key}\t\t{entry.Value}");
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

        private int CountColumnValues(string column)
        {
            // Count the number of unique values in the specified column
            HashSet<string> uniqueValues = new();

            string? value;
            foreach (DataObject dataObject in _dataset.GetData())
            {
                if ((value = dataObject.GetColumnValue(column)) != null)
                {
                    uniqueValues.Add(value);
                }
            }

            return uniqueValues.Count;
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
}
