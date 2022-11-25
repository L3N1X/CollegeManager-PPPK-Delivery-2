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
    /// Interaction logic for ListPeoplePage.xaml
    /// </summary>
    public partial class ListPeoplePage : FramedPage
    {
        public ListPeoplePage(CollegeViewModel personViewModel) : base(personViewModel)
        {
            InitializeComponent();
            LvPeople.ItemsSource = personViewModel.Students;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new EditPersonPage(personViewModel: StudentViewModel)
            {
                Frame = Frame
            });
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (LvPeople.SelectedItems != null)
            {
                Frame.Navigate(new EditPersonPage(personViewModel: StudentViewModel, LvPeople.SelectedItem as Student)
                {
                    Frame = Frame
                });
            }
           
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LvPeople.SelectedItems != null)
            {
                StudentViewModel.Students.Remove(LvPeople.SelectedItem as Student);
            }
        }
    }
}
