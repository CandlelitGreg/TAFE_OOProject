using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CommunityToolkit.Mvvm.Collections;
using Avalonia.Controls.Converters;

namespace MyAvaloniaApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    public static MainViewModel Instance { get; } = new MainViewModel();

    public record Activity(string DateStartTime, string Title, float Cost, string Type, int Index);
    public record FitnessActivity(string DateStartTime, string Title, float Cost, string Location);
    public record EntertainmentActivity(string DateStartTime, string Title, float Cost, int MinParticipants);

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to the Activity Editor!";

    public ObservableCollection<FitnessActivity> FitnessActivities {get; set;} = new();
    public ObservableCollection<EntertainmentActivity> EntertainmentActivities {get; set;} = new();
    public ObservableCollection<Activity> AllActivities {get; set;} = new();
    public ObservableCollection<Activity> DisplayedActivities {get; set;} = new();

    public MainViewModel()
    {
        var fitnessActivitiesList = GetListFromFile<FitnessActivity>("FitnessActivities.csv");
        var entertainmentActivitiesList = GetListFromFile<EntertainmentActivity>("EntertainmentActivities.csv");
        AllActivities = GetAllActivities(fitnessActivitiesList, entertainmentActivitiesList);
        for (int i = 0; i < AllActivities.Count; i++)
        {
            DisplayedActivities.Add(AllActivities[i]);
        }
    }

    // Function to read a CSV file and return a list of records of type T (Used primarily for reading activity files)
    public List<T> GetListFromFile<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var list = csv.GetRecords<T>().ToList();
        return list;
    }


    // Function adds FitnessActivity to ObservableCollection, AllActivities Collection, and the FitnessActivities CSV file
    public void AddNewFitnessActivity(FitnessActivity newActivity)
    {
        DisplayedActivities.Clear();
        FitnessActivities.Add(newActivity);
        AllActivities.Add(new Activity(newActivity.DateStartTime, newActivity.Title, newActivity.Cost, "Fitness", FitnessActivities.Count - 1));
        CreateAndFillCSV(FitnessActivities, "FitnessActivities.csv");
        for (int i = 0; i < AllActivities.Count; i++)
        {
            DisplayedActivities.Add(AllActivities[i]);
        }
    }

    // Function adds FitnessActivity to ObservableCollection, AllActivities Collection, and the FitnessActivities CSV file
    public void AddNewEntertainmentActivity(EntertainmentActivity newActivity)
    {
        DisplayedActivities.Clear();
        EntertainmentActivities.Add(newActivity);
        AllActivities.Add(new Activity(newActivity.DateStartTime, newActivity.Title, newActivity.Cost, "Entertainment", EntertainmentActivities.Count - 1));
        CreateAndFillCSV(EntertainmentActivities, "EntertainmentActivities.csv");
        for (int i = 0; i < AllActivities.Count; i++)
        {
            DisplayedActivities.Add(AllActivities[i]);
        }
    }

    // Function to convert an ObservableCollection of type T to a string array in CSV format
    //THIS FUNCTION IS NOT CURRENTLY USED, BUT MAY BE USEFUL IN THE FUTURE
    // public string[] ConvertListToCsvArray<T>(ObservableCollection<T> list)
    // {
    //     string[] csvArray = [];
    //     for (int i = 0; i < list.Count; i++)
    //     {
    //         var properties = typeof(T).GetProperties();
    //         var values = properties.Select(p => p.GetValue(list[i])?.ToString() ?? string.Empty);
    //         csvArray = csvArray.Append(string.Join(",", values)).ToArray();
    //     }
    //     return csvArray;
    // }

    // Function to create a CSV file and fill it with records from an ObservableCollection of type T
    public void CreateAndFillCSV<T>(ObservableCollection<T> list, string csvFileName)
    {
        using (var writer = new StreamWriter(csvFileName))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(list);
        }
    }

    // Function to combine fitness and entertainment activities into a single list of activities
    // Also fills FitnessActivities and EntertainmentActivities ObservableCollections with the respective activities
    public ObservableCollection<Activity> GetAllActivities(List<FitnessActivity> fitnessActivities, List<EntertainmentActivity> entertainmentActivities)
    {
        AllActivities.Clear();
        FitnessActivities.Clear();
        EntertainmentActivities.Clear();
        for (int i = 0; i < fitnessActivities.Count; i++)
        {
            FitnessActivities.Add(fitnessActivities[i]);
            Activity newActivity = new Activity(fitnessActivities[i].DateStartTime, fitnessActivities[i].Title, fitnessActivities[i].Cost, "Fitness", i);
            AllActivities.Add(newActivity);
        }
        for (int i = 0; i < entertainmentActivities.Count; i++)
        {
            EntertainmentActivities.Add(entertainmentActivities[i]);
            Activity newActivity = new Activity(entertainmentActivities[i].DateStartTime, entertainmentActivities[i].Title, entertainmentActivities[i].Cost, "Entertainment", i);
            AllActivities.Add(newActivity);
        }
        return AllActivities;
    }
}