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
    /// Логика взаимодействия для ChangeEmployee.xaml
    /// </summary>
    public partial class ChangeEmployee : Window
    {
        Employee employee;
        List<string> PostNameList;
        public ChangeEmployee(Employee employee, List<string> postList)
        {
            InitializeComponent();
            this.employee = employee;
            PostNameList = postList;
            employeePost.ItemsSource = PostNameList;
            employeeSurname.Text = employee.Surname;
            employeeName.Text = employee.Name;
            employeeSecondName.Text = employee.Secondname;
            employeeBirthday.Text = employee.Birthday;
            employeeStart.Text = employee.DateOfEnrollment;
            employeePost.Text = employee.Post;
            employeePhoneNumber.Text = employee.PhoneNumber;
        }

        private void employeePost_KeyDown(object sender, KeyEventArgs e)
        {
            employeePost.IsDropDownOpen = true;
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
                    saveChanges();
                    this.Owner.Activate();
                    this.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void saveChanges()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(SqlDBConnection.connection))
                {
                    connection.Open();
                    var ID_in = employee.ID;
                    var Surname_in = employeeSurname.Text;
                    var Name_in = employeeName.Text;
                    var SecondName_in = employeeSecondName.Text;
                    var Birthday_in = employeeBirthday.SelectedDate;
                    var DateOfEnrollment_in = employeeStart.SelectedDate;
                    var Post_in = employeePost.Text;
                    var PhoneNumber_in = employeePhoneNumber.Text;
                    if(ID_in != 0)
                    {
                        string query = $@"update Employee set Name = '{Name_in}', SecondName = '{SecondName_in}', Surname = '{Surname_in}', Birthday = (CAST('{Birthday_in}' as datetime2)), IDPost = (select id from post where Positionname = '{Post_in}'), PhoneNumber = '{PhoneNumber_in}' where ID = '{ID_in}'";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Информация о сотруднике изменена!");
                    }
                    else
                    {
                        MessageBox.Show("Такой учетной записи не существует!");
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

