namespace DataAnalysisTool
{
    class DataVisualizer
    {
        private readonly Dataset _dataset;
        public DataVisualizer(Dataset dataset)
        {
            this._dataset = dataset;
        }

        public void PrintAllData()
        {
            string[] columns = _dataset.GetColumnsNames().ToArray();

            PrintColumns(columns);
        }

        private void PrintColumns(string[] columns)
        {
            // Print column headers
            PrintRow(columns.ToList());

            string? value;
            // Print data rows
            foreach (var dataObject in _dataset.GetData())
            {
                List<string> values = new();

                foreach (var column in columns)
                {
                    if ((value = dataObject.GetColumnValue(column)) != null)
                    {
                        values.Add(value);
                    }
                    else
                    {
                        values.Add("");
                    }
                }

                PrintRow(values);
            }
        }

        private static void PrintRow(List<string> values)
        {
            Console.WriteLine(string.Join("\t", values));
        }

        public void CreateAndSaveLinePlot()
        {

        }

        public void CreateAndSaveBarPlot()
        {

        }

        public void CreateAndSaveScatterPlot()
        {

        }

        public void CreateAndSavePiePlot()
        {

        }

        public void CreateAndSaveHistogram()
        {

        }
    }
}
