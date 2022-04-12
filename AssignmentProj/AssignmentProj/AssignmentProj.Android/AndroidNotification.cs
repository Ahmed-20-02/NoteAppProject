using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AssignmentProj.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

//https://youtu.be/G2TTote5pcI
//https://github.com/ichim/xamarin.forms/blob/master/Notification/Not/Not.Android/AndroidNotification.cs

[assembly: Dependency(typeof(AssignmentProj.Droid.AndroidNotification))]
namespace AssignmentProj.Droid
{
    public class AndroidNotification : ICustomNotification
    {
        const string channel_id = "default";
        const string channel_name = "Default";
        int notify_index = 0;

        public void send(string title, string message)
        {
            NotificationManager manager = (NotificationManager)Android.App.Application.Context.GetSystemService(Android.App.Application.NotificationService);

            var channelNameJava = new Java.Lang.String(channel_name);
            var channel = new NotificationChannel(channel_id, channelNameJava, NotificationImportance.High)
            {
                Description = "Channel Description"
            };

            manager.CreateNotificationChannel(channel);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channel_id)

                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.NoteAppLogo))
                .SetSmallIcon(Resource.Drawable.NoteAppLogo)
                .SetPriority((int)NotificationPriority.High)
                .SetVisibility((int)NotificationVisibility.Public)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            Notification notification = builder.Build();
            manager.Notify(notify_index++, notification);

        }
    }
}