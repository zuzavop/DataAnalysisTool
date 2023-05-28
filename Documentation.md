# Data Analysis Tool
The Data Analysis Tool is implemented in C# using the .NET Core framework with using Python for creation of plots. This section provides an overview of the code structure and important classes to help developers understand and extend the functionality of the tool. More documentation is in code itself.

## Key Functionalities
The Data Analysis Tool supports a wide range of functionalities for data analysis. Here are some of the key operations it offers:

* Exploring dataset statistics.
* Exporting the dataset to various file formats.
* Displaying the dataset in the console.
* Filtering the dataset based on column values.
* Cleaning and preprocessing the dataset.
* Removing duplicates from the dataset.
* Appending data from another file to the dataset.
* Calculating various statistical measures for columns.
* Finding outliers in numerical columns.
* Performing regression analysis between two columns.
* Creating and saving different types of plots.

## Classes
* `Dataset`: Represents a dataset.
* `DataObject`: Represents a single data object with column-value pairs.
* `DataImporter`: Provides functionalities to import data from CSV and JSON files into a `Dataset`.
* `DataExporter`: Provides functionalities to export data from a `Dataset` to CSV and JSON files.
* `DataProcessor`: Performs various data processing operations (statistic calculation) on a `Dataset`.
* `DatasetExplorer`: Allows users to explore and analyze a `Dataset`.
* `DataVisualizer`: Allows users to visualize a `Dataset` and mediating python script calls for creation of plots.
* `DataAnalyzer`: Processes all user requirements for analysis.

### Responsibilities
#### Main Application (`DataAnalyzer`)
The Main Application is the entry point of the Data Analysis Tool. It handles user input, command execution, and provides an interactive command-line interface (CLI) for users to interact with the tool. Load data from input file using `DataImporter`.

The key responsibilities of the Main Application include:
* Parsing user commands and parameters.
* Validating command syntax and parameters.
* Executing the corresponding command handler module based on the user command.
* Providing error handling and feedback to the user.

#### `DataImporter`
The `DataImporter` is responsible for importing the dataset from the input file. It provides functionality to load the dataset from file of CSV or JSON format.

The key responsibilities of the `DataImporter` module include:
* Validating the file path and format of the input file.
* Converting the data from the input file to the desired format.

#### `DataExporter`
The `DataExporter` is responsible for exporting the dataset to a file. It provides functionality to save the dataset in various file formats, such as CSV, or JSON.

The key responsibilities of the `DataExporter` module include:
* Validating the file path and format for exporting.
* Converting the dataset to the desired file format.
* Writing the dataset to the specified file.

#### `DataProcessor`
The `DataProcessor` handles data manipulation and analysis operations. It provides a set of functions to filter, clean, preprocess, calculate statistics, find outliers, perform regression analysis, and more.

The key responsibilities of the `DataProcessor` include:
* Applying various data manipulation operations based on user commands.
* Modifying the dataset based on the requested operations.

#### `DataVisualizer`
The `DataVisualizer` handles data visualization and plotting operations. It utilizes data visualization library Matplotlib to generate visual representations of the dataset.

The key responsibilities of the `DataVisualizer` include:
* Creating different types of plots, including bar plots, line plots, scatter plots, histograms, box plots, and pie charts.
* Saving the generated plots to png files.

#### `DatasetExplorer`
The `DatasetExplorer` provides functionality to explore dataset. It offers operations to retrieve column names, data types, statistical information.

The key responsibilities of the `DatasetExplorer` include:
* Extracting metadata information, such as column names, data types, and statistical measures.