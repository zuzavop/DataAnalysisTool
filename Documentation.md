# Data Analysis Tool
The Data Analysis Tool is implemented in C# using the .NET Core framework with using Python for creation of plots. This section provides an overview of the code structure and important classes to help developers understand and extend the functionality of the tool. More documentation is in code itself.

## Classes
* `Dataset`: Represents a dataset.
* `DataObject`: Represents a single data object with column-value pairs.
* `DataImporter`: Provides functionalities to import data from CSV and JSON files into a `Dataset`.
* `DataExporter`: Provides functionalities to export data from a `Dataset` to CSV and JSON files.
* `DataProcessor`: Performs various data processing operations (statistic calculation) on a `Dataset`.
* `DatasetExplorer`: Allows users to explore and analyze a `Dataset`.
* `DataVisualizer`: Allows users to visualize a `Dataset` and mediating python script calls for creation of plots.
* `DataAnalyzer`: Processes all user requirements for analysis.