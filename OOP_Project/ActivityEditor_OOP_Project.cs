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
            public int? MinParticipants {get;set;}
            public string? Location {get;set;}
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
                    bool editing = true;
                    while (editing)
                    {
                        EditFitnessActivities();
                        Console.WriteLine("\n");
                        var response = GetClosedAnswer("Would you like to edit more Fitness Activities? (yes/no)");
                        if (response == "no")
                        {
                            editing = false;
                        }
                    }
                    goto default;
                case 4:
                    ViewEntertainmentActivities();
                    goto default;
                case 5:
                    AddEntertainmentActivities();
                    goto default;
                case 6:
                    editing = true;
                    while (editing)
                    {
                        EditEntertainmentActivities();
                        Console.WriteLine("\n");
                        var response = GetClosedAnswer("Would you like to edit more Entertainment Activities? (yes/no)");
                        if (response == "no")
                        {
                            editing = false;
                        }
                    }
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

        static int[] GetManyMenuResponses(int optionQuant)
        {
            string rawInput = Console.ReadLine();
            string[] indexStrings = SeperateCSVLine(rawInput);
            while (indexStrings.Length > optionQuant)
            {
                Console.WriteLine($"{rawInput} contains more than {optionQuant} indexes. Please input fewer options:");
                rawInput = Console.ReadLine();
                indexStrings = SeperateCSVLine(rawInput);
            }
            int[] inputInts = [];
            int[] receivedInputs = [];
            for (int i = 0; i < indexStrings.Length; i++)
            {
                var input = ConvertStringToInt(indexStrings[i]);
                while(input.err != "" || input.num < 1 || optionQuant < input.num || receivedInputs.Contains(input.num))
                {
                    Console.WriteLine(input.err);
                    if (input.err != "")
                    {
                        input = ConvertStringToInt(ReAskInput(input.err, "Please select your desired action's number:"));
                    } else if (receivedInputs.Contains(input.num))
                    {
                        input = ConvertStringToInt(ReAskInput($"{input.num} has already been selected.", "Please select your desired action's number:"));
                    }
                    else
                    {
                        input = ConvertStringToInt(ReAskInput($"{input.num} is not within the selectable number range.", "Please select your desired action's number:"));
                    }
                }
                inputInts = inputInts.Append(input.num).ToArray();
                receivedInputs = receivedInputs.Append(input.num).ToArray();
            }
            return inputInts;
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
            Console.WriteLine("Please find all activities below:");
            using var reader = new StreamReader("FitnessActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<FitnessActivity>().ToList();
            //Write all the activities with a number next to them
            Console.WriteLine("Index. DateStartTime - Title - Cost - Location");
            for (int i = 0; i < activitiesList.Count; i++)
            {
                Console.WriteLine($"{i+1}. {activitiesList[i].DateStartTime} - {activitiesList[i].Title} - {activitiesList[i].Cost} - {activitiesList[i].Location}");
            }

            //Ask user to select number of activity they would like to change
            Console.WriteLine("Please input number for activity you would like to edit:");
            int activityIndex = GetMenuResponse(activitiesList.Count) - 1;
            FitnessActivity selectedActivity = new FitnessActivity() {
                DateStartTime = activitiesList[activityIndex].DateStartTime,
                Title = activitiesList[activityIndex].Title,
                Cost = activitiesList[activityIndex].Cost,
                Location = activitiesList[activityIndex].Location
            };

            //List details of activity with a number next to them
            Console.WriteLine($"1. DateStartTime: {selectedActivity.DateStartTime}");
            Console.WriteLine($"2. Title: {selectedActivity.Title}");
            Console.WriteLine($"3. Cost: {selectedActivity.Cost}");
            Console.WriteLine($"4. Location: {selectedActivity.Location}");
            Console.WriteLine("Please select index of detail/s you would like to change");

            //Ask user to select number of details they would like to change, with comma between
            Console.WriteLine("If you would like to change more than one index, input all indexes seperated by commas");
            int[] changingIndex = GetManyMenuResponses(4);

            //Run loop going through each selected number's detail showing previous state and requesting what to update to
            //As response is received, push new detail to fresh object
            //Re ask for input if it does not match required dataType
            selectedActivity = GetActivityDetails(activitiesList[activityIndex], selectedActivity, changingIndex).activity;

            //Make changes in .csv
            activitiesList[activityIndex] = selectedActivity;
            Console.WriteLine("Changes have been saved to file!");
            WriteMultipleLines(activitiesList, "FitnessActivities.csv");
        }

        static (FitnessActivity activity, string makeChanges) GetActivityDetails(FitnessActivity oldFitnessActivity, FitnessActivity newFitnessActivity, int[] detailsToChange)
        {
            for (int i = 0; i < detailsToChange.Length; i++)
            {
                switch (detailsToChange[i])
                {
                    case 1:
                        Console.Write($"Current DateStartTime is ");
                        WriteColour(newFitnessActivity.DateStartTime, ConsoleColor.Green);
                        Console.WriteLine("");
                        newFitnessActivity.DateStartTime = GetActivityDateTime();
                        break;
                    case 2:
                        Console.Write($"Current Title is ");
                        WriteColour(newFitnessActivity.Title, ConsoleColor.Green);
                        Console.WriteLine("");
                        newFitnessActivity.Title = AskForActivityInput(" title");
                        break;
                    case 3:
                        Console.Write($"Current Cost is ");
                        WriteColour($"{newFitnessActivity.Cost}", ConsoleColor.Green);
                        Console.WriteLine("");
                        newFitnessActivity.Cost = GetActivityCost();
                        break;
                    case 4:
                        Console.Write($"Current Location is ");
                        WriteColour(newFitnessActivity.Location, ConsoleColor.Green);
                        Console.WriteLine("");
                        newFitnessActivity.Location = AskForActivityInput(" location");
                        break;
                }
            }

            //Reprint activity with updated details
            Console.WriteLine("Please confirm activity's new details:");
            Console.WriteLine($"Original Details: {oldFitnessActivity.DateStartTime} - {oldFitnessActivity.Title} - {oldFitnessActivity.Cost} - {oldFitnessActivity.Location}");
            Console.WriteLine("New Details: ");
            if (oldFitnessActivity.DateStartTime != newFitnessActivity.DateStartTime)
            {
                WriteColour(newFitnessActivity.DateStartTime, ConsoleColor.Blue);
            } 
            else
            {
                Console.Write($"{newFitnessActivity.DateStartTime}");
            }
            Console.Write(" - ");
            if (oldFitnessActivity.Title != newFitnessActivity.Title)
            {
                WriteColour(newFitnessActivity.Title, ConsoleColor.Blue);
            }
            else
            {
                Console.Write($"{newFitnessActivity.Title}");
            }
            Console.Write(" - ");
            if (oldFitnessActivity.Cost != newFitnessActivity.Cost)
            {
                WriteColour($"{newFitnessActivity.Cost}", ConsoleColor.Blue);
            }
            else
            {
                Console.Write($"{newFitnessActivity.Cost}");
            }
            Console.Write(" - ");
            if (oldFitnessActivity.Location != newFitnessActivity.Location)
            {
                WriteColour(newFitnessActivity.Location, ConsoleColor.Blue);
            }
            else
            {
                Console.Write(newFitnessActivity.Location);
            }
            Console.WriteLine("");

            //Prompt user if happy with changes
            string makeChanges = "no";
            string happy = GetClosedAnswer("Are you happy with you changes? (yes/no)");
            if (happy == "no")
            {
                makeChanges = GetClosedAnswer("Would you like to ammend your changes? (yes/no)");
                while (makeChanges != "yes")
                {
                    var result = GetActivityDetails(oldFitnessActivity, newFitnessActivity, detailsToChange);
                    makeChanges = result.makeChanges;
                    newFitnessActivity = result.activity;
                }
            }
            return (newFitnessActivity, makeChanges);
        }

        static (EntertainmentActivity activity, string makeChanges) GetActivityDetails(EntertainmentActivity oldActivity, EntertainmentActivity newActivity, int[] detailsToChange)
        {
            for (int i = 0; i < detailsToChange.Length; i++)
            {
                switch (detailsToChange[i])
                {
                    case 1:
                        Console.Write($"Current DateStartTime is ");
                        WriteColour(newActivity.DateStartTime, ConsoleColor.Green);
                        Console.WriteLine("");
                        newActivity.DateStartTime = GetActivityDateTime();
                        break;
                    case 2:
                        Console.Write($"Current Title is ");
                        WriteColour(newActivity.Title, ConsoleColor.Green);
                        Console.WriteLine("");
                        newActivity.Title = AskForActivityInput(" title");
                        break;
                    case 3:
                        Console.Write($"Current Cost is ");
                        WriteColour($"{newActivity.Cost}", ConsoleColor.Green);
                        Console.WriteLine("");
                        newActivity.Cost = GetActivityCost();
                        break;
                    case 4:
                        Console.Write($"Current Location is ");
                        WriteColour($"{newActivity.MinParticipants}", ConsoleColor.Green);
                        Console.WriteLine("");
                        var parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
                        while(parseResult.err != "")
                        {
                            Console.WriteLine(parseResult.err);
                            parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
                        }
                        newActivity.MinParticipants = parseResult.num;
                        break;
                }
            }

            //Reprint activity with updated details
            Console.WriteLine("Please confirm activity's new details:");
            Console.WriteLine($"Original Details: {oldActivity.DateStartTime} - {oldActivity.Title} - {oldActivity.Cost} - {oldActivity.MinParticipants}");
            Console.WriteLine("New Details: ");
            if (oldActivity.DateStartTime != newActivity.DateStartTime)
            {
                WriteColour(newActivity.DateStartTime, ConsoleColor.Blue);
            } 
            else
            {
                Console.Write($"{newActivity.DateStartTime}");
            }
            Console.Write(" - ");
            if (oldActivity.Title != newActivity.Title)
            {
                WriteColour(newActivity.Title, ConsoleColor.Blue);
            }
            else
            {
                Console.Write($"{newActivity.Title}");
            }
            Console.Write(" - ");
            if (oldActivity.Cost != newActivity.Cost)
            {
                WriteColour($"{newActivity.Cost}", ConsoleColor.Blue);
            }
            else
            {
                Console.Write($"{newActivity.Cost}");
            }
            Console.Write(" - ");
            if (oldActivity.MinParticipants != newActivity.MinParticipants)
            {
                WriteColour($"{newActivity.MinParticipants}", ConsoleColor.Blue);
            }
            else
            {
                Console.Write($"{newActivity.MinParticipants}");
            }
            Console.WriteLine("");

            //Prompt user if happy with changes
            string happy = GetClosedAnswer("Are you happy with you changes? (yes/no)");
            string makeChanges = "no";
            if (happy == "no")
            {
                makeChanges = GetClosedAnswer("Would you like to ammend your changes? (yes/no)");
                while (makeChanges != "yes")
                {
                    var result = GetActivityDetails(oldActivity, newActivity, detailsToChange);
                    makeChanges = result.makeChanges;
                    newActivity = result.activity;
                }
            }
            return (newActivity, makeChanges);
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
            Console.WriteLine("Please find all activities below:");
            using var reader = new StreamReader("EntertainmentActivities.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var activitiesList = csv.GetRecords<EntertainmentActivity>().ToList();
            //Write all the activities with a number next to them
            Console.WriteLine("Index. DateStartTime - Title - Cost - MinParticipants");
            for (int i = 0; i < activitiesList.Count; i++)
            {
                Console.WriteLine($"{i+1}. {activitiesList[i].DateStartTime} - {activitiesList[i].Title} - {activitiesList[i].Cost} - {activitiesList[i].MinParticipants}");
            }

            //Ask user to select number of activity they would like to change
            Console.WriteLine("Please input number for activity you would like to edit:");
            int activityIndex = GetMenuResponse(activitiesList.Count) - 1;
            EntertainmentActivity selectedActivity = new EntertainmentActivity() {
                DateStartTime = activitiesList[activityIndex].DateStartTime,
                Title = activitiesList[activityIndex].Title,
                Cost = activitiesList[activityIndex].Cost,
                MinParticipants = activitiesList[activityIndex].MinParticipants
            };

            //List details of activity with a number next to them
            Console.WriteLine($"1. DateStartTime: {selectedActivity.DateStartTime}");
            Console.WriteLine($"2. Title: {selectedActivity.Title}");
            Console.WriteLine($"3. Cost: {selectedActivity.Cost}");
            Console.WriteLine($"4. Minimum Participants: {selectedActivity.MinParticipants}");
            Console.WriteLine("Please select index of detail/s you would like to change");

            //Ask user to select number of details they would like to change, with comma between
            Console.WriteLine("If you would like to change more than one index, input all indexes seperated by commas");
            int[] changingIndex = GetManyMenuResponses(4);

            //Run loop going through each selected number's detail showing previous state and requesting what to update to
            //As response is received, push new detail to fresh object
            //Re ask for input if it does not match required dataType
            selectedActivity = GetActivityDetails(activitiesList[activityIndex], selectedActivity, changingIndex).activity;

            //Make changes in .csv
            activitiesList[activityIndex] = selectedActivity;
            Console.WriteLine("Changes have been saved to file!");
            WriteMultipleLines(activitiesList, "EntertainmentActivities.csv");
        }

        static void SearchAllActivities()
        {
            //Reading and ordering all activities from both files
            using var eReader = new StreamReader("EntertainmentActivities.csv");
            using var eCsv = new CsvReader(eReader, CultureInfo.InvariantCulture);
            var eActivitiesList = eCsv.GetRecords<EntertainmentActivity>().ToList();
            using var fReader = new StreamReader("FitnessActivities.csv");
            using var fCsv = new CsvReader(fReader, CultureInfo.InvariantCulture);
            var fActivitiesList = fCsv.GetRecords<FitnessActivity>().ToList();
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
            
            //Getting date input to filter search on
            Console.WriteLine("Please input a date for us to search from.");
            Console.WriteLine("Once a date is selected, you may view all activities on, before or after that date");
            var dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            while(dateParseResult.err != "")
            {
                Console.WriteLine(dateParseResult.err);
                dateParseResult = ConvertStringToTime(AskForActivityInput(" date in format dd/MM/yyyy"), "dd/MM/yyyy");
            }
            DateOnly searchDate = DateOnly.FromDateTime(dateParseResult.time);

            //Getting filter type for seach
            Console.WriteLine("Please select how you want to filter your search:");
            Console.WriteLine($"1. All Activities BEFORE {searchDate}");
            Console.WriteLine($"2. All Activities ON {searchDate}");
            Console.WriteLine($"3. All Activities AFTER {searchDate}");
            int input = GetMenuResponse(3);
            Console.WriteLine("\n");

            //Filtering search
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
            newActivity.DateStartTime = GetActivityDateTime();
            newActivity.Cost = GetActivityCost();
            newActivity.Location = ToTitleCase(AskForActivityInput("'s location"));
            return newActivity;
        }

        static EntertainmentActivity GetEntertainmentActivityDetails()
        {
            EntertainmentActivity newActivity = new EntertainmentActivity();
            //All entries still have to be type checked for further use
            newActivity.Title = ToTitleCase(AskForActivityInput(" name"));
            newActivity.DateStartTime = GetActivityDateTime();
            newActivity.Cost = GetActivityCost();
            var parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
            while(parseResult.err != "")
            {
                Console.WriteLine(parseResult.err);
                parseResult = ConvertStringToInt(AskForActivityInput("'s minimum required participants"));
            }
            newActivity.MinParticipants = parseResult.num;
            return newActivity;
        }

        static string GetActivityDateTime()
        {
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
            return $"{dateParseResult.time:dd/MM/yyyy} {timeParseResult.time:HH:mm}";
        }

        static int GetActivityCost()
        {
            var parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            while(parseResult.err != "")
            {
                Console.WriteLine(parseResult.err);
                parseResult = ConvertStringToInt(AskForActivityInput(" cost"));
            }
            return parseResult.num;
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

        static void WriteColour(string text, ConsoleColor colour)
        {
            ConsoleColor standardColour = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.Write(text);
            Console.ForegroundColor = standardColour;
        }
    }

}