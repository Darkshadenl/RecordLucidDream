using HouseScraping.Views.Homescreen;

namespace HouseScraping;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(RecordAudioPage), typeof(RecordAudioPage));
        // Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}

	// private async void OnSettingsClicked(object sender, EventArgs e)
    // {
    //     await Shell.Current.GoToAsync("//SettingsPage");
    // }
}
