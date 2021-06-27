using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для FilmWindow.xaml
    /// </summary>
    public partial class FilmWindow : Window
    {
        Film film;
        public FilmWindow(Film film)
        {
            InitializeComponent();
            this.film = film;
            name.Text = film.Name;
            year.Text = film.Year;
            genre.Text = film.Genre;
            duration.Text = film.Duration;
            ageLimit.Text = film.AgeLimit + " лет";
            description.Text = film.Description;

            MemoryStream strIm = new MemoryStream(film.Imagine);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strIm;
            myBitmapImage.DecodePixelWidth = 270;
            myBitmapImage.DecodePixelHeight = 250;
            myBitmapImage.EndInit();
            img.Source = myBitmapImage;
        }
    }
}
