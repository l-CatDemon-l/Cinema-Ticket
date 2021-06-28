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
            GetSeance();
            seanceTable.ItemsSource = allSeanceList;
            allSeanceTime.ItemsSource = SeanceTimeList;
            GetHall();
            allSeanceHall.ItemsSource = HallNameList;
            GetPost();
            GetGenre();

        }



        #endregion





        #region Общее || Полностью готово



            #region Функции || Полностью готово

        private void products_SelectionChanged(object sender, SelectionChangedEventArgs e) // Обновление при переключении между вкладками || Полностью готово
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
