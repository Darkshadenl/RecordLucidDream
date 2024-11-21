using HouseScraping.ViewModels;

namespace HouseScraping.Views.Homescreen;

public partial class RecordAudioPage : ContentPage
{
	public RecordAudioPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}