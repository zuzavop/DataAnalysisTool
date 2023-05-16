using OfficeOpenXml;
using System.Data;

namespace DataAnalysisTool
{
    interface IDataImporter
    {
        Dataset ImportData(string filePath);
    }
    class DataImporter : IDataImporter
    {
        public Dataset ImportData(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            // Create a new DataSet object
            Dataset dataset = new Dataset();

            // Perform data import logic based on the specific file format
            string fileExtension = Path.GetExtension(filePath);
            switch (fileExtension.ToLower())
            {
                case ".csv":
                    ImportCsvData(filePath, dataset);
                    break;
                case ".json":
                    ImportJsonData(filePath, dataset);
                    break;
                case ".xlsx":
                    ImportExcelData(filePath, dataset);
                    break;
                // Add more cases for additional file formats if needed
                default:
                    throw new NotSupportedException("File format not supported.");
            }

            return dataset;
        }

        private void ImportCsvData(string filePath, Dataset dataset)
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                foreach (string value in values)
                {
                    DataObject dataObject = new DataObject(value);
                    dataset.Add(dataObject);
                }
            }
        }

        private void ImportJsonData(string filePath, Dataset dataset)
        {
            string jsonData = File.ReadAllText(filePath);

            // Deserialize the JSON data into a collection of objects
            List<JsonDataObject> jsonObjects = null; // JsonConvert.DeserializeObject<List<JsonDataObject>>(jsonData);

            // Create DataObject instances based on the deserialized JSON objects and add them to the DataSet
            foreach (JsonDataObject jsonObject in jsonObjects)
            {
                DataObject dataObject = new DataObject(jsonObject.Value);
                dataset.Add(dataObject);
            }
        }

        private class JsonDataObject
        {
            public string Value { get; set; }
        }

        private void ImportExcelData(string filePath, Dataset dataset)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;
                int columnCount = worksheet.Dimension.Columns;

                for (int row = 1; row <= rowCount; row++)
                {
                    for (int column = 1; column <= columnCount; column++)
                    {
                        object cellValue = worksheet.Cells[row, column].Value;

                        if (cellValue != null)
                        {
                            string value = cellValue.ToString();
                            DataObject dataObject = new DataObject(value);
                            dataset.Add(dataObject);
                        }
                    }
                }
            }
        }
    }
}
