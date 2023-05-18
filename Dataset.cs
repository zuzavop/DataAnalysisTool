namespace DataAnalysisTool
{
    class Dataset
    {
        private List<DataObject> data;
        private readonly List<string> columnsNames;

        public Dataset()
        {
            data = new List<DataObject>();
            columnsNames = new List<string>();
        }

        public void SetHeader(string[] header)
        {
            foreach (var item in header)
            {
                columnsNames.Add(item);
            }
        }

        public void AddData(DataObject dataObject)
        {
            data.Add(dataObject);
        }

        public void RemoveData(DataObject dataObject)
        {
            data.Remove(dataObject);
        }

        public List<DataObject> GetData()
        {
            return data;
        }

        public List<string> GetColumnsNames()
        {
            return columnsNames;
        }

        internal void FilterByColumnValue(string column, string value)
        {
            data = data.Where(dataObject => dataObject.HasColumnValue(column, value)).ToList();
        }

        public void RemoveRowsWithMissingValues()
        {
            data.RemoveAll(dataObject => dataObject.HasMissingValues());
        }

        public void NormalizeColumn(string columnName)
        {
            List<double> values = new();

            foreach (DataObject dataObject in data)
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    values.Add(value);
                }
            }

            double min = values.Min();
            double max = values.Max();

            foreach (DataObject dataObject in data)
            {
                if (dataObject.TryGetNumericValue(columnName, out double value))
                {
                    double normalizedValue = (value - min) / (max - min);
                    dataObject.SetColumnValue(columnName, normalizedValue.ToString());
                }
            }
        }

        internal List<string> GetNumericColumns()
        {
            List<string> numericColumns = new List<string>();

            foreach (string column in columnsNames)
            {
                bool isNumeric = true;

                foreach (DataObject dataObject in data)
                {
                    if (!dataObject.TryGetNumericValue(column, out _))
                    {
                        isNumeric = false;
                        break;
                    }
                }

                if (isNumeric)
                {
                    numericColumns.Add(column);
                }
            }

            return numericColumns;
        }
    }

    class DataObject
    {
        private readonly Dictionary<string, string?> columnValuePairs;

        public DataObject()
        {
            columnValuePairs = new Dictionary<string, string?>();
        }

        public bool TryGetColumnValue(string column, out string value)
        {
            return columnValuePairs.TryGetValue(column, out value);
        }

        public string? GetColumnValue(string columnName)
        {
            return columnValuePairs.ContainsKey(columnName) ? columnValuePairs[columnName] : null;
        }

        public bool TryGetNumericValue(string column, out double value)
        {
            if (TryGetColumnValue(column, out string stringValue))
            {
                return double.TryParse(stringValue, out value);
            }

            value = 0;
            return false;
        }

        public bool HasMissingValues()
        {
            return columnValuePairs.Values.Any(string.IsNullOrEmpty);
        }

        public Dictionary<string, string?> GetColumns()
        {
            return columnValuePairs;
        }

        public void SetColumnValue(string column, string value)
        {
            columnValuePairs[column] = value;
        }

        public bool HasColumnValue(string column, string value)
        {
            return columnValuePairs.ContainsKey(column) && columnValuePairs[column] == value;
        }

        public bool HasColumnNumericValue(string column, double value)
        {
            return columnValuePairs.ContainsKey(column) && double.TryParse(column, out double columnValue) && columnValue == value;
        }
    }
}
