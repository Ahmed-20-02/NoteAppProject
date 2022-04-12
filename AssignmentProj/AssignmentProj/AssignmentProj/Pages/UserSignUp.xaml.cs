using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AssignmentProj
{
    /// <UNIT TEST 1>
    // Arrange signing up
    // Arranged usernames, " ", "Ben", "Benjamin", "Ahmed22"
    // after creating user with username "Ahmed22"

    // Act
    // Try all usernames with valid name, valid password length and passwords match

    // Assert
    //  Expect " " to force empty username error
    //  Expect "Ben" to force username length error
    //  Expect "Benjamin" to direct me to log in page and produce a notification
    //  Expect "Ahmed22" to force username already exists error

    // Result
    //  " " forces empty username error
    //  "Ben" forces username length error
    //  "Benjamin" directed me to log in page and notified me that "Ahmed22" account created
    //  "Ahmed22" forces username already exists error
    /// </UNIT TEST 1>

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
            if (username.Text == string.Empty || password.Text == string.Empty || repeatPassword.Text == string.Empty || username.Text == "" || password.Text == "" || repeatPassword.Text == "")
            {
                error.Text = "Fields Cannot Be Empty*";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (username.Text.Length < 5)
            {
                error.Text = "Username Length Cannot Be Less Than 5 Characters Long*";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (name.Text.Length < 3)
            {
                error.Text = "Name Length Cannot Be Less Than 3 Characters Long*";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (!name.Text.All(char.IsLetter))
            {
                error.Text = "Name Cannot Contain Numbers Or Symbols*";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (password.Text.Length < 5 && repeatPassword.Text == password.Text)
            {
                error.Text = "Password Length Cannot Be Less Than 5 Characters Long*";
                password.Text = string.Empty;
                repeatPassword.Text = string.Empty;
            }
            else if (repeatPassword.Text != password.Text)
            {
                error.Text = "Passwords Must Match*";
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
                { error.Text = "Username already exists*"; }
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