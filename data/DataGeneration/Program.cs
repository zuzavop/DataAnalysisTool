namespace DataGeneration
{
    /// <summary>
    /// Represents a CSV generator for generating CSV files with random data.
    /// </summary>
    public class CsvGenerator
    {
        private static readonly Random Random = new();

        /// <summary>
        /// Generates a CSV file with the specified number of rows.
        /// </summary>
        /// <param name="filePath">The path to the output CSV file.</param>
        /// <param name="rowCount">The number of rows to generate.</param>
        public static void GenerateCsvFile(string filePath, int rowCount)
        {
            List<string> lines = new()
            {
                "Name,Age,Gender,Country,Salary"
            };

            for (int i = 0; i < rowCount; i++)
            {
                string name = GetRandomName();
                int age = Random.Next(18, 60);
                string gender = GetRandomGender();
                string country = GetRandomCountry();
                int salary = Random.Next(20000, 100000);

                string line = $"{name},{age},{gender},{country},{salary}";
                lines.Add(line);
            }

            File.WriteAllLines(filePath, lines);
        }

        /// <summary>
        /// Gets a random name from a predefined list of names.
        /// </summary>
        /// <returns>A random name.</returns>
        private static string GetRandomName()
        {
            List<string> names = new()
            {
                "John", "Sarah", "Michael", "Emily", "David", "Emma", "Daniel", "Olivia",
                "Matthew", "Sophia", "Andrew", "Isabella", "John", "Lucia", "Kuba", "Marcus"
            };

            return names[Random.Next(names.Count)];
        }

        /// <summary>
        /// Gets a random gender ("Male" or "Female").
        /// </summary>
        /// <returns>A random gender.</returns>
        private static string GetRandomGender()
        {
            return Random.Next(2) == 0 ? "Male" : "Female";
        }

        /// <summary>
        /// Gets a random country from a predefined list of countries.
        /// </summary>
        /// <returns>A random country.</returns>
        private static string GetRandomCountry()
        {
            List<string> countries = new()
            {
                "USA", "Canada", "UK", "Australia", "Czechia", "France", "Spain", "Slovakia"
            };

            return countries[Random.Next(countries.Count)];
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string? projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
            if (projectDirectory == null) projectDirectory = "";
            else projectDirectory += "\\..\\";

            string filePath = args.Length > 0 ? args[0] : projectDirectory + "test.csv";

            int rowCount = 100;
            if (args.Length > 2 && int.TryParse(args[1], out int count))
            {
                rowCount = count;
            }

            CsvGenerator.GenerateCsvFile(filePath, rowCount);
            Console.WriteLine($"CSV file '{filePath}' with {rowCount} rows generated successfully.");
        }
    }
}
