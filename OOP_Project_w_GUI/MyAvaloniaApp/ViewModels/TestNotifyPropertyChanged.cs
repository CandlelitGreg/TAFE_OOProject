// using Avalonia;
// using Avalonia.Controls;
// using System;
// using System.Linq;
// using System.Globalization;
// using System.Text;
// using System.IO;
// using System.Collections.ObjectModel;
// using CommunityToolkit.Mvvm.ComponentModel;
// using CsvHelper;
// using CsvHelper.Configuration;

// namespace MyAvaloniaApp.ViewModels;

// public class MyViewModel : INotifyPropertyChanged
// {
//     private string _name;

//     public string Name
//     {
//         get { return _name; }
//         set
//         {
//             _name = value;
//             OnPropertyChanged(nameof(Name));
//         }
//     }

//     public event PropertyChangedEventHandler PropertyChanged;

//     protected virtual void OnPropertyChanged(string propertyName)
//     {
//         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//     }
// }