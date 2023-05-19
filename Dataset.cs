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

        public void FilterByColumnValue(string column, string value)
        {
            data = data.Where(dataObject => dataObject.HasColumnValue(column, value)).ToList();
        }

        public void FilterByColumnValue(Func<DataObject, bool> f)
        {
            data = (List<DataObject>)(from d in data
                   where f(d)
                   select d);
        }

        public Dataset GetFilterDataset(string column, string value)
        {
            List<DataObject> new_data = data.Where(dataObject => dataObject.HasColumnValue(column, value)).ToList();
            return new Dataset(new_data, columnsNames);
        }

        public Dataset GetFilterDataset(Func<DataObject, bool> f)
        {
            List<DataObject> new_data = (List<DataObject>)(from d in data
                                      where f(d)
                                      select d);
            return new Dataset(new_data, columnsNames);
        }

        public void RemoveRowsWithMissingValues()
        {
            data.RemoveAll(dataObject => dataObject.HasMissingValues(columnsNames));
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

        public List<string> GetNumericColumns()
        {
            List<string> numericColumns = new();

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

        internal List<double> GetNumericColumnValues(string columnName)
        {
            if (columnName.Contains(columnName) && GetNumericColumns().Contains(columnName))
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
    }

    class DataObject
    {
        private readonly Dictionary<string, string> columnValuePairs;
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
            return columnValuePairs.ContainsKey(columnName) && columnValuePairs[columnName] == value;
        }

        public bool HasColumnNumericValue(string columnName, double value)
        {
            return columnValuePairs.ContainsKey(columnName) && double.TryParse(columnName, out double columnValue) && columnValue == value;
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
}
