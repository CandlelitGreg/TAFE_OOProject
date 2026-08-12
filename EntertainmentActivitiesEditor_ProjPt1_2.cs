using System;
using System.IO;


public sealed class App
{
    static void Main()
    {
        Console.WriteLine("Welcome!");
        Console.WriteLine("Below are all of our current Entertainment Activities:");
        string[] entertainmentCSVLines = File.ReadAllLines("EntertainmentActivities.csv");
        for (int i = 1; i < entertainmentCSVLines.Length; i++)
        {
            string[] currentActivity = SeperateCSVLine(entertainmentCSVLines[i]);
            Console.WriteLine(currentActivity[1]);
        }
        bool addActivity = true;
        string[] newActivities = [];
        while(addActivity)
        {
            var result = GetClosedAnswer("Would you like to add another activity? (yes/no):");
            if (result.response == "yes")
            {
                string newActivity = GetActivityDetails();
                newActivities = newActivities.Append(newActivity).ToArray();
            } else {
                addActivity = false;
            }
        }
        Console.WriteLine($"{newActivities.Length} new entries are being added to the Entertainment Activies file!");
        string[] totalActivities = entertainmentCSVLines.Concat(newActivities).ToArray();
        WriteMultipleLines(totalActivities, "EntertainmentActivities.csv");
    }

    static string GetActivityDetails()
    {
        //All entries still have to be type checked for further use
        string name = AskForActivityInput(" name");
        string dateTime = AskForActivityInput(" date and start time");
        string cost = AskForActivityInput(" cost");
        string minParticipants = AskForActivityInput("'s minimum participants");
        string activity = $"{dateTime},{name},{cost},{minParticipants}";
        return activity;
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
    static void WriteMultipleLines(string[] lines, string filepath)
    {
        //This call will write each index element of the array into the file on their own line
        File.WriteAllLines(filepath, lines);
    }
}