using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows.Media;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using Cinema_Ticket.Connection;
using Brushes = System.Windows.Media.Brushes;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {





        #region Переменные || Полностью готово



        public static List<Employee> allEmployeeList = new List<Employee>();
        public static List<Employee> sortEmployeeList = new List<Employee>();
        public static List<Film> allFilmList = new List<Film>();
        public static List<Film> sortFilmList = new List<Film>();
        public static List<string> FilmNameList = new List<string>();
        public static List<Seance> allSeanceList = new List<Seance>();
        public static List<Seance> sortSeanceList = new List<Seance>();
        public static List<string> SeanceTimeList = new List<string>();
        public static List<Hall> allHallList = new List<Hall>();
        public static List<string> HallNameList = new List<string>();
        public static List<Post> allPostList = new List<Post>();
        public static List<string> PostNameList = new List<string>();
        public static List<Genre> allGenreList = new List<Genre>();
        public static List<string> GenreNameList = new List<string>();
        public static BindingList<Ticket> allTicketList = new BindingList<Ticket>();
        public static BindingList<int> RawsList = new BindingList<int>();
        public static BindingList<int> PlaceList = new BindingList<int>();
        public static BindingList<Ticket> sortTicketList = new BindingList<Ticket>();
        int userID;



        #endregion





        #region Инициализация || Полностью готово



        public AdminWindow()
        {
            InitializeComponent();

            filmStartDate.Text = Convert.ToString(DateTime.Today);
            filmEndDate.Text = Convert.ToString(DateTime.Today);
            GetEmployee();
            employeeTable.ItemsSource = allEmployeeList;
            GetFilm();
            filmTable.ItemsSource = allFilmList;
            GetSeance();
            allSeanceTime.ItemsSource = SeanceTimeList;
            seanceTable.ItemsSource = allSeanceList;
            GetHall();
            allSeanceHall.ItemsSource = HallNameList;
            GetPost();
            GetGenre();

        }

        public AdminWindow(int ID, string FIO)
        {
            InitializeComponent();

            filmStartDate.Text = Convert.ToString(DateTime.Today);
            filmEndDate.Text = Convert.ToString(DateTime.Today);
            userID = ID;
            fio.Content = FIO;
            GetEmployee();
            employeeTable.ItemsSource = allEmployeeList;
            GetFilm();
            filmTable.ItemsSource = allFilmList;
            ticketFilmList.ItemsSource = FilmNameList;
            GetSeance();
            seanceTable.ItemsSource = allSeanceList;
            allSeanceTime.ItemsSource = SeanceTimeList;
            ticketTimeList.ItemsSource = SeanceTimeList;
            GetHall();
            allSeanceHall.ItemsSource = HallNameList;
            ticketHallList.ItemsSource = HallNameList;
            GetPost();
            GetGenre();
            GetTicketInfo();
            ticketTable.ItemsSource = allTicketList;
        }



        #endregion





        #region Общее || Полностью готово



            #region Функции || Полностью готово

        private void products_SelectionChanged(object sender, MouseButtonEventArgs e) // Обновление при переключении между вкладками || Полностью готово
        {
            GetSeance();
            RefreshSeance();
            GetFilm();
            RefreshFilm();
            GetEmployee();
            RefreshEmployee();
        }

        #endregion



            #region Кнопки || Полностью готово

        private void Exit(object sender, RoutedEventArgs e) // Выход из текущего аккаунта || Полностью готово
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }

        #endregion



        #endregion
        #region Продажа || Полностью готово



        #region Функции || Полностью готово

        public void CheckRaw() // Получение списка рядов || Полностью готово
        {
            RawsList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Hall_in = ticketHallList.SelectedItem.ToString();
                    var Date_in = ticketDateChoose.SelectedDate;
                    var Film_in = ticketFilmList.SelectedItem.ToString();
                    var Time_in = ticketTimeList.SelectedItem.ToString();
                    object FreePlace;
                    int sumFreePlace;
                    string query = $@"select DISTINCT RowNumber from Place where IDHall = (select id from Hall where Name = '{Hall_in}')  order by ROWNUMBER";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        sumFreePlace = 0;
                        int id = Convert.ToInt32(row["RowNumber"]);
                        string CheckRow = $@"select id from Place where Rownumber = '{id}' and IDHall = (select id from Hall where Name = '{Hall_in}')";
                        cmd = new SqlCommand(CheckRow, connection);
                        DataTable crdt = new DataTable();
                        crdt.Load(cmd.ExecuteReader());
                        foreach (DataRow crow in crdt.Rows)
                        {
                            int crid = Convert.ToInt32(crow["ID"]);
                            string CheckPlace = $@"SELECT count(*) from PlaceStatus where IDSeance = (select id from Seance  where Dataseance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDPlace = '{crid}') and Status = '0'";
                            cmd = new SqlCommand(CheckPlace, connection);
                            FreePlace = cmd.ExecuteScalar();
                            int a = Convert.ToInt32(FreePlace);
                            if (a != 0)
                            {
                                sumFreePlace += a;
                                break;
                            }
                        }
                        if (sumFreePlace != 0)
                        {
                            RawsList.Add(id);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetPlace() // Получение списка мест || Полностью готово
        {
            PlaceList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Hall_in = ticketHallList.SelectedItem;
                    var Date_in = ticketDateChoose.SelectedDate;
                    var Film_in = ticketFilmList.SelectedItem.ToString();
                    var Time_in = ticketTimeList.SelectedItem.ToString();
                    var Row_in = Convert.ToInt32(Raw.SelectedItem);
                    string query = $@"select IDPlace from PlaceStatus where IDSeance = (select id from Seance  where Dataseance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')) and Status = '0'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Convert.ToInt32(row["IDPlace"]);
                        string CheckPlace = $@"select Place from Place where IDHall = (select id from Hall where Name = '{Hall_in}') and RowNumber = '{Row_in}' and ID = '{id}' order by Place";
                        cmd = new SqlCommand(CheckPlace, connection);
                        DataTable cpdt = new DataTable();
                        cpdt.Load(cmd.ExecuteReader());
                        foreach (DataRow cprow in cpdt.Rows)
                        {
                            int cpid = Convert.ToInt32(cprow["Place"]);
                            PlaceList.Add(cpid);
                        }
                    }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SellTicket() // Продажа билетов || Полностью готово
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Date_in = ticketDateChoose.SelectedDate;
                    var Time_in = ticketTimeList.SelectedItem.ToString();
                    var Hall_in = ticketHallList.SelectedItem.ToString();
                    var Film_in = ticketFilmList.SelectedItem.ToString();
                    var Row_in = Convert.ToInt32(Raw.SelectedItem);
                    var Place_in = Convert.ToInt32(Place.SelectedItem);
                    var Employee_in = Convert.ToInt32(userID);
                    string query = $@"SELECT COUNT(*) from PlaceStatus where IDSeance = (select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm =(select id from Film where Name = '{Film_in}')) and IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')) and Status = '0'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    var result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a == 0)
                    {
                        MessageBox.Show("Билет уже продан!");
                    }
                    else
                    {
                        query = $@"select MAX(Ticketnumber) from Ticket";
                        cmd = new SqlCommand(query, connection);
                        result = cmd.ExecuteScalar();
                        if (result == DBNull.Value)
                        {
                            a = 1;
                        }
                        else
                        {
                            a = Convert.ToInt32(result);
                            a++;
                        }
                        query = $@"insert into Ticket(IDSeance, IDPlaceStatus, IDEmployee, Ticketnumber) values((select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')),(select id from PlaceStatus where IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')) and IDSeance = (select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}'))) ,'{Employee_in}', '{a}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        query = $@"update PlaceStatus set status = '1' where IDSeance = (select id from Seance where Dataseance = '{Date_in}' and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm =(select id from Film where Name = '{Film_in}')) and IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}'))";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Билет продан успешно!");
                    }
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void PrintTicketInfo() // Заполнение билета для печати || Полностью готово
        {
            Ticket curr_ticket = allTicketList[allTicketList.Count - 1];
            ticketNumberEnter.Content = curr_ticket.Number.ToString();
            ticketCostEnter.Content = curr_ticket.Cost.ToString();
            ticketDateEnter.Content = Convert.ToDateTime(curr_ticket.seance.seanceDate).ToString("D");
            ticketTimeEnter.Content = curr_ticket.seance.seanceTime;
            ticketHallEnter.Content = curr_ticket.place.Hall.Name.ToString();
            ticketRawEnter.Content = curr_ticket.place.Raw.ToString();
            ticketPlaceEnter.Content = curr_ticket.place.Number.ToString();
            ticketTypeEnter.Content = curr_ticket.place.Type.ToString();
            ticketFilmEnter.Content = curr_ticket.seance.seanceFilm.Name.ToString();
            ticketDurationEnter.Content = curr_ticket.seance.seanceFilm.Duration;
            ticketLimitEnter.Content = curr_ticket.seance.seanceFilm.AgeLimit + " лет";
            ticketEmployeeEnter.Content = fio.Content;
        }

        #endregion



        #region Комбобоксы || Полностью готово

        public void GetFilmWithDate() // Получение фильма из базы данных || Полностью готово
        {
            FilmNameList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Date_in = ticketDateChoose.SelectedDate;
                    string query = $@"select Name from Film where ID in (select IDFilm from Seance where DataSeance = CAST('{Date_in}' as datetime2))";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Name"].ToString();
                        FilmNameList.Add(name);
                    }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetTimeWithDateFilm() // Получение времени сеанса из базы данных || Полностью готово
        {
            SeanceTimeList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Date_in = ticketDateChoose.SelectedDate;
                    var Film_in = ticketFilmList.SelectedItem;
                    string query = $@"select Timeseance from Seance where DataSeance = CAST('{Date_in}' as datetime2) and IDFilm in (select  ID FROM Film WHERE Name = '{Film_in}')";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Timeseance"].ToString();
                        SeanceTimeList.Add(name);
                    }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetHallWithSeance() // Получение номера зала из базы данных || Полностью готово
        {
            HallNameList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Date_in = ticketDateChoose.SelectedDate;
                    var Film_in = ticketFilmList.SelectedItem.ToString();
                    var Time_in = ticketTimeList.SelectedItem.ToString();
                    string query = $@"select Name from Hall where ID in (select IDhall from Seance where DataSeance = CAST('{Date_in}' as datetime2) and IDFilm in (select  ID FROM Film WHERE Name = '{Film_in}') and Timeseance = CAST('{Time_in}' as time))";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Name"].ToString();
                        HallNameList.Add(name);
                    }
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion



        #region Кнопки || Полностью готово

        private void addTicketButton_Click(object sender, RoutedEventArgs e) // Продажа билета и занесения в поля для печати/отправки на почту || Полностью готово
        {
            SellTicket();
            GetTicketInfo();
            ticketTable.ItemsSource = allTicketList;
            PrintTicketInfo();

            Place.IsEnabled = false;
            Raw.IsEnabled = false;
            ticketHallList.IsEnabled = false;
            ticketTimeList.IsEnabled = false;
            ticketFilmList.IsEnabled = false;
            ticketHallList.ItemsSource = null;
            ticketDateChoose.SelectedDate = null;


            Place.ItemsSource = null;
            Raw.ItemsSource = null;
            ticketHallList.ItemsSource = null;
            ticketTimeList.ItemsSource = null;
            ticketFilmList.ItemsSource = null;
        }

        private void printButton_Click(object sender, RoutedEventArgs e) // Печать/отправка на почту билета || Полностью готово
        {
            try
            {
                Bitmap printscreen = new Bitmap(322, 367);
                Graphics graphics = Graphics.FromImage(printscreen as System.Drawing.Image);
                graphics.CopyFromScreen(1050, 352, 0, 0, printscreen.Size);
                printscreen.Save("Ticket.png", System.Drawing.Imaging.ImageFormat.Png);
                using (MailMessage mess = new MailMessage())
                {
                    SmtpClient client = new SmtpClient("smtp.mail.ru", Convert.ToInt32(587))
                    {
                        Credentials = new NetworkCredential("artem-nefedov13-1999@mail.ru", "rg1FGqutT9BEZjZsastb"),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    mess.From = new MailAddress("artem-nefedov13-1999@mail.ru");
                    mess.To.Add(new MailAddress("artemnefedov11@gmail.com"));
                    mess.Subject = "Билет на сеанс";
                    mess.IsBodyHtml = true;
                    mess.SubjectEncoding = Encoding.UTF8;
                    mess.Body = $"<html><head></head><body><p>Билет на сеанс</br></p></body>";

                    try
                    {
                        mess.Attachments.Add(new Attachment("Ticket.png"));
                    }
                    catch { }

                    client.Send(mess);
                    mess.Dispose();
                    client.Dispose();
                    MessageBox.Show("Отправлено");

                    ticketNumberEnter.Content = "";
                    ticketCostEnter.Content = "";
                    ticketDateEnter.Content = "";
                    ticketTimeEnter.Content = "";
                    ticketHallEnter.Content = "";
                    ticketRawEnter.Content = "";
                    ticketPlaceEnter.Content = "";
                    ticketTypeEnter.Content = "";
                    ticketFilmEnter.Content = "";
                    ticketDurationEnter.Content = "";
                    ticketLimitEnter.Content = "";
                    ticketEmployeeEnter.Content = "";

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
        }

        #endregion



        #region Триггеры || Полностью готово

        private void ticketDateChoose_CalendarClosed(object sender, RoutedEventArgs e) // Разблокировка и заполнение чекбокса с фильмами  || Полностью готово
        {
            if (ticketDateChoose.SelectedDate != null)
            {
                ticketFilmList.IsEnabled = true;
                GetFilmWithDate();
                ticketFilmList.ItemsSource = FilmNameList;
                ticketTimeList.IsEnabled = false;
                SeanceTimeList.Clear();
                ticketHallList.IsEnabled = false;
                HallNameList.Clear();
                Raw.IsEnabled = false;
                RawsList.Clear();
                Place.IsEnabled = false;
                PlaceList.Clear();
            }
        }

        private void ticketFilmList_SelectionChanged(object sender, RoutedEventArgs e) // Разблокировка и заполнение чекбокса с временем  || Полностью готово
        {
            if (ticketFilmList.SelectedValue != null && ticketDateChoose.SelectedDate != null)
            {
                ticketTimeList.IsEnabled = true;
                GetTimeWithDateFilm();
                ticketTimeList.ItemsSource = SeanceTimeList;
                ticketHallList.IsEnabled = false;
                HallNameList.Clear();
                Raw.IsEnabled = false;
                RawsList.Clear();
                Place.IsEnabled = false;
                PlaceList.Clear();
            }
        }

        private void ticketTimeList_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с залами  || Полностью готово
        {
            if (ticketTimeList.SelectedValue != null && ticketFilmList.SelectedValue != null && ticketDateChoose.SelectedDate != null)
            {
                ticketHallList.IsEnabled = true;
                GetHallWithSeance();
                ticketHallList.ItemsSource = HallNameList;
                Raw.IsEnabled = false;
                RawsList.Clear();
                Place.IsEnabled = false;
                PlaceList.Clear();
            }
        }

        private void ticketHallList_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с рядами  || Полностью готово
        {
            if (ticketHallList.SelectedValue != null && ticketTimeList.SelectedValue != null && ticketFilmList.SelectedValue != null && ticketDateChoose.SelectedDate != null)
            {
                Raw.IsEnabled = true;
                CheckRaw();
                Raw.ItemsSource = RawsList;
                Place.IsEnabled = false;
                PlaceList.Clear();
            }
        }

        private void Raw_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с местами  || Полностью готово
        {
            if (Raw.SelectedValue != null && ticketHallList.SelectedValue != null && ticketTimeList.SelectedValue != null && ticketFilmList.SelectedValue != null && ticketDateChoose.SelectedDate != null)
            {
                Place.IsEnabled = true;
                GetPlace();
                Place.ItemsSource = PlaceList;
            }
        }

        #endregion



        #endregion





        #region Билеты || Полностью готово



        #region Функции || Полностью готово

        public void GetTicketInfo() // Получение списка билетов || Полностью готово
        {
            allTicketList.Clear();

            Ticket cur_ticket;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "Select Ticket.ID, Ticket.TicketNumber, Seance.dataSeance DateSeance,  Seance.timeSeance,Film.name as FilmName, Film.duration FilmDuration, Film.ageLimit FilmLimit, Genre.name Genre,Hall.name Hall, Place.rowNumber RowNumber,Place.place, Typeplace.name typePlace, Typeplace.cost Cost from Ticket inner join Seance on Ticket.IDSeance = Seance.ID inner join Film on Seance.IDFilm = Film.ID inner join Genre on Film.IDGenre = Genre.ID inner join PlaceStatus on Ticket.IDPlaceStatus = PlaceStatus.ID inner join Place on PlaceStatus.IDPlace = Place.ID inner join TypePlace on Place.IDTypePlace = Typeplace.ID inner join Hall on Place.IDHall = Hall.ID inner join Employee on Ticket.IDEmployee = Employee.ID inner join Post on Employee.IDPost = Post.ID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        int number = Convert.ToInt32(row["ticketNumber"]);
                        int cost = Convert.ToInt32(row["Cost"]);
                        string dateSeance = Convert.ToDateTime(row["DateSeance"]).ToString("D");
                        string timeSeance = row["TimeSeance"].ToString();
                        string hallPlace = row["Hall"].ToString();
                        int rowPlace = Convert.ToInt32(row["RowNumber"]);
                        int placeNumber = Convert.ToInt32(row["Place"]);
                        string typePlace = (row["typePlace"]).ToString();
                        string filmName = (row["FilmName"]).ToString();
                        string filmDuration = (row["FilmDuration"]).ToString();
                        string filmLimit = (row["FilmLimit"]).ToString();

                        Hall hall = new Hall(hallPlace);
                        Film film = new Film(filmName, filmDuration, filmLimit);
                        Place place = new Place(rowPlace, placeNumber, hall, typePlace, cost);
                        Seance seance = new Seance(dateSeance, timeSeance, film);


                        cur_ticket = new Ticket(id, number, seance, place, fio.Content.ToString());
                        allTicketList.Add(cur_ticket);
                        ;

                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteCurrentTicket() // Возврат проданного билета || Полностью готово
        {
            if (ticketTable.SelectedIndex != -1)
            {
                int index = allTicketList.IndexOf((Ticket)ticketTable.SelectedItem);
                Ticket cur_ticket = allTicketList[index];
                try
                {
                    using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                    {
                        connection.Open();
                        var Number_in = cur_ticket.Number;
                        var Date_in = Convert.ToDateTime(cur_ticket.seance.seanceDate);
                        var Time_in = cur_ticket.seance.seanceTime;
                        var Hall_in = cur_ticket.place.Hall.Name;
                        var Film_in = cur_ticket.seance.seanceFilm.Name;
                        var Row_in = cur_ticket.place.Raw;
                        var Place_in = cur_ticket.place.Number;
                        string query = $@"SELECT COUNT(*) from PlaceStatus where IDSeance = (select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm =(select id from Film where Name = '{Film_in}')) and IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}'))";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        object result = cmd.ExecuteScalar();
                        int a = Convert.ToInt32(result);
                        if (a != 0)
                        {
                            query = $@"delete from Ticket where IDSeance = (select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}')) and IDPlaceStatus = (select id from PlaceStatus where IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDSeance = (select id from Seance where Dataseance = '{Date_in}'  and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}'))))";
                            cmd = new SqlCommand(query, connection);
                            cmd.ExecuteNonQuery();
                            query = $@"update PlaceStatus set status = '0' where IDSeance = (select id from Seance where Dataseance = '{Date_in}' and TimeSeance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm =(select id from Film where Name = '{Film_in}')) and IDPlace = (select id from Place where Rownumber = '{Row_in}' and Place = '{Place_in}' and IDHall = (select id from Hall where Name = '{Hall_in}'))";
                            cmd = new SqlCommand(query, connection);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Билет отменен!");
                        }
                        else
                        {
                            MessageBox.Show("Такого билета не существует!");
                        }

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion



        #region Кнопки || Полностью готово

        private void SearchButton_Click(object sender, RoutedEventArgs e) // Поиск по билетам || Полностью готово
        {
            try
            {
                if (Search.Text == "")
                {
                    allTicketList.Clear();
                    GetTicketInfo();
                    ticketTable.ItemsSource = allTicketList;
                    MessageBox.Show("Введите данные!");
                }
                else
                {
                    sortTicketList.Clear();

                    if (Search.Text != "" && !Search.Text.ToLower().Contains("w") && !Search.Text.ToLower().Contains("^") && !Search.Text.ToLower().Contains("s") && !Search.Text.ToLower().Contains("d") && !Search.Text.Contains("["))
                    {
                        string s = Search.Text;
                        Regex regex = new Regex(@"^" + s + @"(\w)*", RegexOptions.IgnoreCase);
                        MatchCollection matches;
                        foreach (var p in allTicketList)
                        {
                            matches = regex.Matches(p.Number.ToString());

                            foreach (Match match in matches)
                            {
                                sortTicketList.Add(p);
                                ticketTable.ItemsSource = sortTicketList;

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ничего не найдено!");
                    }

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ничего не найдено!");
                Search.Text = "";
            }
        }

        private void deleteTicket_Click(object sender, RoutedEventArgs e) // Кнопка удаления билета || Полностью готово
        {
            DeleteCurrentTicket();
            GetTicketInfo();
            ticketTable.ItemsSource = allTicketList;
        }

        #endregion



        #endregion









        #region Сеанс || Полностью готово



        #region Функции || Полностью готово

        public void GetSeance() // Получение информации о сеансе || Полностью готово
        {
            allSeanceList.Clear();
            Seance cur_seance;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "select Seance.ID as ID, Seance.DataSeance DataSeance, Timeseance, Hall.name HallName, Hall.Capacity HallCapacity, Film.name FilmName from Seance inner join Film on Seance.IDFilm = Film.ID inner join Hall on Seance.IDHall = Hall.ID order by Seance.ID;";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        string date = Convert.ToDateTime(row["DataSeance"]).ToString("D");
                        DateTime date2 = Convert.ToDateTime(row["DataSeance"]);
                        string time = row["TimeSeance"].ToString();
                        string hall = row["hallName"].ToString();
                        string filmName = row["FilmName"].ToString();
                        int seanceID = Convert.ToInt32(row["ID"]);
                        Film film = new Film(filmName);
                        string capacity = row["HallCapacity"].ToString();
                        cur_seance = new Seance(seanceID, date, date2, time, hall, film, capacity);
                        allSeanceList.Add(cur_seance);
                        if (SeanceTimeList.Contains(time)) //Проверка на дубликаты
                        {

                        }
                        else
                        {
                            SeanceTimeList.Add(time);
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetHall() // Получение информации о залах || Полностью готово
        {
            Hall hall;
            allHallList.Clear();
            HallNameList.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "SELECT * FROM Hall order by ID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Name"].ToString();
                        string capacity = row["Capacity"].ToString();

                        hall = new Hall(name, capacity);
                        allHallList.Add(hall);
                        HallNameList.Add(name);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteCurrentSeance() // Удаление выбранного сеанса || Полностью готово
        {
            var select = seanceTable.SelectedItem;
            Seance cur_seance = (Seance)select;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Hall_in = cur_seance.seanceHall;
                    var Date_in = cur_seance.seanceDate2;
                    var Film_in = cur_seance.seanceFilm.Name;
                    var Time_in = cur_seance.seanceTime;
                    string query = $@"SELECT COUNT(*) from Seance where DataSeance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm = '{Film_in}'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        query = $@"delete from Seance where DataSeance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDFilm = (select id from Film where Name = '{Film_in}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Сеанс удален!");
                    }
                    else
                    {
                        MessageBox.Show("Такого сеанса не существует!");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshSeance() // Функция обновления || Полностью готово
        {
            seanceTable.ItemsSource = null;
            allSeanceTime.ItemsSource = null;
            seanceTable.ItemsSource = allSeanceList;
            allSeanceTime.ItemsSource = SeanceTimeList;
        } 

        #endregion



            #region Кнопки || Полностью готово

        private void addSeanceBtn_Click(object sender, RoutedEventArgs e) // Открытие окна добавления сеанса || Полностью готово
        {
            GetHall();
            AddSeance addSeance = new AddSeance(HallNameList, FilmNameList, SeanceTimeList, allFilmList);
            addSeance.Owner = this;
            bool? dialogResult = addSeance.ShowDialog();
            switch (dialogResult)
            {
                default:
                    GetSeance();
                    RefreshSeance();
                    break;
            }
        }

        private void deleteSeance_Click(object sender, RoutedEventArgs e) // Удаление выбранного сеанса || Полностью готово
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись? Это действие нельзя будет отменить", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteCurrentSeance();
                GetSeance();
                RefreshSeance();
            }
        }

        #endregion



            #region Триггеры || 50%

        private void allSeanceTime_KeyDown(object sender, KeyEventArgs e) // ???
        {
            allSeanceTime.IsDropDownOpen = true;
        }

        private void allSeanceHall_KeyDown(object sender, KeyEventArgs e) // ???
        {
            allSeanceHall.IsDropDownOpen = true;
        }

        private void seanceTable_MouseDoubleClick(object sender, MouseButtonEventArgs e) // Открытие окна редактирования сеанса || Полностью готово
        {
            if (seanceTable.SelectedIndex != -1)
            {
                GetHall();
                ChangeSeance changeSeance = new ChangeSeance(allSeanceList[seanceTable.SelectedIndex], HallNameList, FilmNameList, SeanceTimeList);
                changeSeance.Owner = this;
                bool? dialogResult = changeSeance.ShowDialog();
                switch (dialogResult)
                {
                    default:
                        GetSeance();
                        RefreshSeance();
                        break;
                }
            }
        }

        private void showSeanceList_Click(object sender, RoutedEventArgs e) // Сортировка по критериям || Полностью готово
        {
            GetSeance();
            sortSeanceList.Clear();
            foreach (var l in allSeanceList)
            {
                if (seancceDateChoose.SelectedDate != null && allSeanceTime.Text != "" && allSeanceHall.Text != "")
                {
                    if (Convert.ToDateTime(l.seanceDate) == seancceDateChoose.SelectedDate && l.seanceTime == allSeanceTime.Text && l.seanceHall == allSeanceHall.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate != null && allSeanceTime.Text != "" && allSeanceHall.Text == "")
                {
                    if (Convert.ToDateTime(l.seanceDate) == seancceDateChoose.SelectedDate && l.seanceTime == allSeanceTime.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate != null && allSeanceTime.Text == "" && allSeanceHall.Text != "")
                {
                    if (Convert.ToDateTime(l.seanceDate) == seancceDateChoose.SelectedDate && l.seanceHall == allSeanceHall.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate == null && allSeanceTime.Text != "" && allSeanceHall.Text != "")
                {
                    if (l.seanceTime == allSeanceTime.Text && l.seanceHall == allSeanceHall.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate != null && allSeanceTime.Text == "" && allSeanceHall.Text == "")
                {
                    if (Convert.ToDateTime(l.seanceDate) == seancceDateChoose.SelectedDate)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate == null && allSeanceTime.Text != "" && allSeanceHall.Text == "")
                {
                    if (l.seanceTime == allSeanceTime.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else if (seancceDateChoose.SelectedDate == null && allSeanceTime.Text == "" && allSeanceHall.Text != "")
                {
                    if (l.seanceHall == allSeanceHall.Text)
                    {
                        sortSeanceList.Add(l);
                    }
                    else
                    {

                    }
                }
                else
                {
                    sortSeanceList.Add(l);
                }

            }
            seanceTable.ItemsSource = sortSeanceList;
        }

        #endregion



        #endregion





        #region Фильмы || Полностью готово



            #region Функции || Полностью готово

        public void GetGenre() // Получение списка жанров || Полностью готово
        {
            Genre genre;
            allGenreList.Clear();
            GenreNameList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "Select Name from Genre";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["Name"].ToString();

                        genre = new Genre(name);
                        allGenreList.Add(genre);
                        GenreNameList.Add(name);

                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetFilm() // Получение списка фильмов || Полностью готово
        {
            allFilmList.Clear();
            FilmNameList.Clear();

            Film curr_film;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "select Film.ID as ID ,Film.Name, Genre.Name as Genre, year, duration, ageLimit, startRelease, EndRelease, Img, description from Film  inner join Genre  on Film.IDGenre = Genre.ID;";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    foreach (DataRow row in dt.Rows)
                    {
                        int id = Convert.ToInt32(row["ID"]);
                        string name = row["Name"].ToString();
                        string genre = row["genre"].ToString();
                        string year = row["year"].ToString();
                        string duration = row["duration"].ToString();
                        string ageLimit = row["agelimit"].ToString();
                        string description = row["description"].ToString();
                        string startRelease = Convert.ToDateTime(row["startrelease"]).ToString("D");
                        string endRelease = Convert.ToDateTime(row["endrelease"]).ToString("D");
                        byte[] img = (byte[])(row["Img"]);

                        curr_film = new Film(id, name, genre, year, duration, ageLimit, startRelease, endRelease, img, description);
                        allFilmList.Add(curr_film);
                        FilmNameList.Add(name);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SortFilm() // Сортировка по дате || Полностью готово
        {
            GetFilm();
            if (filmStartDate.SelectedDate <= filmEndDate.SelectedDate)
            {
                sortFilmList.Clear();
                foreach (var l in allFilmList)
                {
                    if (Convert.ToDateTime(l.StartreleaseDate) <= filmStartDate.SelectedDate && Convert.ToDateTime(l.EndreleaseDate) >= filmEndDate.SelectedDate)
                    {
                        sortFilmList.Add(l);
                    }
                }
                filmTable.ItemsSource = sortFilmList;

            }
            else if (filmStartDate.SelectedDate >= filmEndDate.SelectedDate)
            {
                MessageBox.Show("Неверный период!");
                filmStartDate.Text = Convert.ToString(DateTime.Today);
                filmEndDate.Text = Convert.ToString(DateTime.Today);
            }
            else
            {
                filmStartDate.Text = Convert.ToString(DateTime.Today);
                filmEndDate.Text = Convert.ToString(DateTime.Today);
            }
        }

        public void DeleteFilm() // Удаление выбранного фильма || Полностью готово
        {
            var select = filmTable.SelectedItem;
            Film cur_film = (Film)select;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Id_in = cur_film.ID;
                    var Name_in = cur_film.Name;
                    var Genre_in = cur_film.Genre;
                    var Year_in = cur_film.Year;
                    string query = $@"SELECT COUNT(*) from Film where Name = '{Name_in}' and IDGenre = (select id from Genre where name = '{Genre_in}') and Year = '{Year_in}'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        query = $@"delete from Film  where Name = '{Name_in}' and IDGenre = (select id from Genre where name = '{Genre_in}') and Year = '{Year_in}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Фильм удален!");
                    }
                    else
                    {
                        MessageBox.Show("Такого фильма не существует!");
                    }
                }

            }

            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshFilm() // Функция обновления || Полностью готово
        {
            filmTable.ItemsSource = null;
            filmTable.ItemsSource = allFilmList;
        }

        #endregion



            #region Кнопки || Полностью готово

        private void Button_Click(object sender, RoutedEventArgs e) // Открытие окна добавления фильма || Полностью готово
        {
            GetGenre();
            AddFilm addFilm = new AddFilm(GenreNameList);
            addFilm.Owner = this;
            bool? dialogResult = addFilm.ShowDialog();
            switch (dialogResult)
            {
                default:
                    GetFilm();
                    RefreshFilm();
                    break;
            }
        }

        private void deleteFilm_Click(object sender, RoutedEventArgs e) // Удаление выбранного сеанса || Полностью готово
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись? Это действие нельзя будет отменить", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteFilm();
                GetFilm();
                RefreshFilm();
            }
        }

        #endregion



            #region Триггеры || Полностью готово

        private void filmTable_MouseDoubleClick(object sender, MouseButtonEventArgs e) // Открытие окна редактирования фильма || Полностью готово
        {
            if (filmTable.SelectedIndex != -1)
            {
                FilmWindow film = new FilmWindow(allFilmList[filmTable.SelectedIndex]);
                bool? dialogResult = film.ShowDialog();
                switch (dialogResult)
                {
                    default:
                        GetFilm();
                        RefreshFilm();
                        break;
                }
            }
        }

        private void filmStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Сортировка по выбранной дате || Полностью готово
        {
            SortFilm();
        }

        private void filmEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e) // Сортировка по выбранной дате || Полностью готово
        {
            SortFilm();
        }

        #endregion



        #endregion





        #region Пользователи || Полностью готово



            #region Функции || Полностью готово

        public void GetEmployee() // Получение списка пользователей || Полностью готово
        {
            allEmployeeList.Clear();
            Employee cur_employee;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "select Login, Password, Employee.ID, name,secondname,surname,phoneNumber,birthday,dateOfEnrollment,positionName from Employee inner join Post on IDPost = Post.ID left outer join Users on Employee.Id = IDEmployee";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        int ID = Convert.ToInt32(row["ID"]);

                        string name = row["name"].ToString();
                        string secondname = row["secondName"].ToString();
                        string surname = row["surname"].ToString();
                        string fio = surname + " " + name.Substring(0, 1) + ". " + secondname.Substring(0, 1) + ".";
                        string birthday = Convert.ToDateTime(row["birthday"]).ToString("D");
                        string startDay = Convert.ToDateTime(row["dateOfEnrollment"]).ToString("D");
                        string phone = row["phoneNumber"].ToString();
                        string post = row["positionName"].ToString();
                        string login = (row["Login"]).ToString();
                        string password = (row["Password"]).ToString();

                        cur_employee = new Employee(ID, name, secondname, surname, fio, birthday, startDay, phone, post, login, password);
                        allEmployeeList.Add(cur_employee);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void GetPost() // Получение списка уровней прав || Полностью готово
        {
            Post post;
            allPostList.Clear();
            PostNameList.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "Select PositionName, Accesslevel from Post";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = row["PositionName"].ToString();
                        string accessLevel = row["AccessLevel"].ToString();

                        post = new Post(name, accessLevel);
                        allPostList.Add(post);
                        PostNameList.Add(name);

                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void searchEmployee(object sender, RoutedEventArgs e) // Поиск пользователей || Полностью готово
        {
            try
            {
                if (Search.Text == "")
                {
                    allEmployeeList.Clear();
                    GetEmployee();
                    employeeTable.ItemsSource = allEmployeeList;
                    Search.BorderBrush = Brushes.Red;
                    MessageBox.Show("Введите данные!");
                }
                else
                {
                    sortEmployeeList.Clear();

                    if (Search.Text != "" && !Search.Text.ToLower().Contains("w") && !Search.Text.ToLower().Contains("^") && !Search.Text.ToLower().Contains("s") && !Search.Text.ToLower().Contains("d") && !Search.Text.Contains("["))
                    {
                        string s = Search.Text;
                        Regex regex = new Regex(@"^" + s + @"(\w)*", RegexOptions.IgnoreCase);
                        MatchCollection matches;
                        foreach (var p in allEmployeeList)
                        {
                            matches = regex.Matches(p.FIO);

                            foreach (Match match in matches)
                            {
                                sortEmployeeList.Add(p);
                                employeeTable.ItemsSource = sortEmployeeList;

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ничего не найдено!");
                    }
                    Search.BorderBrush = Brushes.Silver;

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ничего не найдено!");
                Search.BorderBrush = Brushes.Silver;
                Search.Text = "";
            }
        }

        public void DeleteCurrentEmployee() // Удаление выбранного пользователя || Полностью готово
        {
            var select = employeeTable.SelectedItem;
            Employee cur_emp = (Employee)select;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var ID_in = cur_emp.ID;
                    string query = $@"SELECT count(*)  from Employee inner join Users on Employee.ID = Users.IDEmployee where Employee.ID = '{ID_in}'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        query = $@"delete from Users where ID = (select Users.ID from Employee inner join Users  on Employee.ID = Users.IDEmployee where Employee.ID = '{ID_in}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        query = $@"delete from Employee where ID = '{ID_in}'";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Пользователь удален!");
                    }
                    else
                    {
                        MessageBox.Show("Такого пользователя не существует!");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshEmployee() // Функция обновления || Полностью готово
        {
            employeeTable.ItemsSource = null;
            employeeTable.ItemsSource = allEmployeeList;
        }

        #endregion



            #region Кнопки || Полностью готово

        private void addEmployeeButton_Click(object sender, RoutedEventArgs e) // Открытие окна добавления пользователя || Полностью готово
        {
            AddEmployee addEmployee = new AddEmployee(PostNameList);
            addEmployee.Owner = this;
            bool? dialogResult = addEmployee.ShowDialog();
            switch (dialogResult)
            {
                default:
                    GetEmployee();
                    RefreshEmployee();
                    break;
            }
        }

        private void deleteEmployee_Click(object sender, RoutedEventArgs e) // Удаление выбранного пользователя || Полностью готово
        {
            if (MessageBox.Show("Вы действительно хотите удалить запись? Это действие нельзя будет отменить", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DeleteCurrentEmployee();
                GetEmployee();
                RefreshEmployee();
            }
        }

        #endregion



            #region Триггеры || Полностью готово

        private void employeeTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)  // Открытие окна редактирования пользователя || Полностью готово
        {
            if (employeeTable.SelectedIndex != -1)
            {
                GetPost();
                ChangeEmployee changeEmployee = new ChangeEmployee(allEmployeeList[employeeTable.SelectedIndex], PostNameList);
                changeEmployee.Owner = this;
                bool? dialogResult = changeEmployee.ShowDialog();
                switch (dialogResult)
                {
                    default:
                        GetEmployee();
                        RefreshEmployee();
                        break;
                }
            }
        }







        #endregion

        #endregion


    }
}
