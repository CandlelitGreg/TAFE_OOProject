namespace TAFE_OOP_Project
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using Microsoft.VisualBasic;
    using System;
    using System.Data.Common;
    using System.Globalization;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class ActivityEditor
    {
        public class FitnessActivity
        {
            public string DateStartTime {get; set;}
            public string Title {get;set;}
            public int Cost {get;set;}
            public string Location {get;set;}
        }

        public class EntertainmentActivity
        {
            public string DateStartTime {get; set;}
            public string Title {get;set;}
            public int Cost {get;set;}
            public int MinParticipants {get;set;}
        }

        public class BlanketActivity
        {
            public string Type {get;set;}
            public string DateStartTime {get; set;}
            public string Title {get;set;}
            public int Cost {get;set;}
            public int MinParticipants {get;set;}
            public string Location {get;set;}
        }


        static void Main()
        {
            CheckOrUpdateDisplay("Welcome to the activities hub!", "What would you like to do today?");
        }

        static void CheckOrUpdateDisplay(string s1, string s2)
        {
            Console.WriteLine(s1);
            Console.WriteLine(s2);
            Console.WriteLine("1. View Current Fitness Activities");
            Console.WriteLine("2. Add New Fitness Activities");
            Console.WriteLine("3. Edit Current Fitness Activities");
            Console.WriteLine("4. View Current Entertainment Activities");
            Console.WriteLine("5. Add New Entertainment Activities");
            Console.WriteLine("6. Edit Current Entertainment Activities");
            Console.WriteLine("7. View ALL Activities");
            Console.WriteLine("8. Search for activity by date");
            Console.WriteLine("9. Quit Application");
            Console.WriteLine("Please select your desired action's number:");
            int input = GetMenuResponse(9);
            Console.WriteLine("\n\n");
            switch (input)
            {
                case 1:
                    ViewFitnessActivities();
                    goto default;
                case 2:
                    AddFitnessActivities();
                    goto default;
                case 3:
                    EditFitnessActivities();
                    Console.WriteLine("We unfortunately have not implimented this functionality yet.");
                    Console.WriteLine("Please check back with us later.");
                    goto default;
                case 4:
                    ViewEntertainmentActivities();
                    goto default;
                case 5:
                    AddEntertainmentActivities();
                    goto default;
                case 6:
                    EditEntertainmentActivities();
                    Console.WriteLine("We unfortunately have not implimented this functionality yet.");
                    Console.WriteLine("Please check back with us later.");
                    goto default;
                case 7:
                    ViewFitnessActivities();
                    Console.WriteLine("\n");
                    ViewEntertainmentActivities();
                    goto default;
                case 8:
                    bool searching = true;
                    while (searching)
                    {
                        SearchAllActivities();
                        Console.WriteLine("\n");
                        var response = GetClosedAnswer("Would you like to run another search? (yes/no)");
                        if (response == "no")
                        {
                            searching = false;
                        }
                    }
                    goto default;
                case 9:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Thank you for using our application today!");
                    Console.WriteLine("We hope to see you again soon...");
                    break;
                default:
                    Console.WriteLine("\n\n");
                    CheckOrUpdateDisplay("Sending you back to Activities Home...", "Is there anything else we can help you with today?");
                    break;
            }
        }

        static int GetMenuResponse(int optionQuant)
        {
            var input = ConvertStringToInt(Console.ReadLine());
            while(input.err != "" || input.num < 1 || optionQuant < input.num)
            {
                Console.WriteLine(input.err);
                if (input.err != "")
                {
                    input = ConvertStringToInt(ReAskInput(input.err, "Please select your desired action's number:"));
                } else
                {
                    input = ConvertStringToInt(ReAskInput($"{input.num} is not within the selectable number range.", "Please select your desired action's number:"));
                }
            }
            return input.num;
        }

        static void ViewFitnessActivities()
        {
            Console.WriteLine("Below are all of our current Fitness Activities:");
            // Change text color to blue
            Console.ForegroundColor = ConsoleColor.Blue;
            using var reader = new StreamReader("FitnessActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<FitnessActivity>().ToList();
            for (int i = 0; i < activitiesList.Count; i++)
            {
                Console.WriteLine($"{activitiesList[i].Title} at {activitiesList[i].Location}");
            }
            // Reset back to the user's default console colors
            Console.ResetColor();
        }

        static void AddFitnessActivities()
        {
            using var reader = new StreamReader("FitnessActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<FitnessActivity>().ToList();
            bool addActivity = true;
            List<FitnessActivity> newActivities = [];
            while(addActivity)
            {
                var result = GetClosedAnswer("Would you like to add another fitness activity? (yes/no):");
                if (result == "yes")
                {
                    FitnessActivity newActivity = GetFitnessActivityDetails();
                    newActivities.Add(newActivity);
                } else {
                    addActivity = false;
                }
            }
            Console.WriteLine($"{newActivities.Count} new entries are being added to the Fitness Activies file!");
            activitiesList.AddRange(newActivities);
            WriteMultipleLines(activitiesList, "FitnessActivities.csv");
        }

        static void EditFitnessActivities()
        {
            
        }

        static void ViewEntertainmentActivities()
        {
            Console.WriteLine("Below are all of our current Entertainment Activities:");
            // Change text color to blue
            Console.ForegroundColor = ConsoleColor.Cyan;
            using var reader = new StreamReader("EntertainmentActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<EntertainmentActivity>().ToList();
            for (int i = 0; i < activitiesList.Count; i++)
            {
                Console.WriteLine($"{activitiesList[i].Title}, which requires at least {activitiesList[i].MinParticipants} participants");
            }
            // Reset back to the user's default console colors
            Console.ResetColor();
        }

        static void AddEntertainmentActivities()
        {
            using var reader = new StreamReader("EntertainmentActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<EntertainmentActivity>().ToList();
            bool addActivity = true;
            List<EntertainmentActivity> newActivities = [];
            while(addActivity)
            {
                var result = GetClosedAnswer("Would you like to add another entertainment activity? (yes/no):");
                if (result == "yes")
                {
                    EntertainmentActivity newActivity = GetEntertainmentActivityDetails();
                    newActivities.Add(newActivity);
                } else {
                    addActivity = false;
                }
            }
            Console.WriteLine($"{newActivities.Count} new entries are being added to the Entertainment Activies file!");
            activitiesList.AddRange(newActivities);
            WriteMultipleLines(activitiesList, "EntertainmentActivities.csv");
        }

        static void EditEntertainmentActivities()
        {
            
        }

        static void SearchAllActivities()
        {
            using var ereader = new StreamReader("EntertainmentActivities.csv");
            using var ecsv = new CsvReader(ereader, CultureInfo.InvariantCulture);
            var eActivitiesList = ecsv.GetRecords<EntertainmentActivity>().ToList();
            using var freader = new StreamReader("FitnessActivities.csv");
            using var fcsv = new CsvReader(freader, CultureInfo.InvariantCulture);
            var fActivitiesList = fcsv.GetRecords<FitnessActivity>().ToList();
            List<BlanketActivity> activitiesList = [];
            for (int i = 0; i < eActivitiesList.Count; i++)
            {
                activitiesList.Add(UnifyActivityType(eActivitiesList[i]));
            }
            for (int i = 0; i < fActivitiesList.Count; i++)
            {
                activitiesList.Add(UnifyActivityType(fActivitiesList[i]));
            }
            List<BlanketActivity> orderedActivities = activitiesList.OrderByDescending(a => DateTime.Parse(a.DateStartTime)).ToList();
            Console.WriteLine("Please input a date for us to search from.");
            Console.WriteLine("Once a date is selected, you may view all activities on, before or after that date");
            var dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            while(dateParseResult.err != "")
            {
                Console.WriteLine(dateParseResult.err);
                dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            }
            DateOnly searchDate = DateOnly.FromDateTime(dateParseResult.time);
            Console.WriteLine("Please select how you want to filter your search:");
            Console.WriteLine($"1. All Activities BEFORE {searchDate}");
            Console.WriteLine($"2. All Activities ON {searchDate}");
            Console.WriteLine($"3. All Activities AFTER {searchDate}");
            int input = GetMenuResponse(3);
            Console.WriteLine("\n");
            switch (input)
            {
                case 1:
                    Console.WriteLine($"Displaying All activities before {searchDate}");
                    int start = 0;
                    while (start < orderedActivities.Count && DateOnly.FromDateTime(DateTime.Parse(orderedActivities[start].DateStartTime)) >= searchDate)
                    {
                        start++;
                    }
                    for (int i = start; i < orderedActivities.Count; i++)
                    {
                        Console.WriteLine($"{orderedActivities[i].DateStartTime} - {orderedActivities[i].Title} - {orderedActivities[i].Type}");
                    }
                    break;
                case 2:
                    Console.WriteLine($"Displaying All activities on {searchDate}");
                    start = 0;
                    while (start < orderedActivities.Count && DateOnly.FromDateTime(DateTime.Parse(orderedActivities[start].DateStartTime)) != searchDate)
                    {
                        start++;
                    }
                    while (start < orderedActivities.Count && DateOnly.FromDateTime(DateTime.Parse(orderedActivities[start].DateStartTime)) == searchDate)
                    {
                        Console.WriteLine($"{orderedActivities[start].DateStartTime} - {orderedActivities[start].Title} - {orderedActivities[start].Type}");
                        start++;
                    }
                    break;
                case 3:
                    Console.WriteLine($"Displaying All activities after {searchDate}");
                    start = 0;
                    while (start < orderedActivities.Count && DateOnly.FromDateTime(DateTime.Parse(orderedActivities[start].DateStartTime)) > searchDate)
                    {
                        Console.WriteLine($"{orderedActivities[start].DateStartTime} - {orderedActivities[start].Title} - {orderedActivities[start].Type}");
                        start++;
                    }
                    break;
            }
        }

        static BlanketActivity UnifyActivityType(FitnessActivity activity)
        {
            BlanketActivity newActivity = new BlanketActivity();
            newActivity.Title = activity.Title;
            newActivity.DateStartTime = activity.DateStartTime;
            newActivity.Cost = activity.Cost;
            newActivity.Location = activity.Location;
            newActivity.Type = "Fitness";
            return newActivity;
        }

        static BlanketActivity UnifyActivityType(EntertainmentActivity activity)
        {
            BlanketActivity newActivity = new BlanketActivity();
            newActivity.Title = activity.Title;
            newActivity.DateStartTime = activity.DateStartTime;
            newActivity.Cost = activity.Cost;
            newActivity.MinParticipants = activity.MinParticipants;
            newActivity.Type = "Entertainment";
            return newActivity;
        }

        static FitnessActivity GetFitnessActivityDetails()
        {
            FitnessActivity newActivity = new FitnessActivity();
            //All entries still have to be type checked for further use
            newActivity.Title = ToTitleCase(AskForActivityInput(" name"));
            var dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            while(dateParseResult.err != "")
            {
                Console.WriteLine(dateParseResult.err);
                dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            }
            var timeParseResult = ConvertStringToTime(AskForActivityInput(" starting time in format HH:mm"), "HH:mm");
            while(timeParseResult.err != "")
            {
                Console.WriteLine(timeParseResult.err);
                timeParseResult = ConvertStringToTime(AskForActivityInput(" starting time in format HH:mm"), "HH:mm");
            }
            newActivity.DateStartTime = $"{dateParseResult.time:dd/MM/yyyy} {timeParseResult.time:HH:mm}";
            var parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            while(parseResult.err != "")
            {
                Console.WriteLine(parseResult.err);
                parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            }
            newActivity.Cost = parseResult.num;
            newActivity.Location = ToTitleCase(AskForActivityInput("'s location"));
            return newActivity;
        }

        static EntertainmentActivity GetEntertainmentActivityDetails()
        {
            EntertainmentActivity newActivity = new EntertainmentActivity();
            //All entries still have to be type checked for further use
            newActivity.Title = ToTitleCase(AskForActivityInput(" name"));
            var dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            while(dateParseResult.err != "")
            {
                Console.WriteLine(dateParseResult.err);
                dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            }
            var timeParseResult = ConvertStringToTime(AskForActivityInput(" starting time in format HH:mm"), "HH:mm");
            while(timeParseResult.err != "")
            {
                Console.WriteLine(timeParseResult.err);
                timeParseResult = ConvertStringToTime(AskForActivityInput(" starting time in format HH:mm"), "HH:mm");
            }
            newActivity.DateStartTime = $"{dateParseResult.time:dd/MM/yyyy} {timeParseResult.time:HH:mm}";
            var parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            while(parseResult.err != "")
            {
                Console.WriteLine(parseResult.err);
                parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            }
            newActivity.Cost = parseResult.num;
            parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
            while(parseResult.err != "")
            {
                Console.WriteLine(parseResult.err);
                parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
            }
            newActivity.MinParticipants = parseResult.num;
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

        static string GetClosedAnswer(string question)
        {
            Console.Write(question);
            string response = Console.ReadLine().Trim().ToLower();
            //If answer is yes or no, return value
            if (response.ToLower() == "yes" || response.ToLower() == "y")
            {
                return "yes";
            }
            if (response.ToLower() == "no" || response.ToLower() == "n")
            {
                return "no";
            }
            //If answer is not yes or no, notify user and request new input
            Console.WriteLine($"{response} is not a valid input.");
            return GetClosedAnswer(question);
        }
        static string[] SeperateCSVLine(string line)
        {
            //The string is split into an array via the coma values.
            /*NOTE: if one of the values has a coma in it this will 
            bug out due to too many columns/values. */
            string[] values = line.Split(",");
            return values;
        }
        static void WriteMultipleLines(List<FitnessActivity> lines, string filepath)
        {
            using var writer = new StreamWriter(filepath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(lines);
        }

        static void WriteMultipleLines(List<EntertainmentActivity> lines, string filepath)
        {
            using var writer = new StreamWriter(filepath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(lines);
        }

        static (int num, string err) ConvertStringToInt(string input)
        {
            if (int.TryParse(input, out int num))
            {
                return (num, "");
            }
            //Returning an error message to say the input was not covertible into an Integer type
            return (-1, $"Type conversion failed, {input} is not a type: integer");
        }

        static (DateTime time, string err) ConvertStringToTime(string input, string format)
        {
            if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime time))
            {
                return (time, "");
            }
            //Returning an error message to say the input was not covertible into an Integer type
            return (DateTime.Now, $"Type conversion failed, {input} is not a type: DateTime in format {format}");
        }

        //Takes a string input and returns the same string formatted to Title Case formatting
        static string ToTitleCase(string input)
        {
            //Turns the entire input string to lower case
            input = input.ToLower();
            //Seperates the input string into an array of characters
            char[] returnString = input.ToCharArray();
            //Creates a bool to destinguish if the next letter should be a capital
            bool capsNext = true;
            /*Loops through each character turning them capital if the boolean requests it
             - Boolean will automatically request the primary letter to be capital -
             - If there are any spaces in the string, the next letter will automatically be converted into a captial - */
            for (int i = 0; i < returnString.Length; i++)
            {
                //If the boolean is true, the character will be converted into a capital
                if (capsNext) 
                {
                    returnString[i] = char.ToUpper(returnString[i]);
                    //The boolean will automatically be converted to false
                    capsNext = false;
                }
                //If there is a space in the string, the boolean is set to true to signify that the next character should be a capital
                if (input[i] == ' ')
                {
                    capsNext = true;
                }

            }
            //returns the character array reformatted into a string
            return string.Concat(returnString);
        }
    }

}