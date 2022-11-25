using PeopleManager.Dal;
using PeopleManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPK2.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var subjects = RepositoryFactory.GetRepository().GetSubjects();
            foreach (var s in subjects)
            {
                System.Console.WriteLine(s.Name);
                System.Console.WriteLine($"Lecturer: {s.Lecturer.FirstName}");
                System.Console.WriteLine("Students:");
                foreach (var student in s.Students)
                {
                    System.Console.WriteLine(student.FirstName);
                }
            }
            //subjects.ToList().ForEach(s => System.Console.WriteLine(s.Name));
        }
    }
}
