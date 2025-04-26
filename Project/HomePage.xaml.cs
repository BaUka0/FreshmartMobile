namespace Project;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}
	private async void abu_Clicked(object sender, EventArgs e)
	{
        await Shell.Current.GoToAsync("//login");
    }
}