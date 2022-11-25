﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleManager.Models
{
    public class Student : Person
    {
        public ICollection<Subject> Subjects { get; set; }

        public Student()
        {
            this.Subjects = new HashSet<Subject>();
        }
    }
}
