namespace Project.Pages;

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
}