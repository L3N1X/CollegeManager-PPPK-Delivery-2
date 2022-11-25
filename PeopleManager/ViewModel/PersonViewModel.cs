using PeopleManager.Dal;
using PeopleManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleManager.ViewModel
{
    public class CollegeViewModel
    {
        public ObservableCollection<Student> Students { get; }
        public ObservableCollection<Subject> Subjects { get; }
        public ObservableCollection<Lecturer> Lecturers { get; }

        public CollegeViewModel()
        {
            Students = new ObservableCollection<Student>(RepositoryFactory.GetRepository().GetStudents());
            Subjects = new ObservableCollection<Subject>(RepositoryFactory.GetRepository().GetSubjects());
            Lecturers = new ObservableCollection<Lecturer>(RepositoryFactory.GetRepository().GetLecturers());
            Students.CollectionChanged += Students_CollectionChanged;
            Subjects.CollectionChanged += Subjects_CollectionChanged;
            Lecturers.CollectionChanged += Lecturers_CollectionChanged;
        }

        private void Lecturers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    RepositoryFactory.GetRepository().AddLecturer(Lecturers[e.NewStartingIndex]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RepositoryFactory.GetRepository().DeleteLecturer(
                        e.OldItems.OfType<Lecturer>().ToList()[0]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RepositoryFactory.GetRepository().UpdateLecturer(
                        e.NewItems.OfType<Lecturer>().ToList()[0]);
                    break;

            }
        }

        private void Subjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    RepositoryFactory.GetRepository().AddSubject(Subjects[e.NewStartingIndex]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RepositoryFactory.GetRepository().DeleteSubject(
                        e.OldItems.OfType<Subject>().ToList()[0]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RepositoryFactory.GetRepository().UpdateSubject(
                        e.NewItems.OfType<Subject>().ToList()[0]);
                    break;

            }
        }

        private void Students_CollectionChanged(object sender, 
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    RepositoryFactory.GetRepository().AddStudent(Students[e.NewStartingIndex]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RepositoryFactory.GetRepository().DeleteStudent(
                        e.OldItems.OfType<Student>().ToList()[0]);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RepositoryFactory.GetRepository().UpdateStudent(
                        e.NewItems.OfType<Student>().ToList()[0]);
                    break;
               
            }
        }
        public void UpdateStudent(Student student) => Students[Students.IndexOf(student)] = student;
        public void UpdateSubject(Subject subject) => Subjects[Subjects.IndexOf(subject)] = subject;
        public void UpdateLecturer(Lecturer lecturer) => Lecturers[Lecturers.IndexOf(lecturer)] = lecturer;
    }
}
