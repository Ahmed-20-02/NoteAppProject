using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AssignmentProj
{
    public partial class MainPage : ContentPage
    {
        ICustomNotification notification;

        public List<User> users = new List<User>();

        bool setVisible = false;
        public MainPage()
        {
            InitializeComponent();
            notification = DependencyService.Get<ICustomNotification>();
        }

        protected override bool OnBackButtonPressed()
        {  return true; // true prevent navigation back and false to allow
        }
        private async void showPass(object sender, EventArgs e)
        {
            if (setVisible == false)
            { setVisible = true; showOrHide.Text = "Hide Password"; password.IsPassword = false; }
            else if (setVisible == true) { setVisible = false; showOrHide.Text = "Show Password"; password.IsPassword = true; }
        }
        private async void goToSignUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserSignUp());
        }
        private async void goToHomePage(object sender, EventArgs e)
        {
            if (username.Text == string.Empty || password.Text == string.Empty || username.Text == "" || password.Text == "")
            {
                error.Text = "Fields Cannot Be Empty*";
                username.Text = string.Empty;
                password.Text = string.Empty;
            }
            else if (username.Text.Length < 5)
            {
                error.Text = "Username Length Cannot Be Less Than 5 Characters Long*";
                username.Text = string.Empty;
                password.Text = string.Empty;
            }
            else if (password.Text.Length < 5)
            {
                error.Text = "Password Length Cannot Be Less Than 5 Characters Long*";
                username.Text = string.Empty;
                password.Text = string.Empty;
            }

            else
            {
                User user = users.FirstOrDefault(h => h.userName == username.Text && h.password == password.Text);
                if (user != null) 
                {
                    if(saveLogIn.IsChecked == true)
                    { Preferences.Set("CurrentUser", user.Id.ToString());}

                    var t = Task.Run(async delegate{ notification.send($"{user.userName}", "Logged In"); });
                    error.Text = "";
                    await Navigation.PushAsync(new Home(user));
                }
                else { error.Text = "User Not Found*"; }
            }
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            username.Focus();

            users = await App.Database.GetUsersAsync();

            string ifLoggedIn = Preferences.Get("CurrentUser", null);

            if (ifLoggedIn != null)
            {
                User user = users.FirstOrDefault(h => h.Id == Convert.ToInt32( ifLoggedIn));
                await Navigation.PushAsync(new Home(user));
            }

        }
    }
}
