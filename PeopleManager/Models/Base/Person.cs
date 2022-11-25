using PeopleManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PeopleManager.Models
{
    public abstract class Person
    {
        //[IDPerson], [FirstName], [LastName], [Age], [Email], [Picture]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public byte[] Picture { get; set; }
        public BitmapImage Image 
        {
            get => ImageUtils.ByteArrayToBitmapImage(Picture); 
        }
    }
}
