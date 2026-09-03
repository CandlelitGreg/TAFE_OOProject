using CommunityToolkit.Mvvm.ComponentModel;

namespace MyAvaloniaApp.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    public record Country(string Name, string Region, int Population);
}
