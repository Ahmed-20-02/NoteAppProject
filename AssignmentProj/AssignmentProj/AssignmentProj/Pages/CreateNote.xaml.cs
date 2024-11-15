﻿using Plugin.Media;
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
    /// <UNIT TEST 3>
    // Arrange creating note
    // Arranged ticking location checkbox, take picture and fill input box after installing app and creating new account

    // Act
    // Try ticking checkbox, take picture and fill input box then click "save"

    // Assert
    //  Expect ticking checkbox to produce a location permission request
    //  Expect clicking take picture to produce a camera permission request
    //  Expect allowing location permission to gather my location and display it
    //  Expect allowing camera permission to open native camera app and display image once taken
    //  Expect redirect to home page once "save" clicked, notification and note created is displayed as latest note

    // Result
    //  ticking the checkbox causes permission pop up
    //  clicking on camera button causes permissin pop up
    //  allowing location permission gathers my location and display it
    //  allowing camera permission opens my native camera app and display image once i take a picture
    //  clicking "save" directs me to homepage with note showing as first note and notification with note subject shown
    /// </UNIT TEST 3>

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
            if (subject.Text == string.Empty || content.Text == string.Empty || subject.Text == "" || content.Text == "")
            {
                error.Text = "Fields Cannot Be Empty*";
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
                    string[] formatTime = formatLocation[7].Split(' ');
                    location = formatLocation[0] + " " + formatLocation[1] + " " + formatTime[1] + " " + formatTime[2] + " " + formatTime[3];
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