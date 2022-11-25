using PeopleManager.Models;
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
    /// Interaction logic for ListSubjectsPage.xaml
    /// </summary>
    public partial class ListSubjectsPage : FramedPage
    {
        public ListSubjectsPage(CollegeViewModel collegeViewModel) : base (collegeViewModel)
        {
            InitializeComponent();
            LvPeople.ItemsSource = collegeViewModel.Subjects;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new EditSubjectPage(collegeViewModel: CollegeViewModel)
            {
                Frame = Frame
            });
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LvPeople.SelectedItems != null)
            {
                Frame.Navigate(new EditSubjectPage(collegeViewModel: CollegeViewModel, LvPeople.SelectedItem as Subject)
                {
                    Frame = Frame
                });
            }

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LvPeople.SelectedItems != null)
            {
                CollegeViewModel.Students.Remove(LvPeople.SelectedItem as Student);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.GoBack();
        }
    }
}
