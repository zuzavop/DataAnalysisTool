# Data Analysis Tool
The Data Analysis Tool is implemented in C# using the .NET Core framework. This section provides an overview of the code structure and important classes to help developers understand and extend the functionality of the tool.

## Code structure
The code for the Data Analysis Tool is organized into several classes and namespaces:


## Classes
* `Dataset`: Represents a collection of data objects with column-value pairs.
* `DataObject`: Represents a single data object with column-value pairs.
* `DataImporter`: Provides functionalities to import data from CSV and JSON files into a DataSet.
* `DataExporter`: Provides functionalities to export data from a `Dataset` to CSV and JSON files.
* `DataProcessor`: Performs various data processing operations on a `Dataset`.
* `DatasetExplorer`: Allows users to explore and analyze a `Dataset`.
* `DataVisualizer`: Allows users to visualize a `Dataset`.
* `DataAnalyzer`: Processes all user requirements for analysis.