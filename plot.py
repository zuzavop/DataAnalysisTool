import matplotlib.pyplot as plt
import pandas as pd
import sys

def generate_line_plot(column_names, output_file, df):
    df.plot.line(x=column_names[0], y=column_names[1])

    plt.savefig(output_file)


def generate_bar_plot(column_names, output_file, df):
    df.plot.bar(x=column_names[0], y=column_names[1])

    plt.savefig(output_file)


def generate_scatter_plot(column_names, output_file, df):
    df.plot.scatter(x=column_names[0], y=column_names[1])

    plt.savefig(output_file)


def generate_pie_plot(column_names, output_file, df):
    df.plot.pie(y=column_names[0])

    plt.savefig(output_file)


def generate_histogram(column_names, output_file, df):
    df.plot.hist(column=[column_names[0]], by=column_names[1])

    plt.savefig(output_file)


def generate_box_plot(column_names, output_file, df):
    df.plot.box(column=column_names[0], by=column_names[1])

    plt.savefig(output_file)


if __name__ == '__main__':
    # Read arguments from command line
    column_names = sys.argv[4:-1]
    type_name = sys.argv[1]
    input_file = sys.argv[2]
    seperator = sys.argv[3]
    output_file = sys.argv[-1]

    # Read the data from the CSV file
    df = pd.read_csv(input_file, sep=seperator)

    if type_name == "line_plot":
        generate_line_plot(column_names, output_file, df)
    elif type_name == "bar_plot":
        generate_bar_plot(column_names, output_file, df)
    elif type_name == "scatter_plot":
        generate_scatter_plot(column_names, output_file, df)
    elif type_name == "pie_plot":
        generate_pir_plot(column_names, output_file, df)
    elif type_name == "histogram":
        generate_histogram(column_names, output_file, df)
    elif type_name == "box_plot":
        generate_pir_plot(column_names, output_file, df)
