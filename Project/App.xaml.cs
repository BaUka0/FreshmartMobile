using Project.Models;
using Project.Pages;
using Project.Services;

namespace Project
{
    public partial class App : Application
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        public App(DatabaseService databaseService, AuthService authService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authService = authService;
            MainPage = new SplashPage(_authService);
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