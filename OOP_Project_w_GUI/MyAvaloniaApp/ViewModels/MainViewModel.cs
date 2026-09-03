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
    public record Country(string Name, string Region, int Population);

    public record Activity(string DateStartTime, string Title, float Cost);
    public record FitnessActivity(string DateStartTime, string Title, float Cost, string Location);
    public record EntertainmentActivity(string DateStartTime, string Title, float Cost, int MinParticipants);

    public class RawCountry
    {
        public string Name {get;set;}
        public string Region {get;set;}
        public int Population {get;set;}
    }

    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to the Activity Editor!";
    
    public ObservableCollection<Country> Countries { get; set;} = new();

    public ObservableCollection<FitnessActivity> FitnessActivities {get; set;} = new();

    public MainViewModel()
    {
        using var reader = new StreamReader("CountriesTestCsv.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var countriesList = csv.GetRecords<RawCountry>().ToList();
        for (int i = 0; i < countriesList.Count; i++)
        {
            Country newCountry = new Country(countriesList[i].Name, countriesList[i].Region, countriesList[i].Population);
            Countries.Add(newCountry);
        }
    }

    public void AddNewCountry(Country newCountry)
    {
        Countries.Add(newCountry);
        string[] countriesForCsv = ConvertListToCsvArray(Countries);
        CreateAndFillCSV(Countries, "CountriesTestCsv.csv");
    }

    public void AddNewFitnessActivity(FitnessActivity newActivity)
    {
        FitnessActivities.Add(newActivity);
    }

    public string[] ConvertListToCsvArray(ObservableCollection<Country> countries)
    {
        string[] countriesForCsv = [];
        for (int i = 0; i < countries.Count; i++)
        {
            string nextCountry = $"{countries[i].Name},{countries[i].Region},{countries[i].Population}";
            countriesForCsv = countriesForCsv.Append(nextCountry).ToArray();
        }
        return countriesForCsv;
    }

    public void CreateAndFillCSV(ObservableCollection<Country> countries, string csvFileName)
    {
        using (var writer = new StreamWriter(csvFileName))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(countries);
        }
    }
}