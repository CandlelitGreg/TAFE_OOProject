using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MyAvaloniaApp.ViewModels;
using System.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Tmds.DBus.Protocol;
using Avalonia.Media;
using Avalonia.Rendering;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration.Attributes;
using CommunityToolkit.Mvvm.Collections;

namespace MyAvaloniaApp.Views;

/* Comma input must be removed for .csv. options of attack:
    -Block user from inputting any commas
    -Remove commas from inputted values
    -Prompt user to reinput if an invalid value is received
*/

public partial class MainWindow : Window
{
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


    //Calls checkForMissingInputs function to ensure all activity input fields are filled, if anything is missing, it will highlight the missing input and return without adding a new activity
    //If all inputs are filled, it will create a new Fitness Activity ObservableObject
    //The new Object is added to the appropriate Observable Collections via the mvm.AddNewFitnessActivity function
    //The input boxes are cleared for the next input
    //The displayed list of activities is updated to include the new activity
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

    //Calls checkForMissingInputs function to ensure all activity input fields are filled, if anything is missing, it will highlight the missing input and return without adding a new activity
    //If all inputs are filled, it will create a new Entertainment Activity ObservableObject
    //The new Object is added to the appropriate Observable Collections via the mvm.AddNewEntertainmentActivity function
    //The input boxes are cleared for the next input
    //The displayed list of activities is updated to include the new activity
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

    //Checks Search box is populated and displays activities that match the search period based on selected filter
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

    //Opens the stackPanel that is referenced by the button that was pressed, and disables the current stackPanel
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

    //Alters active and visible XML elements to reflect activity action currently selected
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

    //Toggles displayed XML elements to reflect the selected Fitness activity type. Changes the colour of both activity type buttons to reflect which is selected
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

    //Toggles displayed XML elements to reflect the selected Entertainment activity type. Changes the colour of both activity type buttons to reflect which is selected
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

    //Change colour of two buttons to reflect which is selected
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

    //Takes a button and a list of buttons, and changes their colours to reflect if it is selected
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

    //Catches and highlights missing inputs in TextBoxes
    //Reverts highlights if input bool is true
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

    //Catches and highlights missing inputs in CalendarDatePicker
    //Reverts highlights if input bool is true
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

    //Catches and highlights missing inputs in TimePicker
    //Reverts highlights if input bool is true
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

/*  
    Removes comas from inputted strings before searching for any missing inputs
    Checks if any of the activity input fields are missing information. 
    If any are missing, it will highlight the missing input and return true. 
    If all inputs are filled, it will return false */
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
            if (ActivityTitleInput.Text == "")
            {
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
        if (activityType == "entertainment" && ActivityMinParticipantsInput.Text == null)
        {
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

    //Resets all activity input fields to their default state, including the watermark text and border colour
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
    
    //Clears all activity input fields and resets the activity type buttons to their default state
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

/*




                    FORMATTING FUNCTIONS




*/

    //Catches and reverts non-Int inputs in textbox
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

    //Catches and reverts non-Double inputs in textbox
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

    //Takes a string input and returns the same string with all commas removed. If the string is empty or null, it will return null
    public string removeCommasFromString(string input)
    {
        string output = input.Replace(",", "");
        if (string.IsNullOrWhiteSpace(output))
        {
            output = null;
        }
        return output;
    }

    //Takes a string input and returns the same string formatted to Title Case formatting
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