using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Microsoft.Data.SqlClient;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyAvaloniaApp.ViewModels;
using System.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Tmds.DBus.Protocol;
using Avalonia.Media;
using Avalonia.Rendering;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration.Attributes;
using CommunityToolkit.Mvvm.Collections;
using Avalonia.Media.TextFormatting;

namespace MyAvaloniaApp.Views;

/* Comma input must be removed for .csv. options of attack:
    -Block user from inputting any commas
    -Remove commas from inputted values
    -Prompt user to reinput if an invalid value is received
*/

public partial class MainWindow : Window
{
    string connectionString = "Server=localhost,1433;Database=ActivitiesDb;User Id=sa;Password=Sussex2510!;TrustServerCertificate=True";
    private MainViewModel mvm = new MainViewModel();
    public MainWindow()
    {
        InitializeComponent();
    }

    //Used for int input textBoxes
    private string _lastValidIntText = "";
    private string _lastValidCostText = "";
    private bool costIsHoldingDecimal = false;
    private int validCostDollarLength = 0;

/*




                    ACTIVITY MANAGEMENT FUNCTIONS




*/

    /// <summary>
    /// Calls checkForMissingInputs function to ensure all activity input fields are filled, if anything is missing, it will highlight the missing input and return without adding a new activity
    ///If all inputs are filled, it will create a new Fitness Activity ObservableObject
    ///The new Object is added to the appropriate Observable Collections via the mvm.AddNewFitnessActivity function
    ///The input boxes are cleared for the next input
    /// The displayed list of activities is updated to include the new activity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void addFitnessActivity(object sender, RoutedEventArgs e)
    {
        if (checkForMissingInputs("fitness"))
        {
            return;
        }
        //insert activity input data into a new object
        MainViewModel.FitnessActivity newActivity = 
            new MainViewModel.FitnessActivity(
                $"{ActivityDateInput.SelectedDate.Value.ToString("dd/MM/yyyy")} {ActivityTimeInput.SelectedTime.Value.ToString(@"hh\:mm")}",
                ToTitleCase(ActivityTitleInput.Text.ToString()),
                float.Parse(ActivityCostInput.Text),
                ToTitleCase(ActivityLocationInput.Text.ToString())
            );
        
        //Add the new activity into the existing list
        mvm.AddNewFitnessActivity(newActivity);

        //Clear input boxes
        clearActivityInputs();

        // // Update the existing list to show the new activity
        ActivitiesList.ItemsSource = mvm.DisplayedActivities;
        
        Console.WriteLine($"new fitness activity added\nName: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Title}\nStart Time: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].DateStartTime}\nCost: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Cost}\nLocation: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Location}");
    }

    /// <summary>
    /// Calls checkForMissingInputs function to ensure all activity input fields are filled, if anything is missing, it will highlight the missing input and return without adding a new activity
    /// If all inputs are filled, it will create a new Entertainment Activity ObservableObject
    /// The new Object is added to the appropriate Observable Collections via the mvm.AddNewEntertainmentActivity function
    /// The input boxes are cleared for the next input
    /// The displayed list of activities is updated to include the new activity
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void addEntertainmentActivity(object sender, RoutedEventArgs e)
    {
        if (checkForMissingInputs("entertainment"))
        {
            return;
        }
        //insert activity input data into a new object
        MainViewModel.EntertainmentActivity newActivity = 
            new MainViewModel.EntertainmentActivity(
                $"{ActivityDateInput.SelectedDate.Value.ToString("dd/MM/yyyy")} {ActivityTimeInput.SelectedTime.Value.ToString(@"hh\:mm")}",
                ToTitleCase(ActivityTitleInput.Text.ToString()),
                float.Parse(ActivityCostInput.Text),
                int.Parse(ActivityMinParticipantsInput.Text)
            );
        
        //Add the new activity into the existing list
        mvm.AddNewEntertainmentActivity(newActivity);

        //Clear input boxes
        clearActivityInputs();

        // // Update the existing list to show the new activity
        ActivitiesList.ItemsSource = mvm.DisplayedActivities;

        // for (int i = 0; i < mvm.DisplayedActivities.Count; i++)
        // {
        //     Console.WriteLine(mvm.DisplayedActivities[i].Title);
        // }
        
        // Console.WriteLine($"new entertainment activity added\nName: {mvm.EntertainmentActivities[mvm.EntertainmentActivities.Count - 1].Title}\nStart Time: {mvm.EntertainmentActivities[mvm.EntertainmentActivities.Count - 1].DateStartTime}\nCost: {mvm.EntertainmentActivities[mvm.EntertainmentActivities.Count - 1].Cost}\nMinimum Participants: {mvm.EntertainmentActivities[mvm.EntertainmentActivities.Count - 1].MinParticipants}");
    }

    /// <summary>
    /// Checks Search box is populated and displays activities that match the search period based on selected filter
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void searchActivities(object sender, RoutedEventArgs e)
    {
        if (checkForMissingInputs("search"))
        {
            return;
        }
        mvm.DisplayedActivities.Clear();
        DateTime searchDate = ActivitySearchDateInput.SelectedDate ?? DateTime.Now; 
        
        List<MainViewModel.FitnessActivity> fitnessList = mvm.FitnessActivities.ToList();
        List<MainViewModel.EntertainmentActivity> entertainmentList = mvm.EntertainmentActivities.ToList();
        List<MainViewModel.Activity> allActivitiesList = [];
        for (int i = 0; i < fitnessList.Count; i++)
        {
            MainViewModel.Activity newActivity = new MainViewModel.Activity(DateTime.Parse(fitnessList[i].DateStartTime), fitnessList[i].Title, fitnessList[i].Cost, "Fitness", i);
            allActivitiesList.Add(newActivity);
        }
        for (int i = 0; i < entertainmentList.Count; i++)
        {
            MainViewModel.Activity newActivity = new MainViewModel.Activity(DateTime.Parse(entertainmentList[i].DateStartTime), entertainmentList[i].Title, entertainmentList[i].Cost, "Entertainment", i);
            allActivitiesList.Add(newActivity);
        }
        List<MainViewModel.Activity> orderedActivities = allActivitiesList.OrderByDescending(a => a.DateStartTime).ToList();
        bool searchBefore = (SearchBefore as RadioButton)?.IsChecked ?? false;
        bool searchOn = (SearchOn as RadioButton)?.IsChecked ?? false;
        bool searchAfter = (SearchAfter as RadioButton)?.IsChecked ?? false;

        //Tried Switch casse but didn't work as values were of type ?bool not bool
        if (searchBefore == true)
        {
            int start = 0;
            while (start < orderedActivities.Count && DateOnly.FromDateTime(orderedActivities[start].DateStartTime) >= DateOnly.FromDateTime(searchDate))
            {
                start++;
            }
            for (int i = start; i < orderedActivities.Count; i++)
            {
                mvm.DisplayedActivities.Add(orderedActivities[i]);
            }
        }
        if (searchOn == true)
        {
            int start = 0;
            while (start < orderedActivities.Count && DateOnly.FromDateTime(orderedActivities[start].DateStartTime) != DateOnly.FromDateTime(searchDate))
            {
                start++;
            }
            while (start < orderedActivities.Count && DateOnly.FromDateTime(orderedActivities[start].DateStartTime) == DateOnly.FromDateTime(searchDate))
            {
                mvm.DisplayedActivities.Add(orderedActivities[start]);
                start++;
            }
        }
        if (searchAfter == true)
        {
            int start = 0;
            while (start < orderedActivities.Count && DateOnly.FromDateTime(orderedActivities[start].DateStartTime) > DateOnly.FromDateTime(searchDate))
            {
                mvm.DisplayedActivities.Add(orderedActivities[start]);
                start++;
            }
        }
        ActivitiesList.ItemsSource = mvm.DisplayedActivities;
    }

/*



                
                    PANEL AND BUTTON MANAGEMENT FUNCTIONS




*/

    /// <summary>
    /// Opens the stackPanel that is referenced by the button that was pressed, and disables the current stackPanel
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void openPanel(object sender, RoutedEventArgs e)
    {
        if (sender is Visual visual)
        {
            StackPanel? parentPanel = visual.FindAncestorOfType<StackPanel>();
            if (parentPanel != null)
            {
                parentPanel.IsEnabled = false;
                parentPanel.IsVisible = false;
            }
        }
        if (sender is Button button && button.Tag is StackPanel referencedPanel)
        {
            referencedPanel.IsEnabled = true;
            referencedPanel.IsVisible = true;
        }
    }

    /// <summary>
    /// Alters active and visible XML elements to reflect activity action currently selected
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void openActivitySidePanel(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is StackPanel referencedPanel)
        {
            AddActivitiesPanel.IsEnabled = false;
            AddActivitiesPanel.IsVisible = false;
            EditActivitiesPanel.IsEnabled = false;
            EditActivitiesPanel.IsVisible = false;
            SearchActivitiesPanel.IsEnabled = false;
            SearchActivitiesPanel.IsVisible = false;
            referencedPanel.IsEnabled = true;
            referencedPanel.IsVisible = true;
            selectSingleButton(sender, new System.Collections.Generic.List<Button> {AddActivitiesButton, EditActivitiesButton, SearchActivitiesButton});
        }
    }

    /// <summary>
    /// Toggles displayed XML elements to reflect the selected Fitness activity type. 
    /// Changes the colour of both activity type buttons to reflect which is selected
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void selectFitness(object sender, RoutedEventArgs e)
    {
        //When button is pressed, change stackpanel properties visible/enabled
        SubmitFitnessActivityPanel.IsEnabled = true;
        SubmitFitnessActivityPanel.IsVisible = true;
        SubmitEntertainmentActivityPanel.IsEnabled = false;
        SubmitEntertainmentActivityPanel.IsVisible = false;
        //Also change colour of this button and entertainment button to show this is selected
        selectBinaryButton(sender);
    }

    /// <summary>
    /// Toggles displayed XML elements to reflect the selected Entertainment activity type. 
    /// Changes the colour of both activity type buttons to reflect which is selected
    /// </summary>
    /// <param name="sender">The button calling the function</param>
    /// <param name="e">EventHandler Overload</param>
    public void selectEntertainment(object sender, RoutedEventArgs e)
    {
        //When button is pressed, change stackpanel properties visible/enabled
        SubmitFitnessActivityPanel.IsEnabled = false;
        SubmitFitnessActivityPanel.IsVisible = false;
        SubmitEntertainmentActivityPanel.IsEnabled = true;
        SubmitEntertainmentActivityPanel.IsVisible = true;
        //Also change colour of this button and fitness button to show this is selected
        selectBinaryButton(sender);
    }

    /// <summary>
    /// Change colour of two buttons to reflect which is selected
    /// </summary>
    /// <param name="sender">The selected button</param>
    public void selectBinaryButton(object sender)
    {
        if (sender is Button button)
        {
            button.Background = new SolidColorBrush(Color.Parse("#27325F"));
            button.Foreground = new SolidColorBrush(Color.Parse("#fff"));
            if (button.Tag is Button otherButton)
            {
                otherButton.Background = new SolidColorBrush(Color.Parse("#E0E0E0"));
                otherButton.Foreground = new SolidColorBrush(Color.Parse("#000"));
            }
                
        }
    }

    /// <summary>
    /// Takes a button and a list of buttons, and changes their colours to reflect if it is selected
    /// </summary>
    /// <param name="sender">The selected button</param>
    /// <param name="allButtons">The buttons that should be displayed as unselected</param>
    public void selectSingleButton(object sender, System.Collections.Generic.List<Button> allButtons)
    {
        if (sender is Button button)
        {
            button.Background = new SolidColorBrush(Color.Parse("#27325F"));
            button.Foreground = new SolidColorBrush(Color.Parse("#fff"));
        }
        for (int i = 0; i < allButtons.Count; i++)
        {
            if (allButtons[i] != sender)
            {
                allButtons[i].Background = new SolidColorBrush(Color.Parse("#E0E0E0"));
                allButtons[i].Foreground = new SolidColorBrush(Color.Parse("#000"));
            }
        }
    }


/*





                        INPUT MANAGEMENT FUNCTIONS





*/

    /// <summary>
    /// Catches and highlights missing inputs in TextBoxes
    /// Reverts highlights if input bool is true
    /// </summary>
    /// <param name="missingInput">The input field that is missing input</param>
    /// <param name="switchBack">Whether the input field is being highlighted, or unhighlighted</param>
    public void highlightTextInput(TextBox missingInput, bool switchBack)
    {
        if (switchBack || missingInput.BorderBrush is Avalonia.Media.SolidColorBrush currentBrush && currentBrush.Color != Avalonia.Media.Colors.Red)
        {
            string tempTag = missingInput.Watermark.ToString();
            missingInput.Watermark = missingInput.Tag.ToString();
            missingInput.Tag = tempTag;
        }
        if (!switchBack)
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#ff0000"));
            missingInput.BorderThickness = new Thickness(2.5);
        } else
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#A6000000"));
            missingInput.BorderThickness = new Thickness(1);
        }
    }

    /// <summary>
    /// Catches and highlights missing inputs in CalendarDatePicker
    /// Reverts highlights if input bool is true
    /// </summary>
    /// <param name="missingInput">The input field that is missing input</param>
    /// <param name="switchBack">Whether the input field is being highlighted, or unhighlighted</param>
    public void highlightDateInput(CalendarDatePicker missingInput, bool switchBack)
    {
        if (switchBack || missingInput.BorderBrush is Avalonia.Media.SolidColorBrush currentBrush && currentBrush.Color != Avalonia.Media.Colors.Red)
        {
            string tempTag = missingInput.Watermark.ToString();
            missingInput.Watermark = missingInput.Tag.ToString();
            missingInput.Tag = tempTag;
        }
        if (!switchBack)
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#ff0000"));
            missingInput.BorderThickness = new Thickness(2.5);
        } else
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#A6000000"));
            missingInput.BorderThickness = new Thickness(1);
        }
    }

    /// <summary>
    /// Catches and highlights missing inputs in TimePicker
    /// Reverts highlights if input bool is true
    /// </summary>
    /// <param name="missingInput">The input field that is missing input</param>
    /// <param name="switchBack">Whether the input field is being highlighted, or unhighlighted</param>
    public void highlightTimeInput(TimePicker missingInput, bool switchBack)
    {
        if (!switchBack)
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#ff0000"));
            missingInput.BorderThickness = new Thickness(2.5);
        } else
        {
            missingInput.BorderBrush = new SolidColorBrush(Color.Parse("#A6000000"));
            missingInput.BorderThickness = new Thickness(1);
        }
    }

    /// <summary>
    /// Removes comas from inputted strings before searching for any missing inputs
    /// Checks if any of the activity input fields are missing information. 
    /// If any are missing, it will highlight the missing input and return true. 
    /// If all inputs are filled, it will return false
    /// </summary>
    /// <param name="activityType">string stating the activity type - to know what fields to check</param>
    /// <returns>A bool that states true if there is a missing input (else false)</returns>
    public bool checkForMissingInputs(string activityType)
    {
        if (activityType == "search")
        {
            highlightDateInput(ActivitySearchDateInput, true);
            if (ActivitySearchDateInput.SelectedDate == null)
            {
                highlightDateInput(ActivitySearchDateInput, false);
                return true;
            }
            return false;
        }


        resetActivityInputHighlights();
        /*MAKE SURE TO CHECK FOR NULL TYPES AND COMAS
        PROMPT USER TO ADD MISSING INPUTS IF INFORMATION IS MISSING*/
        if (ActivityTitleInput.Text != null)
        {
            ActivityTitleInput.Text = removeCommasFromString(ActivityTitleInput.Text.ToString());
            if (ActivityTitleInput.Text == "" || ActivityTitleInput.Text.Length < 3)
            {
                DisplayMessage("Input Error", "Please ensure activity title contains at least 3 characters");
                ActivityTitleInput.Text = null;
            }
        }

        if (activityType == "fitness" && ActivityLocationInput.Text != null)
        {
            ActivityLocationInput.Text = removeCommasFromString(ActivityLocationInput.Text.ToString());
            if (ActivityLocationInput.Text == "")
            {
                ActivityLocationInput.Text = null;
            }
        }
        bool missingInput = false;

        if (ActivityDateInput.SelectedDate == null)
        {
            highlightDateInput(ActivityDateInput, false);
            missingInput = true;
        } else if (CheckActivityDate(ActivityDateInput.SelectedDate.Value.ToString("dd/MM/yyyy")))
        {
            Console.WriteLine("Activity already on date");
            highlightDateInput(ActivityDateInput, false);
            missingInput = true;
        }
        if (ActivityTimeInput.SelectedTime == null)
        {
            highlightTimeInput(ActivityTimeInput, false);
            missingInput = true;
        }
        if (ActivityTitleInput.Text == null)
        {
            highlightTextInput(ActivityTitleInput, false);
            missingInput = true;
        }
        if (ActivityCostInput.Text == null)
        {
            highlightTextInput(ActivityCostInput, false);
            missingInput = true;
        }

        //
        //TODO: Run check to ensure number is more than 1 - Insert different watermark
        //
        if ((activityType == "entertainment" && ActivityMinParticipantsInput.Text == null) || (activityType == "entertainment" && ActivityMinParticipantsInput.Text != null && int.Parse(ActivityMinParticipantsInput.Text) < 2))
        {
            if(ActivityMinParticipantsInput.Text != null)
            {
                ActivityMinParticipantsInput.Text = null;
                DisplayMessage("Input error", "Please ensure Minimum Participant value is 2 or greater");
            }
            highlightTextInput(ActivityMinParticipantsInput, false);
            missingInput = true;
        }
        if (activityType == "fitness" && ActivityLocationInput.Text == null)
        {
            highlightTextInput(ActivityLocationInput, false);
            missingInput = true;
        }
        return missingInput;
    }

    /// <summary>
    /// Resets all activity input fields to their default state, including the watermark text and border colour
    /// </summary>
    public void resetActivityInputHighlights()
    {
        if (ActivityTitleInput.Watermark == "TITLE INPUT REQUIRED")
        {
            highlightTextInput(ActivityTitleInput, true);
        }
        if (ActivityLocationInput.Watermark == "LOCATION INPUT REQUIRED")
        {
            highlightTextInput(ActivityLocationInput, true);
        }
        if (ActivityCostInput.Watermark == "COST INPUT REQUIRED")
        {
            highlightTextInput(ActivityCostInput, true);
        }
        if(ActivityDateInput.Watermark == "DATE INPUT REQUIRED")
        {
            highlightDateInput(ActivityDateInput, true);
        }
        highlightTimeInput(ActivityTimeInput, true);
        if (ActivityMinParticipantsInput.Watermark == "MINIMUM PARTICIPANT COUNT REQUIRED")
        {
            highlightTextInput(ActivityMinParticipantsInput, true);
        }
    }
    
    /// <summary>
    /// Clears all activity input fields and resets the activity type buttons to their default state
    /// </summary>
    public void clearActivityInputs()
    {
        resetActivityInputHighlights();
        ActivityTitleInput.Text = null;
        ActivityLocationInput.Text = null;
        ActivityCostInput.Text = null;
        ActivityDateInput.SelectedDate = null;
        ActivityTimeInput.SelectedTime = null;
        ActivityMinParticipantsInput.Text = null;
        SubmitEntertainmentActivityPanel.IsEnabled = false;
        SubmitEntertainmentActivityPanel.IsVisible = false;
        SubmitFitnessActivityPanel.IsEnabled = false;
        SubmitFitnessActivityPanel.IsVisible = false;
        FitnessActivityTypeButton.Background = new SolidColorBrush(Color.Parse("#27325F"));
        FitnessActivityTypeButton.Foreground = new SolidColorBrush(Color.Parse("#fff"));
        EntertainmentActivityTypeButton.Background = new SolidColorBrush(Color.Parse("#27325F"));
        EntertainmentActivityTypeButton.Foreground = new SolidColorBrush(Color.Parse("#fff"));
    }

    /// <summary>
    /// Checks the input date does not have another activity attached
    /// </summary>
    /// <param name="date">The date to check in the db</param>
    /// <returns>A true or false bool depicting whether another activity falls on the given date</returns>
    public bool CheckActivityDate(string date)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("CheckActivityExistsByDate", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateToCheck", DateOnly.FromDateTime(DateTime.Parse(date)));

                conn.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (int.Parse($"{reader["ActivityExists"]}") == 1)
                        {
                            DisplayMessage("Activity Conflict", "Another activity already holds this date, please select a different date");
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Displays a message box with an input error notification
    /// </summary>
    /// <param name="firstLine">Message box name</param>
    /// <param name="secondLine">Message box text</param>
    public async void DisplayMessage(string firstLine, string secondLine)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            firstLine,
            secondLine,
            ButtonEnum.Ok);
        var result = await box.ShowAsync();
    }

/*




                    FORMATTING FUNCTIONS




*/

    /// <summary>
    /// Catches and reverts non-Int inputs in textbox
    /// </summary>
    /// <param name="sender">The XAML object sending the function call to check for non ints</param>
    /// <param name="e">EventHandler Overload</param>
    public void catchNonIntInput(object? sender, TextChangingEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            //creates and assigns a nullable string to textbox input
            string? currentText = textBox.Text;
            
            //If the textbox string is not null and not an int - input is invalid
            if (currentText is not null && !currentText.All(char.IsDigit))
            {
                //Take the function out of the Textbox's textChanging value to prevent overlapping calls
                textBox.TextChanging -= catchNonIntInput;

                //Revert the textbox text to the last known valid input
                textBox.Text = _lastValidIntText;

                //Reassign the function to the Textbox's textChanging value
                textBox.TextChanging += catchNonIntInput;
                return;
            }
            //Assign the current textbox input to the lastValidText string var to be used in invalid input case
            _lastValidIntText = currentText ?? "";
        }
    }

    /// <summary>
    /// Catches and reverts non int to hundredths inputs in textbox
    /// </summary>
    /// <param name="sender">The XAML object sending the function call to check for non cost</param>
    /// <param name="e">EventHandler Overload</param>
    public void catchNonCostInput(object? sender, TextChangingEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            //creates and assigns a nullable string to textbox input
            string? currentText = textBox.Text;
            
            //If the textbox string is not null and not an int - input is invalid
            if (currentText is not null && !currentText.All(char.IsDigit))
            {
                //Is the current input end in a decimal point and inputHoldingDecimal is false?
                if ((currentText.ToCharArray()[currentText.ToCharArray().Length - 1].ToString() == ".") && !costIsHoldingDecimal)
                {
                    //Accept the input and set lastValidCostText to textBox.Text
                    _lastValidCostText = currentText ?? "";
                    //Set inputHoldingDecimal to true
                    costIsHoldingDecimal = true;
                    //Set validDollarCostLength to string length
                    validCostDollarLength = currentText.ToCharArray().Length;
                    return;
                }
                //else if inputHoldingDecimal is true AND currentDecimal is digit AND input string length < validDollarCostLength + 2
                else if (costIsHoldingDecimal && (validCostDollarLength + 2) >= currentText.ToCharArray().Length && char.IsDigit(currentText.ToCharArray()[currentText.ToCharArray().Length - 1]))
                {
                    //Accept the input and set lastValidCostText to textBox.Text
                    _lastValidCostText = currentText ?? "";
                    return;
                }

                //Take the function out of the Textbox's textChanging value to prevent overlapping calls
                textBox.TextChanging -= catchNonCostInput;

                //Revert the textbox text to the last known valid input
                textBox.Text = _lastValidCostText;

                //Reassign the function to the Textbox's textChanging value
                textBox.TextChanging += catchNonCostInput;
                return;
            }
            //IF inputHoldingDecimal = true
            if (costIsHoldingDecimal)
            {
                //inputHoldingDecimal = false
                costIsHoldingDecimal = false;

                //validDollarCostLength = 0
                validCostDollarLength = 0;
            }
                
            //Assign the current textbox input to the lastValidText string var to be used in invalid input case
            _lastValidCostText = currentText ?? "";
        }
    }

    /// <summary>
    /// Takes a string input and returns the same string with all commas removed. If the string is empty or null, it will return null
    /// </summary>
    /// <param name="input">The string commas must be removed from</param>
    /// <returns>The input string with removed comas or null if no text remains</returns>
    public string removeCommasFromString(string input)
    {
        string output = input.Replace(",", "");
        if (string.IsNullOrWhiteSpace(output))
        {
            output = null;
        }
        return output;
    }

    /// <summary>
    /// Takes a string input and returns the same string formatted to Title Case formatting
    /// </summary>
    /// <param name="input">The string to format into title case</param>
    /// <returns>The input string in Title case format</returns>
    public string ToTitleCase(string input)
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