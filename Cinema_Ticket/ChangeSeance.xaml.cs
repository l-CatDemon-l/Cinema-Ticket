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
    /// Логика взаимодействия для ChangeSeance.xaml
    /// </summary>
    public partial class ChangeSeance : Window
    {
        List<string> HallNameList;
        List<string> FilmNameList;
        List<string> SeanceTimeList;
        Seance seance;
        public ChangeSeance(Seance seance, List<string> nameHallList, List<string> nameFilmList, List<string> seanceTimeList)
        {
            InitializeComponent();
            this.seance = seance;
            HallNameList = nameHallList;
            seanceHall.ItemsSource = HallNameList;
            FilmNameList = nameFilmList;
            seanceFilm.ItemsSource = FilmNameList;
            SeanceTimeList = seanceTimeList;
            seanceTime.ItemsSource = SeanceTimeList;
            seanceDate.Text = seance.seanceDate;
            seanceTime.SelectedItem = seance.seanceTime;
            seanceHall.SelectedItem = seance.seanceHall;
            seanceFilm.SelectedItem = seance.seanceFilm.Name;

        }

        private void seanceSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (seanceDate.SelectedDate == null || seanceTime.Text == null || seanceHall.Text == null || seanceFilm.Text == null)
                {
                    MessageBox.Show("Заполните все поля!");
                }
                else
                {
                    changeSeance();
                    this.Owner.Activate();
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void changeSeance()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var ID_in = seance.seanceID;
                    var Date_in = seanceDate.SelectedDate;
                    var Time_in = seanceTime.Text;
                    var Hall_in = seanceHall.Text;
                    var Film_in = seanceFilm.Text;
                    string query = $@"SELECT COUNT(*) from Seance where DataSeance = (CAST('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from hall where name = '{Hall_in}')";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        MessageBox.Show("Такой сеанс уже существует!");
                    }
                    else
                    {
                        query = $@"update Seance set DataSeance = (CAST('{Date_in}' as datetime2)), Timeseance = '{Time_in}', IDHall = (select id from hall where name = '{Hall_in}'), IDFilm = (select id from Film where Name = '{Film_in}') where ID = '{ID_in}'";
                        cmd = new SqlCommand(query, connection);
                        MessageBox.Show("Сеанс изменен!");
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

