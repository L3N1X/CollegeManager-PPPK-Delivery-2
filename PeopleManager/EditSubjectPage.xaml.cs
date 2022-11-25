using Microsoft.Win32;
using PeopleManager.Models;
using PeopleManager.Utils;
using PeopleManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PeopleManager
{
    /// <summary>
    /// Interaction logic for EditSubjectPage.xaml
    /// </summary>
    public partial class EditSubjectPage : FramedPage
    {
        private readonly Subject _subject;
        public EditSubjectPage(CollegeViewModel collegeViewModel, Subject subject = null) : base(collegeViewModel)
        {
            InitializeComponent();
            cbLecturers.DisplayMemberPath = nameof(Lecturer.FullName);
            _subject = subject ?? new Subject();
            cbLecturers.ItemsSource = collegeViewModel.Lecturers;
            if(_subject.Id != 0)
            {
                //JER PRVI PUTA SE NECE PRESELECTATI IZ RAZLOGA ZA KOJI NEMAM IZVACA SADA OTKRIVATE
                int index = collegeViewModel.Lecturers.IndexOf(collegeViewModel.Lecturers.Where(l => l.Id.Equals(_subject.LecturerId)).FirstOrDefault());
                this.cbLecturers.SelectedIndex = index;
            }
            else
            {
                int index = collegeViewModel.Lecturers.IndexOf(collegeViewModel.Lecturers.FirstOrDefault());
                this.cbLecturers.SelectedIndex = index;
            }
            DataContext = _subject;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.GoBack();
        } 

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            if (FormValid())
            {
                _subject.Name = TbCourseName.Text.Trim();
                _subject.Lecturer = cbLecturers.SelectedItem as Lecturer;
                _subject.LecturerId = (cbLecturers.SelectedItem as Lecturer).Id;
                if (_subject.Id == 0)
                {
                    CollegeViewModel.Subjects.Add(_subject);
                }
                else
                {
                    CollegeViewModel.UpdateSubject(_subject);
                }
                Frame.NavigationService.GoBack();
            }
        }

        private bool FormValid()
        {
            bool valid = true;
            GridContainer.Children.OfType<TextBox>().ToList().ForEach(e =>
            {
                if (string.IsNullOrEmpty(e.Text.Trim()))
                {
                    e.Background = Brushes.Coral;
                    valid = false;
                }
                else
                {
                    e.Background = Brushes.White;
                }
            });
            if(cbLecturers.SelectedItem == null)
            {
                cbLecturers.Background= Brushes.LightCoral;
            }
            else
            {
                cbLecturers.Background = Brushes.White;
            }
            return valid;
        }

    }
}
