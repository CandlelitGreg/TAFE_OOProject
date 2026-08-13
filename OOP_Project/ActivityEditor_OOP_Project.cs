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
            var input = ConvertStringToInt(Console.ReadLine());
            while(input.err != "" || input.num < 1 || 9 < input.num)
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
            Console.WriteLine("\n\n");
            switch (input.num)
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
                    SearchAllActivities();
                    Console.WriteLine("We unfortunately have not implimented this functionality yet.");
                    Console.WriteLine("Please check back with us later.");
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
                if (result.response == "yes")
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
                if (result.response == "yes")
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