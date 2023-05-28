namespace DataGeneration
{
    public class CsvGenerator
    {
        private static readonly Random Random = new();

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

        private static string GetRandomName()
        {
            List<string> names = new()
            {
                "John", "Sarah", "Michael", "Emily", "David", "Emma", "Daniel", "Olivia",
                "Matthew", "Sophia", "Andrew", "Isabella", "John", "Lucia", "Kuba", "Marcus"
            };

            return names[Random.Next(names.Count)];
        }

        private static string GetRandomGender()
        {
            return Random.Next(2) == 0 ? "Male" : "Female";
        }

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
        public static void Main()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string? projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
            if (projectDirectory == null) projectDirectory = "";
            else projectDirectory += "\\..\\";

            string filePath = projectDirectory + "test.csv";
            int rowCount = 300;

            CsvGenerator.GenerateCsvFile(filePath, rowCount);
            Console.WriteLine($"CSV file '{filePath}' with {rowCount} rows generated successfully.");
        }
    }
}
