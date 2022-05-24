using System;
using Todo.Utilites;
using Todo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Todo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();


            if (string.IsNullOrWhiteSpace(UserSecureStorage.JwtBearerToken))
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage =new NavigationPage(new TodoListPage());
            }

            
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
