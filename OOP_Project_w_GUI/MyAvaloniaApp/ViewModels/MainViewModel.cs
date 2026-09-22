using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Data;
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
using Microsoft.Data.SqlClient;
using Tmds.DBus.Protocol;

namespace MyAvaloniaApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{

    string connectionString = "Server=localhost,1433;Database=ActivitiesDb;User Id=sa;Password=Sussex2510!;TrustServerCertificate=True";
    public static MainViewModel Instance { get; } = new MainViewModel();

    public record Activity(DateTime DateStartTime, string Title, float Cost, string Type, int Index);
    public record FitnessActivity(string DateStartTime, string Title, float Cost, string Location);
    public record EntertainmentActivity(string DateStartTime, string Title, float Cost, int MinParticipants);


    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to the Activity Editor!";

    public ObservableCollection<FitnessActivity> FitnessActivities {get; set;} = new();
    public ObservableCollection<EntertainmentActivity> EntertainmentActivities {get; set;} = new();
    public ObservableCollection<Activity> AllActivities {get; set;} = new();
    public ObservableCollection<Activity> DisplayedActivities {get; set;} = new();

    /// <summary>
    /// Runs initial info setup, reading info from .csv files, creating the correct lists/collections, and displaying it appropriately
    /// </summary>
    public MainViewModel()
    {
        // Console.WriteLine("connection string created");
        // using (SqlConnection conn = new SqlConnection(connectionString))
        // {
        //     Console.WriteLine("Created Connection");
        //     using (SqlCommand cmd = new SqlCommand("GetAllActivities", conn))
        //     {
        //         Console.WriteLine("Created Command");
        //         cmd.CommandType = CommandType.StoredProcedure;

        //         conn.Open();
        //         Console.WriteLine("Connection open");
        //         using(SqlDataReader reader = cmd.ExecuteReader())
        //         {
        //             while (reader.Read())
        //             {
        //                 Console.WriteLine($"Activity Id: {reader["ActivityID"]} - DateStartTime: {reader["DateStartTime"]} - Title: {reader["Title"]} - Cost: {reader["Cost"]}");
        //             }
        //         }
        //     }
        // }
        // var fitnessActivitiesList = GetListFromFile<FitnessActivity>("FitnessActivities.csv");
        // var entertainmentActivitiesList = GetListFromFile<EntertainmentActivity>("EntertainmentActivities.csv");

        GetFitnessListFromDB();
        GetEntertainmentListFromDB();
        AllActivities = GetAllActivities(FitnessActivities, EntertainmentActivities);
        for (int i = 0; i < AllActivities.Count; i++)
        {
            DisplayedActivities.Add(AllActivities[i]);
        }
    }

    // /// <summary>
    // /// Function to read a CSV file and return a list of records of type T (Used primarily for reading activity files)
    // /// </summary>
    // /// <typeparam name="T">Type of object to read from .csv file</typeparam>
    // /// <param name="filePath">filepath for .csv file</param>
    // /// <returns>List of T type objects read from .csv file</returns>
    // public List<T> GetListFromFile<T>(string filePath)
    // {
    //     using var reader = new StreamReader(filePath);
    //     using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    //     var list = csv.GetRecords<T>().ToList();
    //     return list;
    // }

    /// <summary>
    /// Reads all the Fitness activities from the db into the fitness Activities Observable Collection
    /// </summary>
    public void GetFitnessListFromDB()
    {
        FitnessActivities.Clear();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("GetAllFitnessActivities", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FitnessActivities.Add(new MainViewModel.FitnessActivity($"{reader["DateStartTime"]}", $"{reader["Title"]}", float.Parse($"{reader["Cost"]}"), $"{reader["Location"]}"));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reads all the Fitness activities from the db into the entertainment Activities Observable Collection
    /// </summary>
    public void GetEntertainmentListFromDB()
    {
        EntertainmentActivities.Clear();
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("GetAllEntertainmentActivities", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EntertainmentActivities.Add(new MainViewModel.EntertainmentActivity($"{reader["DateStartTime"]}", $"{reader["Title"]}", float.Parse($"{reader["Cost"]}"), int.Parse($"{reader["MinParticipants"]}")));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Function adds FitnessActivity to ObservableCollection, AllActivities Collection, and the FitnessActivities CSV file
    /// </summary>
    /// <param name="newActivity">Object of Fitness Activity type to insert into any relevant observable collections and .csv file</param>
    public void AddNewFitnessActivity(FitnessActivity newActivity)
    {
        DisplayedActivities.Clear();
        FitnessActivities.Add(newActivity);
        AllActivities.Add(new Activity(DateTime.Parse(newActivity.DateStartTime), newActivity.Title, newActivity.Cost, "Fitness", FitnessActivities.Count - 1));
        // CreateAndFillCSV(FitnessActivities, "FitnessActivities.csv");
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("AddFitnessActivity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateStartTime", DateTime.Parse(newActivity.DateStartTime));
                cmd.Parameters.AddWithValue("@Title", newActivity.Title);
                cmd.Parameters.AddWithValue("@Cost", newActivity.Cost);
                cmd.Parameters.AddWithValue("@Location", newActivity.Location);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        for (int i = 0; i < AllActivities.Count; i++)
        {
            DisplayedActivities.Add(AllActivities[i]);
        }
    }

    /// <summary>
    /// Function adds EntertainmentActivity to ObservableCollection, AllActivities Collection, and the EntertainmentActivities CSV file
    /// </summary>
    /// <param name="newActivity">Object of Entertainment Activity type to insert into any relevant observable collections and .csv file</param>
    public void AddNewEntertainmentActivity(EntertainmentActivity newActivity)
    {
        DisplayedActivities.Clear();
        EntertainmentActivities.Add(newActivity);
        AllActivities.Add(new Activity(DateTime.Parse(newActivity.DateStartTime), newActivity.Title, newActivity.Cost, "Entertainment", EntertainmentActivities.Count - 1));
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("AddEntertainmentActivity", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DateStartTime", DateTime.Parse(newActivity.DateStartTime));
                cmd.Parameters.AddWithValue("@Title", newActivity.Title);
                cmd.Parameters.AddWithValue("@Cost", newActivity.Cost);
                cmd.Parameters.AddWithValue("@MinParticipants", newActivity.MinParticipants);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
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

    /// <summary>
    /// Function to create a CSV file and fill it with records from an ObservableCollection of type T
    /// </summary>
    /// <typeparam name="T">Type of objects in observable collection</typeparam>
    /// <param name="list">Observable Collection of objects of type T to write to csv file</param>
    /// <param name="csvFileName">.csv filepath</param>
    public void CreateAndFillCSV<T>(ObservableCollection<T> list, string csvFileName)
    {
        using (var writer = new StreamWriter(csvFileName))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(list);
        }
    }

    // /// <summary>
    // /// Function to combine fitness and entertainment activities into a single list of activities
    // /// Also fills FitnessActivities and EntertainmentActivities ObservableCollections with the respective activities
    // /// </summary>
    // /// <param name="fitnessActivities">List of all fitness activities</param>
    // /// <param name="entertainmentActivities">List of all entertainment activities</param>
    // /// <returns>Observable collection of all fitness and entertainment activities converted into standard activity type object/record</returns>
    // public ObservableCollection<Activity> GetAllActivities(List<FitnessActivity> fitnessActivities, List<EntertainmentActivity> entertainmentActivities)
    // {
    //     AllActivities.Clear();
    //     FitnessActivities.Clear();
    //     EntertainmentActivities.Clear();
    //     for (int i = 0; i < fitnessActivities.Count; i++)
    //     {
    //         FitnessActivities.Add(fitnessActivities[i]);
    //         Activity newActivity = new Activity(DateTime.Parse(fitnessActivities[i].DateStartTime), fitnessActivities[i].Title, fitnessActivities[i].Cost, "Fitness", i);
    //         AllActivities.Add(newActivity);
    //     }
    //     for (int i = 0; i < entertainmentActivities.Count; i++)
    //     {
    //         EntertainmentActivities.Add(entertainmentActivities[i]);
    //         Activity newActivity = new Activity(DateTime.Parse(entertainmentActivities[i].DateStartTime), entertainmentActivities[i].Title, entertainmentActivities[i].Cost, "Entertainment", i);
    //         AllActivities.Add(newActivity);
    //     }
    //     return AllActivities;
    // }

    /// <summary>
    /// Combines the activities from both the Fitness and Entertainment Observable Collections into the AllActivities Collection 
    /// </summary>
    /// <param name="fitnessActivities">Observable Collection containing all fitness Activities to merge into new collection</param>
    /// <param name="entertainmentActivities">Observable Collection containing all entertainment Activities to merge into new collection</param>
    /// <returns>Observable Collection with all fitness and entertainment activities</returns>
    public ObservableCollection<Activity> GetAllActivities(ObservableCollection<FitnessActivity> fitnessActivities, ObservableCollection<EntertainmentActivity> entertainmentActivities)
    {
        AllActivities.Clear();
        for (int i = 0; i < fitnessActivities.Count; i++)
        {
            Activity newActivity = new Activity(DateTime.Parse(fitnessActivities[i].DateStartTime), fitnessActivities[i].Title, fitnessActivities[i].Cost, "Fitness", i);
            AllActivities.Add(newActivity);
        }
        for (int i = 0; i < entertainmentActivities.Count; i++)
        {
            Activity newActivity = new Activity(DateTime.Parse(entertainmentActivities[i].DateStartTime), entertainmentActivities[i].Title, entertainmentActivities[i].Cost, "Entertainment", i);
            AllActivities.Add(newActivity);
        }
        return AllActivities;
    }
}