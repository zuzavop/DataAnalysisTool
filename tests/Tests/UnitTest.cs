using DataAnalysisTool;

namespace Tests
{
    [TestClass]
    public class DataAnalysisToolTests
    {
        private DataAnalyzer _dataAnalysisTool;

        [TestInitialize]
        public void TestInitialize()
        {
            // Initialize the Data Analysis Tool instance
            _dataAnalysisTool = new DataAnalyzer();
            _dataAnalysisTool.Run("../../../test.csv", false);
        }

        [TestMethod]
        public void TestExploreDataset()
        {
            string columnName = "Age";
            string expectedOutput = "Analyzing Column: Age";

            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand($"explore {columnName}");

            string output = sw.ToString();

            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void TestPrintAllData()
        {
            string expectedOutput = "      Name       Age    Gender   Country    Salary\r\n" +
                                    "      John        30      Male       USA     50000\r\n" +
                                    "     Sarah        25    Female    Canada     60000\r\n" +
                                    "   Michael        35      Male        UK     45000";
            
            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand($"show");

            string output = sw.ToString();

            Console.WriteLine(output);

            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void TestApplyFiltersEqual()
        {
            string columnName = "Country";
            string condition = "!=";
            string value = "USA";

            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand($"filter {columnName} {condition} {value}");

            string output = sw.ToString();

            string expectedOutput = "Filtered data: 1 rows";
            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void TestApplyFiltersGreaterOrEqual()
        {
            string columnName = "Age";
            string condition = ">=";
            string value = "30";

            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand($"filter {columnName} {condition} {value}");

            string output = sw.ToString();

            string expectedOutput = "Filtered data: 1 rows";
            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void TestCleanAndPreprocessData()
        {
            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand("clean");

            string output = sw.ToString();

            string expectedOutput = "Dataset was cleaned";
            Assert.IsTrue(output.Contains(expectedOutput));
        }

        [TestMethod]
        public void TestRemoveDuplicates()
        {
            using StringWriter sw = new();
            Console.SetOut(sw);

            _dataAnalysisTool.ExecuteCommand("remove_duplicates");

            string output = sw.ToString();

            string expectedOutput = "Duplicates removed.";
            Assert.IsTrue(output.Contains(expectedOutput));
        }
    }
}