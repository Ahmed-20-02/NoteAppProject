using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AssignmentProj
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        ICustomNotification notification;

        public User currentUser;

        bool setVisible = false;
        public ProfilePage(User user)
        {
            InitializeComponent();
            currentUser = user;
            id.Text = currentUser.Id.ToString();
            name.Text = currentUser.name;
            username.Text = currentUser.userName;

            notification = DependencyService.Get<ICustomNotification>();
        }
        //DISABLE BACK BUTTON. USE ON EACH PAGE IF NO FIX
        /*protected override bool OnBackButtonPressed() { return false; }*/
        
        private async void showPass(object sender, EventArgs e)
        {
            if(setVisible == false)
            { setVisible= true; showOrHide.Text = "Hide Password"; password.IsPassword = false; newPassword.IsPassword = false; }
            else if(setVisible == true) 
            { setVisible = false; showOrHide.Text = "Show Password"; password.IsPassword = true; newPassword.IsPassword = true; }
        }
        private async void logOut(object sender, EventArgs e)
        {
            Preferences.Clear();
            var t = Task.Run(async delegate { notification.send($"{currentUser.userName}", "Logged Out"); });
            await Navigation.PushAsync(new MainPage());
        }
        private async void home(object sender, EventArgs e)
        {  await Navigation.PushAsync(new Home(currentUser)); }
        private async void updateAccount(object sender, EventArgs e)
        {
            if(password.Text != currentUser.password )//ADD RIGHT VALIDATION
            { error.Text = "Password Does Not Match"; }
            else if(newPassword.Text.Length < 5 || newPassword.Text == "" || newPassword.Text == null || newPassword.Text == string.Empty)
            { error.Text = "New Password Length Is Less Than 5 Characters"; }
            else if(newPassword.Text == currentUser.password)
            { error.Text = "New Password Must Differ From Old Password"; }
            else  
            { 
                currentUser.password = newPassword.Text;
                await App.Database.UpdateUserAsync(currentUser);
                var t = Task.Run(async delegate { notification.send($"{currentUser.userName}", "Account Updated"); });
              //  await DisplayAlert("Password Updated Successfully", "Success", "Ok");
                await Navigation.PushAsync(new ProfilePage(currentUser));
            }
        }
        private async void deleteAccount(object sender, EventArgs e)
        {
            Preferences.Clear();
            var t = Task.Run(async delegate { notification.send($"{currentUser.userName}", "Account Deleted"); });
            await App.Database.DeleteUserAsync(currentUser);
            currentUser = new User();
            await Navigation.PushAsync(new MainPage());
        }
    }
}