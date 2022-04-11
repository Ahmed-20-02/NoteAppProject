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
    public partial class Home : ContentPage
    {
        ICustomNotification notification;

        public User currentUser;
        List<Note> allNotes = new List<Note>();
        List<Note> usersNotes = new List<Note>();
        public Home(User user)
        {
            InitializeComponent();
            currentUser = user;
            helloMessage.Text = user.name + "'s Notes & Memories";
            notification = DependencyService.Get<ICustomNotification>();
        }
        protected override bool OnBackButtonPressed()
        {  return true; // true prevent navigation back and false to allow
        }

        //https://stackoverflow.com/a/68510505/14601418
        private async void onSwipeLeft(object sender, EventArgs e)
        {
            SwipeItem item = sender as SwipeItem;

            Note currentNote = item.BindingContext as Note;

            var t = Task.Run(async delegate{ notification.send($"{currentNote.title}", "Note Deleted"); });

            await App.Database.DeleteNoteAsync(currentNote);

            await Navigation.PushAsync(new Home(currentUser));
        }
        private async void onSwipeRight(object sender, EventArgs e)
        {
            SwipeItem item = sender as SwipeItem;

            Note currentNote = item.BindingContext as Note;

            await Navigation.PushAsync(new UpdateNote(currentNote, currentUser));
        }

        private async void createNote(object sender, EventArgs e)
        { await Navigation.PushAsync(new CreateNote(currentUser)); }
        private async void profile(object sender, EventArgs e)
        { await Navigation.PushAsync(new ProfilePage(currentUser)); }
        private async void searched(object sender, EventArgs e)
        {
            if (searchBar.Text == null || searchBar.Text == string.Empty || searchBar.Text == "")
            { notesList.ItemsSource = usersNotes; error.Text = ""; }
            else 
            {
                List<Note> searchedNotes = new List<Note>();
                foreach (Note note in usersNotes)
                {
                    if (note.title.Contains(searchBar.Text) || note.message.Contains(searchBar.Text))
                    { searchedNotes.Add(note); }
                }
                if (searchedNotes.Count == 0) 
                {
                    error.Text = "No Notes Found, Try Another Search Term";
                    notesList.ItemsSource = new List<Note>();
                }
                else { error.Text = ""; notesList.ItemsSource = searchedNotes; }  
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            allNotes = await App.Database.GetNotesAsync();
            //Get The Notes In Reverse Order Instead So Theyre Chronological
            for (int i = (allNotes.Count - 1); i >= 0; i--)
            {
                if (allNotes[i].authorId == currentUser.Id)
                {
                    usersNotes.Add(allNotes[i]);
                }
            }
            if (usersNotes.Count() == 0)
            { error.Text = "You Currently Have No Notes"; }
            notesList.ItemsSource = usersNotes;
        } 
    }
}
