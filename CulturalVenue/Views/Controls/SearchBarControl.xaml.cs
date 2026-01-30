using System.ComponentModel;
using System.Windows.Input;

namespace CulturalVenue.Views.Controls;

public partial class SearchBar : ContentView
{
	public static readonly BindableProperty TextProperty = 
		BindableProperty.Create(
		nameof(Text),
		typeof(string),
		typeof(SearchBar),
		string.Empty,
		BindingMode.TwoWay);

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
    }

    public SearchBar()
	{
		InitializeComponent();
		ClearCommand = new Command(ClearText);
    }

	public ICommand ClearCommand { get; }

	public void ClearText()
	{
		Text = string.Empty;
    }
}