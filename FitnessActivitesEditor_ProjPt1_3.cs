namespace FitnessActivitiesEditor
{

using System;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;

public class FitnessActivity
{
    public string DateStartTime {get; set;}
    public string Title {get;set;}
    public int Cost {get;set;}
    public string Location {get;set;}
}

public sealed class App2
{
    static void Main()
    {
        Console.WriteLine("Welcome!");
        Console.WriteLine("Below are all of our current Fitness Activities:");
        using var reader = new StreamReader("FitnessActivities.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var activitiesList = csv.GetRecords<FitnessActivity>().ToList();
        for (int i = 1; i < activitiesList.Count; i++)
        {
            Console.WriteLine($"{activitiesList[i].Title} at {activitiesList[i].Location}");
        }
        bool addActivity = true;
        List<FitnessActivity> newActivities = [];
        while(addActivity)
        {
            var result = GetClosedAnswer("Would you like to add another activity? (yes/no):");
            if (result.response == "yes")
            {
                FitnessActivity newActivity = GetActivityDetails();
                newActivities.Add(newActivity);
            } else {
                addActivity = false;
            }
        }
        Console.WriteLine($"{newActivities.Count} new entries are being added to the Fitness Activies file!");
        activitiesList.AddRange(newActivities);
        WriteMultipleLines(activitiesList, "FitnessActivities.csv");
    }

    static FitnessActivity GetActivityDetails()
    {
        FitnessActivity newActivity = new FitnessActivity();
        //All entries still have to be type checked for further use
        newActivity.Title = AskForActivityInput(" name");
        newActivity.DateStartTime = AskForActivityInput(" date and start DateStartTime");
        var parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
        while(parseResult.err != "")
        {
            Console.WriteLine(parseResult.err);
            parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
        }
        newActivity.Cost = parseResult.cost;
        newActivity.Location = AskForActivityInput("'s location");
        return newActivity;
    }

    static string AskForActivityInput(string header)
    {
        Console.WriteLine($"Please enter Activity{header}:");
        string detail = Console.ReadLine();
        bool containsComma = CheckForComma(detail);
        while (containsComma)
        {
            detail = ReAskInput("Sorry, input cannot contain a ',' (comma value).", $"Please input valid Activity{header}:");
            containsComma = CheckForComma(detail);
        }
        return detail;
    }

    static string ReAskInput(string q1, string q2)
    {
        Console.WriteLine(q1);
        Console.WriteLine(q2);
        string answer = Console.ReadLine();
        return answer;
    }

    static bool CheckForComma(string input)
    {
        bool containsComma = false;
        string[] brokenInput = SeperateCSVLine(input);
        if (brokenInput.Length > 1)
        {
            containsComma = true;
        }
        return containsComma;
    }

    static (string response, bool valid) GetClosedAnswer(string question)
    {
        Console.Write(question);
        string response = Console.ReadLine().Trim().ToLower();
        //If answer is yes or no, return value
        if (response.ToLower() == "yes" || response.ToLower() == "y")
        {
            return ("yes", true);
        }
        if (response.ToLower() == "no" || response.ToLower() == "n")
        {
            return ("no", true);
        }
        //If answer is not yes or no, notify user and request new input
        Console.WriteLine($"{response} is not a valid input.");
        return GetClosedAnswer(question);
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

    //Function demonstrates writing an array into a file
    static void WriteMultipleLines(List<FitnessActivity> lines, string filepath)
    {
        using var writer = new StreamWriter("FitnessActivities.csv");
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(lines);
    }

    /*Completes a Parse check to ensure that the inputted value can be converted into a Integer type
    Returns two values, the converted number, plus an error message
    If the error value is the string's null type, there is no error and it returns the conversion*/
    static (int cost, string err) ConvertStringToInt(string input)
    {
        if (int.TryParse(input, out int num))
        {
            return (num, "");
        }
        //Returning an error message to say the input was not covertible into an Integer type
        return (0, $"Type conversion failed, {input} is not a type: integer");
    }
}
}