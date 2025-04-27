namespace Project;

public partial class HomePage : ContentPage
{
    public List<string> CarouselItems { get; set; }

    public HomePage()
    {
        InitializeComponent();

        CarouselItems = new List<string>
        {
            "carusel_one.png",
            "carusel_two.png",
            "carusel_three.png",
        };

        BindingContext = this;
    }

    private async void abu_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//login");
    }
}
