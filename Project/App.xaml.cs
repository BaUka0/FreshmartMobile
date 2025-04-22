using Project.Models;
using Project.Pages;
using Project.Services;

namespace Project
{
    public partial class App : Application
    {
        public static User currentUser {  get; set; }
        public static DatabaseService DatabaseService { get; private set; }
        public App()
        {
            InitializeComponent();
            DatabaseService = new DatabaseService();
            MainPage = new SplashPage();
        }

        protected override async void OnStart()
        {
            await DatabaseService.InitAsync();

            await DatabaseService.AddUserAsync(new User
            {
                username = "admin",
                password = "admin",
                role = "admin"
            });
        }
    }
}