using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentProj
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int authorId { get; set; }
        public string title { get; set; } 
        public string message { get; set; }
        public string location { get; set; } = string.Empty;
        public string imagePath { get; set; } = string.Empty;
    }
}
