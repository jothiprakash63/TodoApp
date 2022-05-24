using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Todo.Services;
using Todo.Shared;
using Todo.Utilites;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Todo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TodoListPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ITaskService taskService;

        private ObservableCollection<TaskModel> _tasks;
        public ObservableCollection<TaskModel> tasks
        {
            get { return _tasks; }
            set
            {
                _tasks = value;
                this.OnPropertyChanged();
            }
        }

        public TodoListPage()
        {
            InitializeComponent();
            this.BindingContext = this;


  
            this.taskService = RestService.For<ITaskService>(Constants.Baseurl1,  new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(UserSecureStorage.JwtBearerToken)
            });
          
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                var res = await this.taskService.GetTasksOfUser();
                tasks = new ObservableCollection<TaskModel>(res.Tasks);
            }
            catch (Exception ex)
            {


            }

        }


        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            UserSecureStorage.JwtBearerToken = string.Empty;
            UserSecureStorage.UserId = string.Empty;

            Application.Current.MainPage = new LoginPage();
        }
    }
}