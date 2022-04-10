using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AssignmentProj
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserSignUp : ContentPage
    {
        ICustomNotification notification;

        public List<User> users = new List<User>();
        public UserSignUp()
        {
            InitializeComponent();
            notification = DependencyService.Get<ICustomNotification>();
        }
        private async void signUp(object sender, EventArgs e)
        {
            if (username.Text == string.Empty || password.Text == string.Empty || repeatPassword.Text == string.Empty)
            {
                error.Text = "Fields Cannot Be Empty";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            if (username.Text == "" || password.Text == "" || repeatPassword.Text == "")
            {
                error.Text = "Fields Cannot Be Empty";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (username.Text.Length < 5)
            {
                error.Text = "Username Length Cant Be Less Than 5 Characters Long";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (name.Text.Length < 3)
            {
                error.Text = "Name Length Cant Be Less Than 3 Characters Long";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (password.Text.Length < 5 && repeatPassword.Text == password.Text)
            {
                error.Text = "Password Length Cant Be Less Than 5 Characters Long";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (repeatPassword.Text != password.Text)
            {
                error.Text = "Passwords Must Match";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else
            {
                var user = users.FirstOrDefault(h => h.userName == username.Text);
                if (user == null) 
                {
                    await App.Database.SaveUserAsync(new User
                    {
                        name = name.Text,
                        userName = username.Text,
                        password = password.Text,
                    });
                    var t = Task.Run(async delegate { notification.send($"Notes", "Account Created"); });
                    // await DisplayAlert("Account Created", $"Account {username.Text} Created!", "Return to log in");

                    await Navigation.PushAsync(new MainPage());
                }
                else
                { error.Text = "Username already exists"; }
            }
        }

        private async void backToLogIn(object sender, EventArgs e)
        { await Navigation.PushAsync(new MainPage()); }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            users = await App.Database.GetUsersAsync();
            name.Focus();
        }
    }
}