namespace DataAnalysisTool
{
    interface IDatasetExplorer
    {
        void ExploreDataset(Dataset dataset);
    }

    class DatasetExplorer : IDatasetExplorer 
    {
        public void ExploreDataset(Dataset dataset)
        {
            Console.WriteLine("Dataset Exploration");

            int totalObjects = dataset.GetObjects().Count();
            Console.WriteLine($"Total objects in the dataset: {totalObjects}");

            Console.WriteLine("Object Values:");
            foreach (DataObject dataObject in dataset.GetObjects())
            {
                Console.WriteLine(dataObject.Value);
            }

            Console.WriteLine("Object Statistics:");
            double minValue = dataset.GetObjects().Min(dataObject => double.Parse(dataObject.Value));
            double maxValue = dataset.GetObjects().Max(dataObject => double.Parse(dataObject.Value));
            double averageValue = dataset.GetObjects().Average(dataObject => double.Parse(dataObject.Value));

            Console.WriteLine($"Min Value: {minValue}");
            Console.WriteLine($"Max Value: {maxValue}");
            Console.WriteLine($"Average Value: {averageValue}");
        }
    }
}
