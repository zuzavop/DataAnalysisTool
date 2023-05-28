using System.Data;

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

        private Dataset(List<DataObject> data, List<string> columnsNames)
        {
            this.data = data;
            this.columnsNames = columnsNames;
        }

        public void SetHeader(string[] header)
        {
            foreach (var item in header)
            {
                columnsNames.Add(item);
            }
        }

        public void AddHeaderName(string name)
        {
            columnsNames.Add(name);
        }

        public void AddData(DataObject dataObject)
        {
            dataObject.Id = data.Count;
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

        public void FilterByColumnValue(params Func<DataObject, bool>[] functions)
        {
            data = (List<DataObject>)(from d in data
                                      where functions.All(func => func(d))
                                      select d);
        }

        public void RemoveRowsWithMissingValues()
        {
            data.RemoveAll(dataObject => dataObject.HasMissingValues(columnsNames));
        }

        public void NormalizeColumn(string columnName)
        {
            if (!columnName.Contains(columnName))
            {
                throw new DatasetException($"Column '{columnName}' does not exist.");
            }

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

        public List<string> GetNumericColumns()
        {
            List<string> numericColumns = new();

            foreach (string column in columnsNames)
            {
                bool isNumeric = true;

                foreach (DataObject dataObject in data)
                {
                    if (dataObject.columnValuePairs.ContainsKey(column) && !dataObject.TryGetNumericValue(column, out _))
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

        public List<double> GetNumericColumnValues(string columnName)
        {
            if (!columnName.Contains(columnName))
            {
                throw new DatasetException($"Column '{columnName}' does not exist.");
            }

            if (GetNumericColumns().Contains(columnName))
            {
                List<double> columnValues = new();

                foreach (DataObject dataObject in data)
                {
                    if (dataObject.TryGetNumericValue(columnName, out double value))
                    {
                        columnValues.Add(value);
                    }
                }

                return columnValues;
            }

            return new List<double>();
        }

        public void AddDataset(Dataset newData)
        {
            int maxId = data.Max(value => value.Id) + 1;
            foreach (DataObject dataObject in newData.GetData())
            {
                DataObject newRow = new(maxId);
                foreach (var valuePair in dataObject.columnValuePairs)
                {
                    if (columnsNames.Contains(valuePair.Key))
                        newRow.AddColumnValue(valuePair.Key, valuePair.Value);
                }

                if (newRow.columnValuePairs.Count > 0)
                {

                    AddData(newRow);
                    ++maxId;
                }
            }
        }

        public void SortByColumn(string columnName)
        {
            if (!columnName.Contains(columnName))
            {
                throw new DatasetException($"Column '{columnName}' does not exist.");
            }

            data = data.OrderBy(row => row.GetColumnValue(columnName)).ToList();
        }

        public void RemoveDuplicates()
        {
            HashSet<string> uniqueRows = new();

            foreach (var row in data)
            {
                var rowValues = String.Join(",", row.columnValuePairs.Values.ToList());

                if (rowValues == null || uniqueRows.Contains(rowValues))
                {
                    RemoveData(row);
                }
                else
                {
                    uniqueRows.Add(rowValues);
                }
            }
        }
    }

    class DataObject
    {
        public readonly Dictionary<string, string> columnValuePairs;
        public int Id { get; set; }

        public DataObject(int iD)
        {
            columnValuePairs = new Dictionary<string, string>();
            Id = iD;
        }

        public string? GetColumnValue(string columnName)
        {
            return columnValuePairs.ContainsKey(columnName) ? columnValuePairs[columnName] : null;
        }

        public bool TryGetNumericValue(string column, out double value)
        {
            string? stringValue;
            if ((stringValue = GetColumnValue(column)) != null)
            {
                return double.TryParse(stringValue, out value);
            }

            value = 0;
            return false;
        }

        public Dictionary<string, string> GetColumns()
        {
            return columnValuePairs;
        }

        public void AddColumnValue(string columnName, string value)
        {
            columnValuePairs.Add(columnName, value);
        }

        public void SetColumnValue(string columnName, string value)
        {
            columnValuePairs[columnName] = value;
        }

        public bool HasColumnValue(string columnName, string value)
        {
            return columnValuePairs.ContainsKey(columnName) && columnValuePairs[columnName].Equals(value);
        }

        public bool HasColumnNumericValue(string columnName, double value)
        {
            return columnValuePairs.ContainsKey(columnName) && double.TryParse(columnName, out double columnValue) && columnValue.Equals(value);
        }

        public bool HasMissingValues(List<string> columnsNames)
        {
            return columnsNames.All(name => columnValuePairs.ContainsKey(name));
        }

        public List<double> GetNumericValues()
        {
            List<double> values = new();

            foreach (var column in columnValuePairs)
            {
                if (TryGetNumericValue(column.Key, out double value))
                {
                    values.Add(value);
                }
            }

            return values;
        }
    }

    class DatasetException : DataAnalysisException
    {
        public DatasetException()
        {
        }

        public DatasetException(string message)
            : base(message)
        {
        }

        public DatasetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
