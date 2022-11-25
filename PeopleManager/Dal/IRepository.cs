using PeopleManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleManager.Dal
{
    public interface IRepository
    {
        void AddStudent(Student student);
        void UpdateStudent(Student student);
        void DeleteStudent(Student student);
        ICollection<Student> GetStudents();
        Student GetStudent(int id);

        void AddLecturer(Lecturer lecturer);
        void UpdateLecturer(Lecturer lecturer);
        void DeleteLecturer(Lecturer lecturer);
        ICollection<Lecturer> GetLecturers();
        Lecturer GetLecturer(int id);

        void AddSubject(Subject subject);
        void UpdateSubject(Subject subject);
        void DeleteSubject(Subject subject);
        ICollection<Subject> GetSubjects();
        Subject GetSubject(int id);
    }
}
