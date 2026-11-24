using System;
using System.IO;

public sealed class Nada
{
    static void Main()
    {
        Console.WriteLine("Here is an example output using the System.IO namespace to access .csv files");
        Console.WriteLine("Below is an example of the entire file being read:");
        string[] entireFile = ReadFile("Activities.CSV");
        for (int i = 0; i < entireFile.Length; i++)
        {
            Console.WriteLine(entireFile[i]);
        }
        Console.WriteLine("Singular line example:");
        string secondLine = ReadOneLine("Activities.CSV", 1);
        Console.WriteLine(secondLine);
        Console.WriteLine("Below is an example of the same line seperated into its values:");
        string[] values = SeperateCSVLine(secondLine);
        for (int i = 0; i < values.Length; i++)
        {
            Console.WriteLine(values[i]);
        }
    }

    //Function showing how to read the entire file using System.IO
    static string[] ReadFile(string filepath) 
    {
        /*The function will put each line of the csv into a string array
        If you desire to edit the csv or read individual values you will
        have to seperate each line's string into another array of strings*/
        string[] lines = File.ReadAllLines(filepath);
        return lines;
    }

    //Function demonstrating how to read a single line from a csv using System.IO
    static string ReadOneLine(string filepath, int lineNum)
    {
        //The file is opened for reading
        using (StreamReader reader = new StreamReader(filepath))
        {
            int i = 0;

            //A line will be called with each reader.ReadLine call
            string line = reader.ReadLine();
            
            /* If you want to continue reading lines, you can loop 
            through and repeatedly run the call */
            while (i != lineNum)
            {
                line = reader.ReadLine();
                i++;
            }
            return line;
        }
    }

    /*Function demonstrating how each line can be 
    converted into its individual values. */
    static string[] SeperateCSVLine(string line)
    {
        //The string is split into an array via the coma values.
        /*NOTE: if one of the values has a coma in it this will 
        bug out due to too many columns/values. */
        string[] values = line.Split(",");
        return values;
    }

    //Function demonstrating writing a single line to an existing csv file
    static void WriteSingleLine(string line, string filepath)
    {
        //Opening the file for writing
        using (StreamWriter writer = new StreamWriter(filepath))
        {
            //Writing a single line with string input
            writer.WriteLine(line);

            /*This same function can be called repeatedly, like in a loop
            which can be used to loop and write each value of an array into a file*/
            writer.WriteLine(line);
            writer.WriteLine(line);
            writer.WriteLine(line);
        }
    }

    //Function demonstrates writing an array into a file
    static void WriteMultipleLines(string[] lines, string filepath)
    {
        //This call will write each index element of the array into the file on their own line
        File.WriteAllLines(filepath, lines);
    }
}