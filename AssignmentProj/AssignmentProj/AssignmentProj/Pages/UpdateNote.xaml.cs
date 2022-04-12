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
    /// <UNIT TEST 5>
    // Arrange updating note
    // Arranged updating note by clicking location checkbox and delete picture after
    // creating note with location enabled and image attached

    // Act
    // removing tick from checkbox, removing image and clicking "update"

    // Assert
    //  Expect removing tick from checkbox to remove the displayed location information
    //  Expect clicking "Delete Image" to remove the displayed image
    //  Expect clicking "Update" to redirect to home screen, show note updated notification
    //  and display note with "location and timestamp not set" with no image attached

    // Result
    //  removing tick from checkbox removes the displayed location information
    //  clicking "delete image" removes the displayed image
    //  clicking "update" redirects me to home screen, notifies me that note has updated
    //  and displays the note with "location and timestamp not set" with no image attached
    /// </UNIT TEST 5>

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateNote : ContentPage
    {

        ICustomNotification notification;

        Note currentNote = new Note();
        User currentUser = new User();

        public static string photofilename;
        public static string imagePath;
        public string location = "Location And Timestamp Not Set";
        public UpdateNote(Note note, User user)
        {
            InitializeComponent();
            currentNote = note;
            currentUser = user;
            subject.Text = currentNote.title;
            content.Text = currentNote.message;
            if(currentNote.location == "Location And Timestamp Not Set")
            { checkBox.IsChecked = false; mapButton.IsEnabled = false;}
            readOnlyLocation.Text = currentNote.location;
            selectedPhoto.Source = currentNote.imagePath;
            imagePath = currentNote.imagePath;

            notification = DependencyService.Get<ICustomNotification>();
        }
        private async void cancel(object sender, EventArgs e)
        { await Navigation.PushAsync(new Home(currentUser)); }

        private async void save(object sender, EventArgs e)
        {
            if (subject.Text == string.Empty || content.Text == string.Empty || subject.Text == "" || content.Text == "")
            {
                error.Text = "Fields Cannot Be Empty*";
                subject.Text = currentNote.title;
                content.Text = currentNote.message;
            }
            else
            {
                currentNote.title = subject.Text;
                currentNote.message = content.Text;
                currentNote.location = location;
                currentNote.imagePath = imagePath;
                await App.Database.UpdateNoteAsync(currentNote);
                var t = Task.Run(async delegate { notification.send($"{currentNote.title}", "Note Updated"); });
                await Navigation.PushAsync(new Home(currentUser));

                selectedPhoto.Source = null;
                photofilename = null;
                imagePath = null;
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
                    string[] formatTime = formatLocation[7].Split(' ');
                    location = formatLocation[0] + " " + formatLocation[1] + " " + formatTime[1] + " " + formatTime[2] + " " + formatTime[3];
                    readOnlyLocation.Text = location;
                    mapButton.IsEnabled = true;
                }
                catch (PermissionException)
                {
                    checkBox.IsChecked = false;
                   
                    await DisplayAlert("Please Accept 'Location' Permission In App Settings To Use This Feature", "Permission", "OK");
                }
            }
            else { mapButton.IsEnabled = false; location = "Location And Timestamp Not Set"; readOnlyLocation.Text = "Location And Timestamp Not Set"; }
        }

        private async void copyMapToClipboard(object sender, EventArgs e)
        {
            if (checkBox.IsChecked == true)
            {
                string[] formatLocation = location.ToString().Split(' ');

                await Clipboard.SetTextAsync(formatLocation[1] + " " + formatLocation[4]);
                await DisplayAlert("Copied", "Use The Button Below To Go To Maps and Paste In The Search Box :)", "Ok");
            }
            else { await DisplayAlert("Can't Copy", "Location For This Note Is Empty, Set Location To Enable Copying And Enable Map Button", "Ok"); }
        }
        private async void map(object sender, EventArgs e)
        {
            try
            {
                var locationVariable = await Geolocation.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromMinutes(1)));
                await Map.OpenAsync(locationVariable);
            }
            catch (PermissionException)
            {
                await DisplayAlert("Please Accept 'Location' Permission In App Settings To Use This Feature", "Permission", "OK");
            }
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
            catch (NullReferenceException nre) { }
            catch (ArgumentNullException ane) { }
            catch (ArgumentException ae) { }
            catch (PermissionException pe)
            { await DisplayAlert("Please Accept 'Files and media' Permission In App Settings To Use This Feature", "Permission", "OK"); }
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
            { }
            catch (ArgumentNullException ane)
            { }
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