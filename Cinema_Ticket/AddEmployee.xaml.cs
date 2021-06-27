using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Data;
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
using System.Collections.Generic;


namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Window
    {
        List<string> PostNameList;
        public AddEmployee(List<string> postList)
        {
            InitializeComponent();
            PostNameList = postList;
            employeePost.ItemsSource = PostNameList;
            employeeBirthday.DisplayDateEnd = DateTime.Now.AddYears(-18);
            employeeStart.DisplayDateEnd = DateTime.Now;
        }

        private void employeeAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (employeeSurname.Text == null || employeeName.Text == null || employeeSecondName.Text == null || employeeBirthday.SelectedDate == null || employeeStart.SelectedDate == null
                                                 || employeePost.Text == null || employeePhoneNumber.Text == null || employeeSalary.Text == null)
                {
                    MessageBox.Show("Заполните все поля!");
                }
                else
                {

                    addEmployee();
                    this.Owner.Activate();
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void checkPost()
        {
            foreach (var l in AdminWindow.allPostList)
            {

                if (employeePost.Text == l.postName)
                {
                    if (l.postAccessLevel == "1" || l.postAccessLevel == "2")
                    {
                        employeeLogin.IsReadOnly = false;
                        employeePassword.IsReadOnly = false;
                        employeeLogin.Text = null;
                        employeePassword.Text = null;
                        employeeLogin.Background = new SolidColorBrush(Colors.White);
                        employeePassword.Background = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        employeeLogin.IsReadOnly = true;
                        employeeLogin.Background = new SolidColorBrush(Colors.Silver);
                        employeePassword.IsReadOnly = true;
                        employeePassword.Background = new SolidColorBrush(Colors.Silver);
                        employeeLogin.Text = null;
                        employeePassword.Text = null;
                    }
                }
            }
        }

        public void addEmployee()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var Surname_in = employeeSurname.Text;
                    var Name_in = employeeName.Text;
                    var SecondName_in = employeeSecondName.Text;
                    var Birthday_in = employeeBirthday.SelectedDate;
                    var DateOfEnrollment_in = employeeStart.SelectedDate;
                    var Post_in = employeePost.Text;
                    var PhoneNumber_in = employeePhoneNumber.Text;
                    var Login_in = employeeLogin.Text;
                    var Password_in = employeePassword.Text;
                    string query = $@"SELECT COUNT(*) from Employee where Surname = '{Surname_in}' and Name = '{Name_in}' and SecondName = '{SecondName_in}' and PhoneNumber = '{PhoneNumber_in}'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    object result = cmd.ExecuteScalar();
                    int a = Convert.ToInt32(result);
                    if (a != 0)
                    {
                        MessageBox.Show("Такой пользователь уже существует!");
                    }
                    else
                    {
                        query = $@"insert into Employee(Surname,Name, SecondName, IDPost,Birthday, DateOfEnrollment,PhoneNumber) values ('{Surname_in}','{Name_in}','{SecondName_in}',(select id from post where Positionname = '{Post_in}'),(CAST('{Birthday_in}' as datetime2)),GETDATE(),'{PhoneNumber_in}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        query = $@"select MAX(Id) from Employee";
                        cmd = new SqlCommand(query, connection);
                        result = cmd.ExecuteScalar();
                        a = Convert.ToInt32(result);
                        query = $@"insert into Users(Login, Password, IDEmployee) values ('{Login_in}','{Password_in}','{a}')";
                        cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Сотрудник добавлен!");
                        (this.Owner as AdminWindow).GetEmployee();
                    }

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void employeePost_GotFocus(object sender, RoutedEventArgs e)
        {
            checkPost();
        }


        private void employeeBirthday_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            employeeStart.DisplayDateStart = new DateTime(employeeBirthday.SelectedDate.Value.Year + 18, employeeBirthday.SelectedDate.Value.Month, employeeBirthday.SelectedDate.Value.Day);
            employeeStart.DisplayDateEnd = DateTime.Now;
        }
    }
}
