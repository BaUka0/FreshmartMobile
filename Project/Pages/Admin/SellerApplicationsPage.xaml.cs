using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Project.Pages.Admin;

public partial class SellerApplicationsPage : ContentPage
{
    public ObservableCollection<SellerApplication> Applications { get; set; } = new ObservableCollection<SellerApplication>();

    private readonly DatabaseService _databaseService;
    public SellerApplicationsPage(DatabaseService databaseService)
	{
		InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadApplicationsAsync();
    }
    private async Task LoadApplicationsAsync()
    {
        try
        {
            var pendingApplications = await _databaseService.GetSellerApplicationsWithUsersAsync("Pending");
            Applications.Clear();
            foreach (var application in pendingApplications)
            {
                Applications.Add(application);
            }
        }
        catch(Exception ex)
        {
            await DisplayAlert("Қате", $"Өтініштер жүктелмеді.{ex.Message}", "OK");
            Debug.WriteLine($"Error loading seller applications: {ex.Message}");
        }
    }
    private async void OnAcceptButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var application = (SellerApplication)button.BindingContext;
        if (application == null || application.User == null)
        {
            await DisplayAlert("Қате", "Өтінімді табу мүмкін емес.", "OK");
            return;
        }

        bool confirmed = await DisplayAlert("Растау", "Бұл өтінімді растағыңыз келетініне сенімдісіз бе?", "Иә", "Жоқ");
        if (confirmed)
        {
            try
            {
                application.User.role = "seller";
                int userUpdateResult = await _databaseService.UpdateUserAsync(application.User);
                if (userUpdateResult > 0)
                {
                    application.Status = "Accepted";
                    int applicationUpdateResult = await _databaseService.UpdateSellerApplicationAsync(application);
                    if (applicationUpdateResult > 0)
                    {
                        await DisplayAlert("Сәтті", "Өтінім қабылданды.", "OK");
                        await LoadApplicationsAsync();
                    }
                    else
                    {
                        await DisplayAlert("Қате", "Өтінім статусы жаңарту мүмкін болмады.", "OK");
                        await LoadApplicationsAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Қате", $"Өтінішті мақұлдау мүмкін болмады.{ex.Message}", "OK");
                Debug.WriteLine($"Error accepting seller application: {ex.Message}");
            }
        }
       
    }
    private async void OnRejectButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var application = (SellerApplication)button.BindingContext;
        if (application == null || application.User == null)
        {
            await DisplayAlert("Қате", "Қолданбаны табу мүмкін емес.", "OK");
            return;
        }
        bool confirmed = await DisplayAlert("Растау", "Бұл сұрауды шынымен бас тартқыңыз келе ме?", "Да", "Нет");
        if (confirmed)
        {
            try
            {
                application.Status = "Rejected";
                int applicationUpdateResult = await _databaseService.UpdateSellerApplicationAsync(application);
                if (applicationUpdateResult > 0)
                {
                    await DisplayAlert("Успех", "Өтінім қабылданбады.", "OK");
                    await LoadApplicationsAsync();
                }
                else
                {
                    await DisplayAlert("Қате", "Өтінім статусы жаңарту мүмкін болмады.", "OK");
                    await LoadApplicationsAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Қате", $"Өтінімді қабылдамау мүмкін болмады.{ex.Message}", "OK");
                Debug.WriteLine($"Error rejecting seller application: {ex.Message}");
            }
        }
    }
    private async void OnViewHistoryButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SellerApplicationHistoryPage(_databaseService));
    }
}