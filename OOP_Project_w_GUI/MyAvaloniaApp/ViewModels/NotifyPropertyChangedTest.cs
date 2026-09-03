// using Avalonia;
// using Avalonia.Controls;
// using Avalonia.Controls.Converters;
// using System.ComponentModel;
// using System.Runtime.CompilerServices;

// public class Country : INotifyPropertyChanged
// {
//     private string _name;
//     private string _region;
//     private int _population;

//     public string Name
//     {
//         get => _name;
//         set => SetProperty(ref _name, value);
//     }

//     public string Region
//     {
//         get => _region;
//         set => SetProperty(ref _region, value);
//     }

//     public int Population
//     {
//         get => _population;
//         set => SetProperty(ref _population, value);
//     }

//     public event PropertyChangedEventHandler? PropertyChanged;

//     protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
//     {
//         if (Equals(storage, value)) return;
//         storage = value;
//         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//     }
// }
