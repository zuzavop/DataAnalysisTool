namespace DataAnalysisTool
{
    interface IDataVisualizer
    {
        void VisualizeData(string[] columns, string chartType);
    }

    class DataVisualizer : IDataVisualizer
    {
        private Dataset _dataset;
        public DataVisualizer(Dataset dataset)
        {
            this._dataset = dataset;
        }
        public void VisualizeData(string[] columns, string chartType)
        {
            Console.WriteLine("Data Visualization");

            foreach (DataObject dataObject in _dataset.GetObjects())
            {
                foreach (var kvp in dataObject.GetColumns())
                {
                    string column = kvp.Key;
                    string value = kvp.Value;

                    Console.WriteLine($"{column}: {value}");
                }
            }
        }
    }
}
