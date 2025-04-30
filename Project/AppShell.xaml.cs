using Project.Pages;
using Project.Pages.Admin;
using Project.Services;
using System.Threading.Tasks;

namespace Project
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;
        public AppShell(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("admin/userlist", typeof(UserListPage));
            Routing.RegisterRoute("home", typeof(HomePage));

            CheckStatus();
        }

        private async void CheckStatus()
        {
            UpdateTabs(GetCurrentUserRole());
            await GoToAsync("///home");
        }
        private string GetCurrentUserRole() => _authService.GetCurrentUserRole();

        public void UpdateTabs(string role)
        {
            var tabBar = appTabBar;

            if (tabBar == null)
                return;

            tabBar.Items.Clear();

            switch (role)
            {
                case "client":
                    tabBar.Items.Add(homeTab);
                    tabBar.Items.Add(catalogTab);
                    tabBar.Items.Add(favouriteTab);
                    tabBar.Items.Add(cartTab);
                    tabBar.Items.Add(profileTab);
                    break;
                case "seller":
                    tabBar.Items.Add(homeTab);
                    tabBar.Items.Add(catalogTab);
                    tabBar.Items.Add(dashboardTab);
                    tabBar.Items.Add(profileTab);
                    break;

                case "admin":
                    tabBar.Items.Add(homeTab);
                    tabBar.Items.Add(catalogTab);
                    tabBar.Items.Add(userList);
                    tabBar.Items.Add(applicationList);
                    tabBar.Items.Add(profileTab);
                    break;

                case "guest":
                    tabBar.Items.Add(homeTab);
                    tabBar.Items.Add(catalogTab);
                    tabBar.Items.Add(loginTab);
                    break;

                default:
                    tabBar.Items.Add(homeTab);
                    break;
            }
        }
        
        public async void OnUserLoggedIn()
        {
            UpdateTabs(GetCurrentUserRole());
            await GoToAsync("///home");
        }
        public async void OnUserLoggedOut()
        {
            appTabBar.Items.Clear();
            await GoToAsync("///login");
        }

    }
}
