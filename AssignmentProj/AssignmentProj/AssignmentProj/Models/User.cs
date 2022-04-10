using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProj
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string name { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
       // public List<Note> notes { get; set; }
    }
}
