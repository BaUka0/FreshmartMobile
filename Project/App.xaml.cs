using Project.Models;
using Project.Pages;
using Project.Services;

namespace Project
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService;
        public App(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            MainPage = new SplashPage();
        }

        protected override async void OnStart()
        {
            await _databaseService.InitAsync();

            var user = await _databaseService.GetUserByCredentialsAsync("admin", "admin");
            if(user == null)
            {
                await _databaseService.AddUserAsync(new User
                {
                    username = "admin",
                    password = "admin",
                    role = "admin"
                });
            }
        }
    }
}