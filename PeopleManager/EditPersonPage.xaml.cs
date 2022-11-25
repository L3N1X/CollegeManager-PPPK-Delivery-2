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
    /// Interaction logic for EditPersonPage.xaml
    /// </summary>
    public partial class EditPersonPage : FramedPage
    {
        private const string Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png";

        private readonly Student _student;
        public EditPersonPage(CollegeViewModel personViewModel, Student student = null) : base (personViewModel)
        {
            InitializeComponent();
            _student = student ?? new Student();
            this.LbSubjects.ItemsSource = personViewModel.Subjects;
            DataContext = _student;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Frame.NavigationService.GoBack();
        }

        private void BtnCommit_Click(object sender, RoutedEventArgs e)
        {
            if (FormValid())
            {
                _student.Age = int.Parse(TbAge.Text.Trim());
                _student.Email = TbEmail.Text.Trim();
                _student.FirstName = TbFirstName.Text.Trim();
                _student.LastName = TbLastName.Text.Trim();
                _student.Picture = ImageUtils.BitmapImageToByteArray(Picture.Source as BitmapImage);
                if (_student.Id == 0)
                {
                    CollegeViewModel.Students.Add(_student);
                }
                else
                {
                    CollegeViewModel.UpdateStudent(_student);
                }
                Frame.NavigationService.GoBack();
            }
        }

        private bool FormValid()
        {
            bool valid = true;
            GridContainer.Children.OfType<TextBox>().ToList().ForEach(e =>
            {
                if (string.IsNullOrEmpty(e.Text.Trim())
                || "Int".Equals(e.Tag) && !int.TryParse(e.Text.Trim(), out int r)
                || "Email".Equals(e.Tag) && !ValidationUtils.isValidEmail(e.Text.Trim()))
                {
                    e.Background = Brushes.Coral;
                    valid = false;
                }
                else
                {
                    e.Background = Brushes.White;
                }
            });
            if(Picture.Source == null)
            {
                PictureBorder.Background = Brushes.LightCoral;
                valid = false;
            }
            else
            {
                PictureBorder.Background = Brushes.White;
            }
            return valid;
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Filter,
            };
            if(dialog.ShowDialog() == true)
            {
                Picture.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }
    }
}
