# Data Analysis Tool

The Data Analysis Tool is a command-line program designed to help users analyze and explore datasets. It provides functionalities to import data from CSV and JSON files, perform data filtering and analysis, and export data to CSV and JSON formats.

## Installation
1. Download the Data Analysis Tool executable file from the provided source.
2. Save the executable file to a directory of your choice.

## Usage
To run the Data Analysis Tool, open a command prompt or terminal and navigate to the directory where the executable file is located. Then, use the following command:

```
DataAnalysisTool <input-file> [options]
```

Replace <input-file> with the path to your data file (CSV or JSON).

### Options
The Data Analysis Tool supports the following options:

* -o <output-file> or --output <output-file>: Specify the output file path for exporting data. If not provided, the default output file will be used based on the input file name.
* -h or --help: Display the help information and available options.

## Commands
Once the Data Analysis Tool is running, you can enter commands to perform various actions on the dataset.

### explore
The explore command displays the entire dataset.

```
> explore
```

### analyze <column>
The analyze command allows you to analyze a specific column in the dataset. Replace <column> with the name of the column you want to analyze.

```
> analyze Age
```

### exit
The exit command terminates the Data Analysis Tool.

```
> exit
```
## Examples
Here are some examples of how to use the Data Analysis Tool:

1. Import data from a CSV file:
```
DataAnalysisTool data.csv
```
2. Import data from a JSON file and specify the output file for exporting:
```
DataAnalysisTool data.json -o output.csv
```
3. Explore the imported dataset:
```
> explore
```
4. Analyze the "sales" column:
```
> analyze sales
```