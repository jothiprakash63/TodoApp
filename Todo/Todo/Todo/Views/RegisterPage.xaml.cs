using AspNetIdentityDemo.Shared;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Todo.Services;
using Todo.Utilites;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Todo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage,INotifyPropertyChanged
    {
        private readonly IAuthService authService;

        public RegisterPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            this.authService = RestService.For<IAuthService>(Constants.Baseurl);
        }
     
  
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        const string AuthenticationUrl = Constants.Baseurl+"auth/";
        private void LoginClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new LoginPage();
        }

        private async void RegisterClicked(object sender, EventArgs e)
        {
            try
            {
                RegisterRequest registerRequest = new RegisterRequest()
                {
                    ConfirmPassword = ConfirmPassword,
                    Email = EmailId,
                    Password = Password
                };

                var res = await authService.RegisterAsync(registerRequest);

                if (res != null)
                {
                    App.Current.MainPage = new LoginPage();
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        private async  void GoogleClicked(object sender, EventArgs e)
        {
            await this.OnAuthenticate("Google");
        }

        private async void FaceBookClicked(object sender, EventArgs e)
        {
            await this.OnAuthenticate("Facebook");
        }

        async Task OnAuthenticate(string scheme)
        {
            try
            {
                var authUrl = new Uri(AuthenticationUrl + scheme);
                var callbackUrl = new Uri("xamarinapp://");

                var result = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);

                string authToken = result.AccessToken;
                string refreshToken = result.RefreshToken;
              //  var jwtTokenExpiresIn = result.Properties["jwt_token_expires"];
                //var refreshTokenExpiresIn = result.Properties["refresh_token_expires"];

                //Testing token wrong expiry time
               // jwtTokenExpiresIn = TimeSpan.FromSeconds(Convert.ToInt64(jwtTokenExpiresIn)).TotalSeconds.ToString();

                var userInfo = new Dictionary<string, string>
                {
                    { "token", authToken },
                    { "name", $"{result.Properties["firstName"]} {result.Properties["secondName"]}"},
                    { "picture", HttpUtility.UrlDecode(result.Properties["picture"]) }
                };

                 var userid  =result.Properties["id"];

                if (result != null)
                {
                    UserSecureStorage.JwtBearerToken = authToken;
                    UserSecureStorage.UserId = userid;
                }

                Application.Current.MainPage =new NavigationPage( new TodoListPage());
            }
            catch (TaskCanceledException ex)
            {
                //Note: User exited auth flow;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
            }
        }
    }
}