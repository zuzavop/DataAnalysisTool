import matplotlib.pyplot as plt
import pandas as pd
import sys

def generate_line_plot(column_names, input_file, output_file):
    # Read the data from the CSV file
    df = pd.read_csv(input_file)

    # Get the selected columns
    selected_columns = df[column_names]

    # Create a new figure and plot the lines
    plt.figure()
    for column in selected_columns:
        plt.plot(selected_columns.index, column)

    # Save the plot to a PNG file
    plt.savefig(output_file)

if __name__ == '__main__':
    # Read arguments from command line
    column_names = sys.argv[2:-1]
    input_file = sys.argv[1]
    output_file = sys.argv[-1]

    # Generate the line plot
    generate_line_plot(column_names, input_file, output_file)
