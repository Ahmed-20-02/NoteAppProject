using AssignmentProj;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentProj
{
    public class database
    {
        private readonly SQLiteAsyncConnection _dDConn;
        public database(string dbPath)
        {
            _dDConn = new SQLiteAsyncConnection(dbPath);
            _dDConn.CreateTableAsync<User>();
            _dDConn.CreateTableAsync<Note>();
        }

        public Task<List<User>> GetUsersAsync()
        { return _dDConn.Table<User>().ToListAsync(); }
        public Task<List<Note>> GetNotesAsync()
        { return _dDConn.Table<Note>().ToListAsync(); }
        public Task<int> SaveUserAsync(User user)
        { return _dDConn.InsertAsync(user); }
        public Task<int> SaveNoteAsync(Note note)
        { return _dDConn.InsertAsync(note); }
        public Task<int> UpdateUserAsync(User user)
        { return _dDConn.UpdateAsync(user); }
        public Task<int> UpdateNoteAsync(Note note)
        { return _dDConn.UpdateAsync(note); }
        public Task<int> DeleteUserAsync(User user)
        { return _dDConn.DeleteAsync(user);}
        public Task<int> DeleteNoteAsync(Note note)
        { return _dDConn.DeleteAsync(note); }
        public Task<int> DeleteAllUsersNotesAsync(int id)
        { return _dDConn.ExecuteAsync($"DELETE FROM Note WHERE authorId='{id}'"); }
    }
}
