using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Data.SqlClient;
using Cinema_Ticket.Connection;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для AddSeance.xaml
    /// </summary>
    public partial class AddSeance : Window
    {
        List<string> HallNameList;
        List<string> FilmNameList;
        List<string> SeanceTimeList;
        List<Film> AllFilmList;

        public AddSeance()
            {
            InitializeComponent();
             }

    public AddSeance(List<string> nameHallList, List<string> nameFilmList, List<string> seanceTimeList, List<Film> allFilmList)
        {
            InitializeComponent();
            AllFilmList = allFilmList;
            HallNameList = nameHallList;
            seanceHall.ItemsSource = HallNameList;
            FilmNameList = nameFilmList;
            seanceFilm.ItemsSource = FilmNameList;
            SeanceTimeList = seanceTimeList;
            seanceTime.ItemsSource = SeanceTimeList;
            seanceDate.DisplayDateStart = DateTime.Now;

        }

        private void seanceAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (seanceDate.SelectedDate == null || seanceTime.Text == null || seanceHall.Text == null || seanceFilm.Text == null)
                {
                    MessageBox.Show("Заполните все поля!");
                }
                else
                {
                    addSeance();
                    this.Owner.Activate();
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void addSeance()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Date_in = seanceDate.SelectedDate;
                    var Time_in = seanceTime.Text;
                    var Hall_in = seanceHall.Text;
                    var Film_in = seanceFilm.Text;
                    string query = $@"SELECT COUNT(*) from Seance  where Dataseance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        MessageBox.Show("Такой сеанс уже существует!");
                    }
                    else 
                    {
                        query = $@"insert into Seance(DataSeance,TimeSeance, IDHall, IDFilm) values ((cast('{Date_in}' as datetime2)),'{Time_in}',(select id from Hall where Name = '{Hall_in}') ,(select id from Film where Name = '{Film_in}'))";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        query = $@"select id from Place  where IDHall ='{Hall_in}' and Rownumber = '1' and Place = '1'";
                        cmd = new SqlCommand(query, connection);
                        object FirstPlace = cmd.ExecuteScalar();
                        int b = Convert.ToInt32(FirstPlace);
                        query = $@"select Capacity from Hall where Name = '{Hall_in}'";
                        cmd = new SqlCommand(query, connection);
                        result = cmd.ExecuteScalar();
                        a = Convert.ToInt32(result);
                        for (int i=0; i<a;)
                        {
                            query = $@"insert into PlaceStatus(IDSeance,IDPlace,Status) values ((select id from Seance where Dataseance =(cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm = (select id from Film where Name = '{Film_in}')),'{b}','0')";
                            cmd = new SqlCommand(query, connection);
                            cmd.ExecuteNonQuery();
                            i++;
                            b++;
                        }
                        MessageBox.Show("Сеанс добавлен!");
                    }
  
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
