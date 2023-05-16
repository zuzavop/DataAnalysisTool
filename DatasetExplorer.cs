namespace DataAnalysisTool
{
    interface IDatasetExplorer
    {
        void ExploreDataset();
        void AnalyzeColumn(string column);
    }

    class DatasetExplorer : IDatasetExplorer
    {
        private Dataset _dataset;
        public DatasetExplorer(Dataset dataset)
        {
            _dataset = dataset;
        }

        public void ExploreDataset()
        {
            Console.WriteLine("Dataset Exploration");

            int totalObjects = _dataset.GetObjects().Count();
            Console.WriteLine($"Total objects in the dataset: {totalObjects}");

            Console.WriteLine("Available Columns:");
            foreach (string column in GetAvailableColumns())
            {
                Console.WriteLine(column);
            }
        }

        public void AnalyzeColumn(string column)
        {
            Console.WriteLine($"Analyzing Column: {column}");

            int count = CountColumnValues(column);
            Console.WriteLine($"Number of unique values: {count}");

            Console.WriteLine("Value\t\tCount");
            foreach (var entry in CountColumnValueOccurrences(column))
            {
                Console.WriteLine($"{entry.Key}\t\t{entry.Value}");
            }
        }

        private string[] GetAvailableColumns()
        {
            if (_dataset.GetObjects().Any())
            {
                DataObject firstObject = _dataset.GetObjects().First();
                return firstObject.GetColumns().Values.ToArray();
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

            foreach (DataObject dataObject in _dataset.GetObjects())
            {
                string value = dataObject.GetColumnValue(column);
                uniqueValues.Add(value);
            }

            return uniqueValues.Count;
        }

        private Dictionary<string, int> CountColumnValueOccurrences(string column)
        {
            // Count the occurrences of each value in the specified column
            Dictionary<string, int> valueOccurrences = new();

            foreach (DataObject dataObject in _dataset.GetObjects())
            {
                string value = dataObject.GetColumnValue(column);
                if (valueOccurrences.ContainsKey(value))
                {
                    valueOccurrences[value]++;
                }
                else
                {
                    valueOccurrences[value] = 1;
                }
            }

            return valueOccurrences;
        }
    }
}
