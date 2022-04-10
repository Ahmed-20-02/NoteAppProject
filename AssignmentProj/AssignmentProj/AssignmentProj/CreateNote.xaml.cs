using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AssignmentProj
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateNote : ContentPage
    {
        //https://stackoverflow.com/a/71774792/14601418

        ICustomNotification notification;

        User currentUser;
        public static string photofilename;
        public static string imagePath;
        public string location = "Location And Timestamp Not Set";
        public CreateNote(User user)
        {
            InitializeComponent();
            currentUser = user;
            notification = DependencyService.Get<ICustomNotification>();
        }
        private async void cancel(object sender, EventArgs e)
        { await Navigation.PushAsync(new Home(currentUser)); }
        private async void save(object sender, EventArgs e)
        {
            //VALIDATION

            if (subject.Text == string.Empty || content.Text == string.Empty)
            {
                error.Text = "Neither Fields Can Be Empty";
                subject.Text = string.Empty;
                content.Text = string.Empty;
            }
            else if (subject.Text == "" || content.Text == "")
            {
                error.Text = "Neither Fields Can Be Empty";
                subject.Text = string.Empty;
                content.Text = string.Empty;
            }
            else
            {
                await App.Database.SaveNoteAsync(new Note
                {
                    authorId = currentUser.Id,
                    title = subject.Text,
                    message = content.Text,
                    location = location,
                    imagePath = imagePath
                }) ;
                var t = Task.Run(async delegate{ notification.send($"{subject.Text}", "Note Created"); });
                selectedPhoto.Source = null;
                photofilename = null;
                imagePath = null;
              //  await DisplayAlert("Memory Created Successfully", "I Hope It Was A Happy One :)", "Return Home");
                await Navigation.PushAsync(new Home(currentUser));
                    
            }
        }
        private async void locationTicked(object sender, EventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                try
                {
                    var locationVariable = await Geolocation.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromMinutes(1)));
                    string[] formatLocation = locationVariable.ToString().Split(',');
                    location = formatLocation[0] + " " + formatLocation[1] + " " + formatLocation[7];
                    readOnlyLocation.Text = location;
                }
                catch (PermissionException)
                { 
                    checkBox.IsChecked = false;
                    await DisplayAlert("Please Accept 'Location' Permission In App Settings To Use This Feature", "Permission", "OK"); 
                }
            }
            else { location = "Location And Timestamp Not Set"; readOnlyLocation.Text = "Location And Timestamp Not Set";}
        }
        private async void selectPhoto(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select An Image Or Cancel" });
                LoadPhotoAsync(result);
                var stream = await result.OpenReadAsync();
                photofilename = result.FileName;
                selectedPhoto.Source = ImageSource.FromStream(() => stream);
                imagePath = Path.Combine(FileSystem.AppDataDirectory, photofilename);
            }
            catch (NullReferenceException nre){  }
            catch (ArgumentNullException ane){  }
            catch (ArgumentException ae){  }
            catch (PermissionException pe)
            { await DisplayAlert("Please Accept 'Files and media' Permission In App Settings To Use This Feature", "Permission", "OK");}
        }
        private async void takePhoto(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions { Title = "Select An Image Or Cancel" });
                LoadPhotoAsync(result);
                var stream = await result.OpenReadAsync();
                photofilename = result.FileName;
                selectedPhoto.Source = ImageSource.FromStream(() => stream);
                imagePath = Path.Combine(FileSystem.AppDataDirectory, photofilename);
            }
            catch (NullReferenceException nre)
            {  }
            catch (ArgumentNullException ane)
            {  }
            catch (ArgumentException ae)
            { }
            catch (PermissionException pe)
            { await DisplayAlert("Please Accept 'Camera' Permission In App Settings To Use This Feature", "Permission", "OK"); }
        }
        async void LoadPhotoAsync(FileResult photo)
        {
            // canceled
            string PhotoPath;
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;

        }
        void deletePhoto(object sender, EventArgs e)
        {
            selectedPhoto.Source = null;
            photofilename = null;
            imagePath = null;
        }
    }
}