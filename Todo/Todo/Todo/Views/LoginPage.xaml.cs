using AspNetIdentityDemo.Shared;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Services;
using Todo.Utilites;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Todo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService authService;

        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            this.authService = RestService.For<IAuthService>(Constants.Baseurl);
        }

        public string EmailId { get; set; }
        public string Password { get; set; }

        private async void LoginClicked(object sender, EventArgs e)
        {
            LoginRequest request = new LoginRequest()
            {
                 Email=EmailId,
                 Password=Password
            };
           var res =  await this.authService.LoginAsync(request);

            if (res != null)
            {
                UserSecureStorage.JwtBearerToken = res.Message;
                UserSecureStorage.UserId = res.UserId;
            }
            Application.Current.MainPage = new NavigationPage(new TodoListPage());
        }

        private void SignInClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new RegisterPage();
        }
    }
}