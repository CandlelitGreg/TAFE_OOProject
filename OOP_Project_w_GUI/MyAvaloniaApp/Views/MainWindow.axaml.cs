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
using Tmds.DBus.Protocol;
using Avalonia.Media;
using Avalonia.Rendering;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration.Attributes;

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
    public void openPanel(object sender, RoutedEventArgs e)
    {
        if (sender is Visual visual && visual.Name != "BackToPreviousPanel")
        {
            StackPanel? parentPanel = visual.FindAncestorOfType<StackPanel>();
            if (parentPanel != null)
            {
                parentPanel.IsEnabled = false;
                parentPanel.IsVisible = false;
                BackToPreviousPanel.IsEnabled = true;
                BackToPreviousPanel.IsVisible = true;
                BackToPreviousPanel.Tag = parentPanel;
            }
        }
        if (sender is Button button && button.Tag is StackPanel referencedPanel)
        {
            referencedPanel.IsEnabled = true;
            referencedPanel.IsVisible = true;
            CurrentPageTracker.Tag = referencedPanel;
        }
    }

    public void backPanel(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is StackPanel referencedPanel && CurrentPageTracker.Tag is StackPanel currentPanel)
        {
            currentPanel.IsEnabled = false;
            currentPanel.IsVisible = false;

            referencedPanel.IsEnabled = true;
            referencedPanel.IsVisible = true;


            BackToPreviousPanel.Tag = currentPanel;
            CurrentPageTracker.Tag = referencedPanel;
        }
    }

    public void addCountry(object sender, RoutedEventArgs e)
    {
        MainViewModel.Country newCountry = 
            new MainViewModel.Country(
                CountryNameInput.Text.ToString(),
                CountryContinentInput.Text.ToString(),
                int.Parse(CountryPupulationInput.Text)
            );
        mvm.AddNewCountry(newCountry);
        AddCountriesList.ItemsSource = mvm.Countries;
    }

    public void addFitnessActivity(object sender, RoutedEventArgs e)
    {
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

        if (ActivityLocationInput.Text != null)
        {
            ActivityLocationInput.Text = removeCommasFromString(ActivityLocationInput.Text.ToString());
            if (ActivityLocationInput.Text == "")
            {
                ActivityLocationInput.Text = null;
            }
        }

        if (checkForMissingInputs("fitness"))
        {
            return;
        }
        //insert activity input data into a new object
        MainViewModel.FitnessActivity newActivity = 
            new MainViewModel.FitnessActivity(
                $"{ActivityDateInput.SelectedDate.Value.ToString("dd/MM/yyyy")} {ActivityTimeInput.SelectedTime.Value.ToString(@"hh\:mm")}",
                ActivityTitleInput.Text.ToString(),
                float.Parse(ActivityCostInput.Text),
                ActivityLocationInput.Text.ToString()
            );
        
        //Add the new activity into the existing list
        mvm.AddNewFitnessActivity(newActivity);

        //Clear input boxes
        clearActivityInputs();


        // // Update the existing list to show the new activity
        // AddCountriesList.ItemsSource = mvm.Countries;
        Console.WriteLine($"new fitness activity added\nName: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Title}\nStart Time: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].DateStartTime}\nCost: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Cost}\nLocation: {mvm.FitnessActivities[mvm.FitnessActivities.Count - 1].Location}");
    }

    public void addEntertainmentActivity(object sender, RoutedEventArgs e)
    {
        // MainViewModel.Activity newCountry = 
        //     new MainViewModel.Country(
        //         CountryNameInput.Text.ToString(),
        //         CountryContinentInput.Text.ToString(),
        //         int.Parse(CountryPupulationInput.Text)
        //     );
        // mvm.AddNewCountry(newCountry);
        // AddCountriesList.ItemsSource = mvm.Countries;
    }

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

    public bool checkForMissingInputs(string activityType)
    {
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
            button.Background = new SolidColorBrush(Color.Parse("#E0E0E0"));
            button.Foreground = new SolidColorBrush(Color.Parse("#000"));
            if (button.Tag is Button otherButton)
            {
                otherButton.Background = new SolidColorBrush(Color.Parse("#27325F"));
                otherButton.Foreground = new SolidColorBrush(Color.Parse("#fff"));
            }
                
        }
    }

    


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

    public string removeCommasFromString(string input)
    {
        string output = input.Replace(",", "");
        if (string.IsNullOrWhiteSpace(output))
        {
            output = null;
        }
        return output;
    }
}