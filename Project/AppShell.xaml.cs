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
        }
        //public void UpdateUsername()
        //{
        //    if (_authService.CurrentUser != null)
        //    {
        //        UsernameLabel.Text = _authService.CurrentUser.username;
        //    }
        //}
        public async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(0.8, 100);
            await button.ScaleTo(1.0, 100);

            _authService.Logout();
            
            await DisplayAlert("Выход", "Вы вышли!", "OK");

            //UsernameLabel.Text = string.Empty;
            await Shell.Current.GoToAsync("//login");
        }
    }
}
