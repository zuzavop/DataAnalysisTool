using System.Data;

namespace DataAnalysisTool
{
    /// <summary>
    /// Represents a dataset for storing and manipulating data objects.
    /// </summary>
    class Dataset
    {
        private List<DataObject> data;
        private readonly List<string> columnsNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dataset"/> class.
        /// </summary>
        public Dataset()
        {
            data = new List<DataObject>();
            columnsNames = new List<string>();
        }

        /// <summary>
        /// Sets the header of the dataset using the provided array of header names.
        /// </summary>
        /// <param name="header">An array of header names.</param>
        public void SetHeader(string[] header)
        {
            foreach (var item in header)
            {
                columnsNames.Add(item);
            }
        }

        /// <summary>
        /// Adds a new header name to the dataset.
        /// </summary>
        /// <param name="name">The name of the header to add.</param>
        public void AddHeaderName(string name)
        {
            columnsNames.Add(name);
        }

        /// <summary>
        /// Adds a new data object to the dataset.
        /// </summary>
        /// <param name="dataObject">The data object to add.</param>
        public void AddNewData(DataObject dataObject)
        {
            dataObject.Id = data.Count;
            data.Add(dataObject);
        }

        /// <summary>
        /// Adds a data object to the dataset while ensuring thread safety.
        /// </summary>
        /// <param name="dataObject">The data object to add.</param>
        public void AddData(DataObject dataObject)
        {
            lock(data)
                data.Add(dataObject);
        }

        /// <summary>
        /// Removes a data object from the dataset.
        /// </summary>
        /// <param name="dataObject">The data object to remove.</param>
        public void RemoveData(DataObject dataObject)
        {
            data.Remove(dataObject);
        }

        /// <summary>
        /// Returns the list of data objects in the dataset.
        /// </summary>
        /// <returns>The list of data objects.</returns>
        public List<DataObject> GetData()
        {
            return data;
        }

        /// <summary>
        /// Returns the list of column names in the dataset.
        /// </summary>
        /// <returns>The list of column names.</returns>
        public List<string> GetColumnsNames()
        {
            return columnsNames;
        }

        /// <summary>
        /// Filters the dataset by column value using the provided filtering functions.
        /// </summary>
        /// <param name="functions">An array of filtering functions.</param>
        /// <returns>The number of rows removed from the dataset.</returns>
        public int FilterByColumnValue(params Func<DataObject, bool>[] functions)
        {
            int beforeCount = data.Count;
            var func = functions.ToList();
            data = (from d in data
                        where functions.All(func => func(d))
                        select d).ToList();

            return beforeCount - data.Count;
        }

        /// <summary>
        /// Removes rows from the dataset that have missing values.
        /// </summary>
        public void RemoveRowsWithMissingValues()
        {
            data.RemoveAll(dataObject => dataObject.HasMissingValues(columnsNames));
        }

        /// <summary>
        /// Normalizes a column in the dataset using the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the column to normalize.</param>
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

            if (values.Count == 0)
            {
                return;
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

        /// <summary>
        /// Returns the list of numeric columns in the dataset.
        /// </summary>
        /// <returns>The list of numeric columns.</returns>
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

        /// <summary>
        /// Returns the list of numeric values in the specified column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The list of numeric values.</returns>
        public List<double> GetNumericColumnValues(string columnName)
        {
            if (!columnName.Contains(columnName))
            {
                throw new DatasetException($"Column '{columnName}' doesn't exist in current dataset.");
            }

            if (GetNumericColumns().Contains(columnName))
            {
                List<double> columnValues = new();

                Parallel.ForEach(data, row =>
                {
                    if (row.TryGetNumericValue(columnName, out double value))
                    {
                        lock (columnValues)
                            columnValues.Add(value);
                    }
                });

                return columnValues;
            }

            return new List<double>();
        }

        /// <summary>
        /// Adds another dataset to the current dataset.
        /// </summary>
        /// <param name="newData">The dataset to add.</param>
        public void AddDataset(Dataset newData)
        {
            foreach (DataObject dataObject in newData.GetData())
            {
                DataObject newRow = new();
                foreach (var valuePair in dataObject.columnValuePairs)
                {
                    if (columnsNames.Contains(valuePair.Key))
                        newRow.AddColumnValue(valuePair.Key, valuePair.Value);
                }

                if (newRow.columnValuePairs.Count > 0)
                {

                    AddNewData(newRow);
                }
            }
        }

        /// <summary>
        /// Sorts the dataset based on the specified column name.
        /// </summary>
        /// <param name="columnName">The name of the column to sort by.</param>
        public void SortByColumn(string columnName)
        {
            if (!columnName.Contains(columnName))
            {
                throw new DatasetException($"Column '{columnName}' does not exist.");
            }

            if (GetNumericColumns().Contains(columnName))
            {
                data = data.OrderBy(row => {
                    row.TryGetNumericValue(columnName, out double value);
                    return value;
                }).ToList();
            } 
            else
            {
                data = data.OrderBy(row => row.GetColumnValue(columnName)).ToList();
            }
        }

        /// <summary>
        /// Sorts the dataset based on the row ID.
        /// </summary>
        public void SortDataset()
        {
            data = data.OrderBy(row => row.Id).ToList();
        }

        /// <summary>
        /// Removes duplicate rows from the dataset.
        /// </summary>
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

    /// <summary>
    /// Represents a data object in a dataset.
    /// </summary>
    class DataObject
    {
        public readonly Dictionary<string, string> columnValuePairs;
        public int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObject"/> class.
        /// </summary>
        public DataObject()
        {
            columnValuePairs = new Dictionary<string, string>();
            Id = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObject"/> class with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the data object.</param>
        public DataObject(int id)
        {
            columnValuePairs = new Dictionary<string, string>();
            Id = id;
        }

        /// <summary>
        /// Returns the value of the specified column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>The value of the column, or null if the column doesn't exist.</returns>
        public string? GetColumnValue(string columnName)
        {
            return columnValuePairs.ContainsKey(columnName) ? columnValuePairs[columnName] : null;
        }

        /// <summary>
        /// Tries to parse the value of the specified column as a double.
        /// </summary>
        /// <param name="column">The name of the column.</param>
        /// <param name="value">The parsed numeric value.</param>
        /// <returns>True if the parsing is successful, otherwise false.</returns>
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

        /// <summary>
        /// Returns a dictionary containing all column names and their corresponding values.
        /// </summary>
        /// <returns>A dictionary of column names and values.</returns>
        public Dictionary<string, string> GetColumns()
        {
            return columnValuePairs;
        }

        /// <summary>
        /// Adds a new column value to the data object.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">The value to add.</param>
        public void AddColumnValue(string columnName, string value)
        {
            columnValuePairs.Add(columnName, value);
        }

        /// <summary>
        /// Sets the value of the specified column.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">The new value.</param>
        public void SetColumnValue(string columnName, string value)
        {
            columnValuePairs[columnName] = value;
        }

        /// <summary>
        /// Checks if the data object has a column-value pair with the specified column name and value.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the data object has the column-value pair; otherwise, <c>false</c>.</returns>
        public bool HasColumnValue(string columnName, string value)
        {
            return columnValuePairs.ContainsKey(columnName) && columnValuePairs[columnName].Equals(value);
        }

        /// <summary>
        /// Checks if the data object has a numerical column-value pair with the specified column name and value.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">The numerical value to check.</param>
        /// <returns><c>true</c> if the data object has the numerical column-value pair; otherwise, <c>false</c>.</returns>
        public bool HasColumnNumericValue(string columnName, double value)
        {
            return columnValuePairs.ContainsKey(columnName) && double.TryParse(columnName, out double columnValue) && columnValue.Equals(value);
        }

        /// <summary>
        /// Checks if the data object has missing values in the specified columns.
        /// </summary>
        /// <param name="columns">The list of columns to check.</param>
        /// <returns>True if the data object has missing values, otherwise false.</returns>
        public bool HasMissingValues(List<string> columnsNames)
        {
            return columnsNames.All(name => columnValuePairs.ContainsKey(name));
        }

        /// <summary>
        /// Gets the numeric values from the data object.
        /// </summary>
        /// <returns>A list of numeric values.</returns>
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

    /// <summary>
    /// Represents an exception specific to dataset operations.
    /// </summary>
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
