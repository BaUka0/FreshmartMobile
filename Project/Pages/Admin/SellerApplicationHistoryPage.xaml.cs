using Project.Models;
using Project.Services;
using System.Collections.ObjectModel;

namespace Project.Pages.Admin;

public partial class SellerApplicationHistoryPage : ContentPage
{
    public ObservableCollection<SellerApplication> ApplicationsHistory { get; set; } = new ObservableCollection<SellerApplication>();
	private readonly DatabaseService _databaseService;
    public SellerApplicationHistoryPage(DatabaseService databaseService)
	{
		InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;

        if(!Resources.ContainsKey("StatusToColorConverter"))
        {
            Resources.Add("StatusToColorConverter", new StatusToColorConverter());
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadApplicationsHistoryAsync();
    }
    private async Task LoadApplicationsHistoryAsync()
    {
        ApplicationsHistory.Clear();
        var applications = await _databaseService.GetSellerApplicationsWithUsersAsync();
        foreach (var application in applications)
        {
            ApplicationsHistory.Add(application);
        }
    }
}