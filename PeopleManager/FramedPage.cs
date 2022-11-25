using PeopleManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PeopleManager
{
    public class FramedPage : Page
    {
        public FramedPage(CollegeViewModel personViewModel)
        {
            StudentViewModel = personViewModel;
        }
        public CollegeViewModel StudentViewModel { get; }
        public Frame Frame { get; set; }
    }
}
