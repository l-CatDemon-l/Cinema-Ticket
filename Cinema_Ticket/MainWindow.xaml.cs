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
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Employee curr_employee;
        public MainWindow()
        {
            InitializeComponent();
            Header.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot_MouseLeftButtonDown);
            tbLog.ToolTip = "Введите имя пользователя";
            pbPass.ToolTip = "Пароль должен содержать 16 символов";
        }

        public void checkUser()
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var in_login = tbLog.Text;
                    var in_password = pbPass.Password;
                    string query = $@"select Users.ID as Users, Login, Password, Employee.ID, Name, SecondName, Surname, PhoneNumber, Birthday, DateOfEnrollment, PositionName from Employee inner join Post on IDPost = Post.ID left outer join Users on Employee.Id = IDEmployee where Login = '{in_login}' and Password = '{in_password}'";
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
                        string post = row["positionName"].ToString();
                        string phone = row["phoneNumber"].ToString();
                        string login = (row["Login"]).ToString();
                        string password = (row["Password"]).ToString();
                        curr_employee = new Employee(ID, fio, name, secondname, surname, birthday, startDay, phone, post, login, password);
                    }

                }
                try
                {
                    if (curr_employee != null)
                    {
                        if (curr_employee.Post == "Пользователь" || curr_employee.Post == "Менеджер" || curr_employee.Post == "Администратор")
                        {

                            if (curr_employee.Post == "Администратор")
                            {

                                AdminWindow adminWindow = new AdminWindow(curr_employee.ID, curr_employee.FIO);
                                adminWindow.Show();
                                this.Close();
                            }
                            if (curr_employee.Post == "Менеджер")
                            {
                                BookingWindow bookingWindow = new BookingWindow(curr_employee.ID, curr_employee.FIO);
                                bookingWindow.Show();
                                this.Close();
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Пользователя не существует!");
                        tbLog.Text = "";
                        pbPass.Password = "";
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void Login()
        {
            if (tbLog.Text != "" && pbPass.Password != "")
            {
                checkUser();
            }
            else if (tbLog.Text == "" && pbPass.Password != "")
            {
                PopupText.Content = "Заполните поле";
                tbLog.BorderBrush = Brushes.Red;
            }
            else if (tbLog.Text != "" && pbPass.Password == "")
            {
                PopupText1.Content = "Заполните поле";
                pbPass.BorderBrush = Brushes.Red;
            }
            else
            {
                PopupText.Content = "Заполните поле";
                tbLog.BorderBrush = Brushes.Red;
                PopupText1.Content = "Заполните поле";
                pbPass.BorderBrush = Brushes.Red;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void loginText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbLog.Text == "")
            {
                tbLog.BorderBrush = Brushes.Red;
                PopupText.Content = "Заполните поле";
            }
            else
            {
                tbLog.BorderBrush = Brushes.Silver;
                PopupText.Content = "";
            }
        }

        private void passwordText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pbPass.Password == "")
            {
                pbPass.BorderBrush = Brushes.Red;
                PopupText1.Content = "Заполните поле";
            }
            else
            {
                pbPass.BorderBrush = Brushes.Silver;
                PopupText1.Content = "";
            }
        }

        private void passwordText_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPass.BorderBrush = Brushes.Silver;
            PopupText1.Content = "";
        }

        private void loginText_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPass.BorderBrush = Brushes.Silver;
            PopupText.Content = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegisterWindow RegisterWindow = new RegisterWindow();
            RegisterWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void tbLog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void pbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

    }
}