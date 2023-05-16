namespace DataAnalysisTool
{
    class Dataset
    {
        private List<DataObject> dataObjects;

        public Dataset()
        {
            dataObjects = new List<DataObject>();
        }

        public void Add(DataObject dataObject)
        {
            dataObjects.Add(dataObject);
        }

        public void Remove(DataObject dataObject)
        {
            dataObjects.Remove(dataObject);
        }

        public IEnumerable<DataObject> GetObjects()
        {
            return dataObjects;
        }

        internal void FilterByColumnValue(string column, string value)
        {
            dataObjects = dataObjects.Where(dataObject => dataObject.HasColumnValue(column, value)).ToList();
        }
    }

    class DataObject
    {
        private Dictionary<string, string> data;

        public DataObject()
        {
            data = new Dictionary<string, string>();
        }

        public void AddColumnValue(string column, string value)
        {
            data[column] = value;
        }

        public bool HasColumnValue(string column, string value)
        {
            return data.ContainsKey(column) && data[column] == value;
        }

        internal Dictionary<string, string> GetColumns()
        {
            return data;
        }

        public string GetColumnValue(string column)
        {
            return data[column];
        }
    }
}
