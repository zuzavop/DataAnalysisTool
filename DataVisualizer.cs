using System.Data;

namespace DataAnalysisTool
{
    class DataVisualizer
    {
        private Dataset _dataset;
        public DataVisualizer(Dataset dataset)
        {
            this._dataset = dataset;
        }

        public void PrintAllData()
        {
            string[] columns = _dataset.GetColumnsNames().ToArray();

            PrintData(columns);
        }

        public void PrintData(string[] columns)
        {
            Console.WriteLine("Data Visualization");

            // Print column headers
            PrintRow(columns.ToList());

            // Print data rows
            foreach (var dataObject in _dataset.GetData())
            {
                List<string> values = new();

                foreach (var column in columns)
                {
                    if (dataObject.TryGetColumnValue(column, out string value))
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
    }
}
