using System;

namespace FilesToDatabaseImporter.Models
{
    public class File
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
    }
}