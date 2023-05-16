using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisTool
{
    interface IDataVisualizer
    {
        void VisualizeData(Dataset dataset, string[] columns, string chartType);
    }

    class DataVisualizer : IDataVisualizer
    {
        public void VisualizeData(Dataset dataset, string[] columns, string chartType)
        {
            Console.WriteLine("Data Visualization");

            foreach (DataObject dataObject in dataset.GetObjects())
            {
                string value = dataObject.Value;
                int count = int.Parse(value);

                string visualization = new string('*', count);
                Console.WriteLine($"{value}: {visualization}");
            }
        }
    }
}
