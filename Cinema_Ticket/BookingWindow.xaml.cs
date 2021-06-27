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
    /// Логика взаимодействия для BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {




        #region Переменные || Полностью готово



        public static BindingList<Ticket> allTicketList = new BindingList<Ticket>();
        public static BindingList<Ticket> sortTicketList = new BindingList<Ticket>();
        public static BindingList<Film> allFilmList = new BindingList<Film>();
        public static BindingList<Film> sortFilmList = new BindingList<Film>();
        public static BindingList<string> FilmNameList = new BindingList<string>();
        public static BindingList<Seance> allSeanceList = new BindingList<Seance>();
        public static BindingList<Seance> sortSeanceList = new BindingList<Seance>();
        public static BindingList<string> SeanceTimeList = new BindingList<string>();
        public static BindingList<Hall> allHallList = new BindingList<Hall>();
        public static BindingList<string> HallNameList = new BindingList<string>();
        public static BindingList<int> RawsList = new BindingList<int>();
        public static BindingList<int> PlaceList = new BindingList<int>();
        int userID;



        #endregion





        #region Инициализация || Полностью готово

        public BookingWindow()
        {
            InitializeComponent();
            GetTicketInfo();
            ticketTable.ItemsSource = allTicketList;
            GetFilm();
            ticketFilmList.ItemsSource = FilmNameList;
            GetSeance();
            ticketTimeList.ItemsSource = SeanceTimeList;
            GetHall();
            ticketHallList.ItemsSource = HallNameList;
            ticketEmployeeEnter.Content = "ФИО";
        }

        public BookingWindow(int ID, string FIO)
        {
            InitializeComponent();
            userID = ID;
            fio.Content = FIO;
            ticketEmployeeEnter.Content = FIO;
            GetTicketInfo();
            ticketTable.ItemsSource = allTicketList;
            GetFilm();
            ticketFilmList.ItemsSource = FilmNameList;
            GetSeance();
            ticketTimeList.ItemsSource = SeanceTimeList;
            GetHall();
            ticketHallList.ItemsSource = HallNameList;

        }

        #endregion





        #region Общее || Полностью готово



            #region Функции || ???



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





        #region Продажа || ???



            #region Функции || Закончить 2 неготовые функции

        public void GetSeance() // Получение списка сеансов || Полностью готово
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

        public void GetHall() // Получение списка залов || Полностью готово
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
                            string CheckPlace = $@"SELECT count(*) from PlaceStatus where IDSeance = (select id from Seance  where Dataseance = (cast('{Date_in}' as datetime2)) and Timeseance = '{Time_in}' and IDHall = (select id from Hall where Name = '{Hall_in}') and IDPlace = '{crid}' and Status = '0'";
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

        public void SellTicket() // Продажа билетов || Не готово
        {

     try
     {
         using (OracleConnection connection = new OracleConnection(OracleDatabaseConnection.connection_meneger))
         {
             connection.Open();
             OracleParameter date_in = new OracleParameter
             {
                 ParameterName = "Date_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Date,
                 Value = ticketDateChoose.SelectedDate
             };
             OracleParameter time_in = new OracleParameter
             {
                 ParameterName = "Time_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Varchar2,
                 Value = ticketTimeList.SelectedItem.ToString()
             };
             OracleParameter hall_in = new OracleParameter
             {
                 ParameterName = "Hall_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Varchar2,
                 Value = ticketHallList.SelectedItem.ToString()
             };
             OracleParameter film_in = new OracleParameter
             {
                 ParameterName = "Film_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Varchar2,
                 Value = ticketFilmList.SelectedItem.ToString()
             };
             OracleParameter row_in = new OracleParameter
             {
                 ParameterName = "Row_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Int32,
                 Value = Convert.ToInt32(Raw.SelectedItem)
             };
             OracleParameter place_in = new OracleParameter
             {
                 ParameterName = "Place_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Int32,
                 Value = Convert.ToInt32(Place.SelectedItem)
             };
             OracleParameter emp_in = new OracleParameter
             {
                 ParameterName = "Employee_in",
                 Direction = ParameterDirection.Input,
                 OracleDbType = OracleDbType.Int32,
                 Value = Convert.ToInt32(userID)
             };
             using (OracleCommand command = new OracleCommand("admin.sellTicket"))
             {
                 command.Connection = connection;
                 command.CommandType = CommandType.StoredProcedure;
                 command.Parameters.AddRange(new OracleParameter[] { date_in, time_in, hall_in, film_in, row_in, place_in, emp_in });
                 command.ExecuteNonQuery();
                 MessageBox.Show("Билет продан успешно!");
             }
         }
     }
     catch (OracleException ex)
     {
         MessageBox.Show(ex.Message);
     }

 }

        public void PrintTicketInfo() // Заполнение билета для печати || Не готово
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
            ticketDurationEnter.Content = curr_ticket.seance.seanceFilm.Duration.Substring(0, curr_ticket.seance.seanceFilm.Duration.IndexOf('.')) + " ч " + curr_ticket.seance.seanceFilm.Duration.Substring(curr_ticket.seance.seanceFilm.Duration.IndexOf('.') + 1, 2) + " мин"; ;
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



            #region Кнопки || Протестировать

        private void addTicketButton_Click(object sender, RoutedEventArgs e) // Продажа билета и отправка билета на печать/почту || Протестировать
        {
    SellTicket();
    GetTicketInfo();
    ticketTable.ItemsSource = allTicketList;
    PrintTicketInfo();

    ticketDateChoose.Text = "";
    ticketTimeList.Text = "";
    ticketHallList.Text = "";
    ticketFilmList.Text = "";
    Raw.Text = "";
    Place.Text = "";
}

        private void printButton_Click(object sender, RoutedEventArgs e) // Печать/отправка на почту билета || Протестировать
        {
    try
    {
        Bitmap printscreen = new Bitmap(414, 463);
        Graphics graphics = Graphics.FromImage(printscreen as System.Drawing.Image);
        graphics.CopyFromScreen(1070, 302, 0, 0, printscreen.Size);
        printscreen.Save("D:\\Alesya\\3course\\2sem\\БД\\Курсовая работа\\Screen\\screen.png", System.Drawing.Imaging.ImageFormat.Png);
        using (MailMessage mess = new MailMessage())
        {
            SmtpClient client = new SmtpClient("smtp.mail.ru", Convert.ToInt32(587))
            {
                Credentials = new NetworkCredential("alesya.demidkevich@mail.ru", "nevergiveup-98765"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            mess.From = new MailAddress("alesya.demidkevich@mail.ru");
            mess.To.Add(new MailAddress("alesia.demidkevich@gmail.com"));
            mess.Subject = "Cinema";
            mess.IsBodyHtml = true;
            mess.SubjectEncoding = Encoding.UTF8;
            mess.Body = $"<html><head></head><body><p>Билет на сеанс</br></p></body>";

            try
            {
                mess.Attachments.Add(new Attachment("D:\\Alesya\\3course\\2sem\\БД\\Курсовая работа\\Screen\\screen.png"));
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
    catch (Exception exception)
    {
        MessageBox.Show("Error");
    }
}

        #endregion



            #region Триггеры || Полностью готово

        private void ticketDateChoose_CalendarClosed(object sender, RoutedEventArgs e) // Разблокировка и заполнение чекбокса с фильмами  || Полностью готово
        {
            ticketFilmList.IsEnabled = true;
            GetFilmWithDate();
            ticketFilmList.ItemsSource = FilmNameList;
        }

        private void ticketFilmList_SelectionChanged(object sender, RoutedEventArgs e) // Разблокировка и заполнение чекбокса с временем  || Полностью готово
        {
            ticketTimeList.IsEnabled = true;
            GetTimeWithDateFilm();
            ticketTimeList.ItemsSource = SeanceTimeList;
        }
                                                                                                                                                                   
        private void ticketTimeList_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с залами  || Полностью готово
        {
            ticketHallList.IsEnabled = true;
            GetHallWithSeance();
            ticketHallList.ItemsSource = HallNameList;
        }

        private void ticketHallList_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с рядами  || Полностью готово
        {
            Raw.IsEnabled = true;
            CheckRaw();
            Raw.ItemsSource = RawsList;
        }

        private void Raw_SelectionChanged(object sender, SelectionChangedEventArgs e) // Разблокировка и заполнение чекбокса с местами  || Полностью готово
        {
            if (Raw.SelectedItem != null)
            {
                Place.IsEnabled = true;
                GetPlace();
                Place.ItemsSource = PlaceList;
            }
        }

        #endregion
            // Проработать логику непоследовательного выбора


        #endregion





        #region Билеты || ???



            #region Функции || Не готово

        public void GetTicketInfo() // Получение списка билетов || Поменять sql запрос
        {
            allTicketList.Clear();

            Ticket cur_ticket;
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    string query = "SELECT * FROM TicketInfo";
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
                        int rowPlace = Convert.ToInt32(row["RawNumber"]);
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

        public void DeleteCurrentTicket() // Возврат проданного билета || Не готово
        {
    int index = allTicketList.IndexOf((Ticket)ticketTable.SelectedItem);
    Ticket cur_ticket = allTicketList[index];
    try
    {
        using (OracleConnection connection = new OracleConnection(OracleDatabaseConnection.connection_meneger))
        {
            connection.Open();
            OracleParameter number_in = new OracleParameter
            {
                ParameterName = "Number_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Int32,
                Value = cur_ticket.Number
            };
            OracleParameter date_in = new OracleParameter
            {
                ParameterName = "Date_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Date,
                Value = Convert.ToDateTime(cur_ticket.seance.seanceDate)
            };
            OracleParameter time_in = new OracleParameter
            {
                ParameterName = "Time_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Varchar2,
                Value = cur_ticket.seance.seanceTime
            };
            OracleParameter hall_in = new OracleParameter
            {
                ParameterName = "Hall_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Varchar2,
                Value = cur_ticket.place.Hall.Name
            };
            OracleParameter film_in = new OracleParameter
            {
                ParameterName = "Film_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Varchar2,
                Value = cur_ticket.seance.seanceFilm.Name
            };
            OracleParameter row_in = new OracleParameter
            {
                ParameterName = "Row_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Int32,
                Value = cur_ticket.place.Raw
            };
            OracleParameter place_in = new OracleParameter
            {
                ParameterName = "Place_in",
                Direction = ParameterDirection.Input,
                OracleDbType = OracleDbType.Int32,
                Value = cur_ticket.place.Number
            };
            using (OracleCommand command = new OracleCommand("admin.deleteTicket"))
            {
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(new OracleParameter[] { number_in, date_in, time_in, hall_in, film_in, row_in, place_in });
                command.ExecuteNonQuery();
                MessageBox.Show("Билет отменен!");
            }
        }
    }
    catch (OracleException ex)
    {
        MessageBox.Show(ex.Message);
    }
}

        #endregion



            #region Кнопки || Протестировать

        private void SearchButton_Click(object sender, RoutedEventArgs e) // Поиск по билетам || Протестировать
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

        private void deleteTicket_Click(object sender, RoutedEventArgs e) // Кнопка удаления билета || Протестировать
        {
    DeleteCurrentTicket();
    GetTicketInfo();
    ticketTable.ItemsSource = allTicketList;
}

        #endregion



            #region Триггеры || ???


        #endregion



        #endregion





    }

}