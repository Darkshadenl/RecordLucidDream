namespace HouseScraping;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        // Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
	}

	// private async void OnSettingsClicked(object sender, EventArgs e)
    // {
    //     await Shell.Current.GoToAsync("//SettingsPage");
    // }
}
