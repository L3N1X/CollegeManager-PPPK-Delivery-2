using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleManager.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LecturerId { get; set; }
        public Lecturer Lecturer { get; set; }
        public ICollection<Student> Students { get; set; }
        public Subject()
        {
            this.Students = new HashSet<Student>(); 
        }
        public int StudentsEnrolled { get => Students.Count; }
        public string LecturerName { get => Lecturer.FullName; }
        public bool IsChecked { get; set; } = false;
    }
}
