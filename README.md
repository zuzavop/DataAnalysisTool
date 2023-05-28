# Data Analysis Tool

The Data Analysis Tool is a command-line program designed to help users analyze and explore datasets. It provides functionalities to import data from CSV and JSON files, perform data filtering and analysis, and export data to CSV and JSON formats.

## Getting Started
1. Download the Data Analysis Tool executable file from the provided source.
2. Build the application:
   * Open a terminal or command prompt.
   * Navigate to the directory containing the DataAnalysisTool project files.
   * Run the following command to build the application:
        ```
        dotnet build
        ```
    * Download needed python package:
        ```
        pip install -r requirements.txt
        ```

## Prerequisites
* Python
* .NET 6.0

## Usage
To run the Data Analysis Tool, open a command prompt or terminal and navigate to the directory where the project is located. After building whole project, use the following command:

```
dotnet run -- <input-file> [options]
```

Replace <input-file> with the path (full or relative) to your data file (CSV or JSON).

### Options
The Data Analysis Tool supports the following options:

* -s <seperator> or --seperator <seperator>: Specify the seperator in CSV input file. Default option is ','.


## Commands
Once the Data Analysis Tool is running, you can enter commands to perform various actions on the dataset.

### explore
This command provides overall statistics about the dataset or detailed statistics about a specific column. If you provide a column name, it will display statistics only for that column.
The statistics include the available column names, the number of objects in the dataset, and the number of unique values.

```
explore [column name]
```

### export
This command exports the dataset to a CSV or JSON file. Specify the file path where you want to save the dataset.

```
export [file path]
```

### show
This command displays all the data in the dataset.

```
show
```

### filter
This command filters the dataset by a specific column and value based on the provided condition. Use `=` for equal, `!=` for not equal, `<` for less than, `>` for greater than, `=>` for greater than or equal, `=<` for less than or equal, and `in` to check if the column value contains the specified value.

```
filter [column name] [=|!=|<|>|=>|=<|in] [value]
```

#### Example
Filter out values in column named `age` that are less or equal to 30:
```
filter age > 30
```

### clean
This command cleans the dataset by removing rows with missing values and normalizing columns with numerical values.

```
clean
```

### remove_duplicates
This command removes duplicates from the dataset.

```
remove_duplicates
```

### append
This command appends data from another file to the end of the dataset. It only includes columns that already exist in the dataset.

```
append [file path]
```

### statistic
This command shows statistics of a specific column. Specify the column name and choose the type of statistic you want to calculate (`mean`, `median`, `deviation`, `entropy`, `mode`, or `all`).

```
statistic [column name] [mean|median|deviation|entropy|mode|all]
```

### correlation
This command shows the Pearson correlation coefficient between two columns. Specify the names of the two columns.

```
correlation [column name] [column name]
```

### outliers
This command finds outliers in a column if the column contains numerical values.

```
outliers [column name]
```

### regression
This command performs a regression analysis on two columns. The data from the first column will be used as the x-coordinate, and the data from the second column will be used as the y-coordinate.

```
regression [column name] [column name]
```

### bar_plot
This command exports a bar plot created from selected columns in the input file data. Provide the output file path and the names of the two columns to plot.

```
bar_plot [output file path] [column name] [column name]
```

### line_plot
This command exports a line plot created from selected columns in the input file data. Provide the output file path and the names of the two columns to plot.

```
line_plot [output file path] [column name] [column name]
```

### scatter_plot
This command exports a scatter plot created from selected columns in the input file data. Provide the output file path and the names of the two columns to plot.

```
scatter_plot [output file path] [column name] [column name]
```

### histogram
This command exports a histogram created from selected columns in the input file data. Provide the output file path and the names of the columns to plot.

```
histogram [output file path] [column name] [column name]
```

### pie_plot
This command exports a pie plot created from selected column in the input file data. Provide the output file path and the names of the column to plot.

```
pie_plot [output file path] [column name]
```

### box_plot
This command exports a box plot created from selected columns in the input file data. Provide the output file path and the names of the two columns to plot.

```
box_plot [output file path] [column name] [column name]
```

### sort
This command sorts the dataset by a specific column.

```
sort [column name]
```

### help
This command prints help information for available commands.

```
help [command name]
```

### exit
The exit command terminates the Data Analysis Tool.

```
exit
```