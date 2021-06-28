using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Data.SqlClient;
using Cinema_Ticket.Connection;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для AddFilm.xaml
    /// </summary>
    public partial class AddFilm : Window
    {
        private byte[] imageCode;
        List<string> GenreNameList;
        public AddFilm(List<string> genreNameList)
        {
            InitializeComponent();
            GenreNameList = genreNameList;
            filmGenre.ItemsSource = GenreNameList;

        }

        private void filmImgAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
            if (ofd.ShowDialog() != true) return;
            FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] ic = br.ReadBytes((Int32)fs.Length);
            imageCode = ic;

            MemoryStream strIm = new MemoryStream(imageCode);
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = strIm;
            myBitmapImage.DecodePixelWidth = 225;
            myBitmapImage.DecodePixelHeight = 160;
            myBitmapImage.EndInit();
            filmImg.Source = myBitmapImage;
        }

        public void addNewFilm()//Добавление фильма в базу
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Name_in = filmName.Text;
                        var Genre_in = filmGenre.Text;
                        var Year_in = filmYear.Text;
                        var Duration_in = filmDuration.Text;
                        var Limit_in = filmAgeLimit.Text;
                        var Start_in = filmStart.SelectedDate;
                        var End_in = filmEnd.SelectedDate;
                        var Description_in = filmDescription.Text;
                        var Img_in = imageCode;
                    string query = $@"SELECT COUNT(*) from Film  where Name = '{Name_in}' and Year = '{Year_in}' and IDGenre = (select id from Genre where Name ='{Genre_in}')";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        MessageBox.Show("Такой фильм уже существует!");
                    }
                    else
                    {
                        query = $@"insert into Film(Name,Year, Duration, StartRelease, EndRelease, Description, AgeLimit, IDGenre,IMG) values ('{Name_in}','{Year_in}','{Duration_in}',(CAST('{Start_in}' as datetime2)),(CAST('{End_in}' as datetime2)),'{Description_in}','{Limit_in}','{Genre_in}','{Img_in}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Фильм добавлен!");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void filmAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (filmName.Text == null || filmGenre.Text == null || filmYear.Text == null || filmDuration.Text == null || filmAgeLimit.Text == null
                                                 || filmStart.SelectedDate == null || filmEnd.SelectedDate == null || filmDescription.Text == null)
                {
                    MessageBox.Show("Заполните все поля!");
                }
                else
                {
                    addNewFilm();
                    this.Owner.Activate();
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void filmStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filmEnd.DisplayDateStart = new DateTime(filmStart.SelectedDate.Value.Year, filmStart.SelectedDate.Value.Month, filmStart.SelectedDate.Value.Day);

        }

        private void filmEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filmEnd.DisplayDateStart = new DateTime(filmStart.SelectedDate.Value.Year, filmStart.SelectedDate.Value.Month, filmStart.SelectedDate.Value.Day);

        }
    }
}
    