using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinema_Ticket
{
    /*class Registration
    {
        static CinEntities1 db = new CinEntities1();
        static public void Regist(TextBox tbFN, TextBox tbLN, TextBox tbLog, PasswordBox pbPassw, Window RegisterWindow)
        {
            if (String.IsNullOrEmpty(tbLog.Text) || String.IsNullOrEmpty(pbPassw.Password) || String.IsNullOrEmpty(tbFN.Text) || String.IsNullOrEmpty(tbLN.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                Func f = new Func();
                authorisation emp = new authorisation
                {
                    FirstName = tbFN.Text,
                    LastName = tbLN.Text,
                    Login = tbLog.Text,
                    Pass = f.GetHashPassword(pbPassw.Password),
                };
                try
                {
                    db.authorisation.Add(emp);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    db.authorisation.Remove(emp);
                    db.SaveChanges();
                    MessageBox.Show("Имя пользователя занято");
                    tbLog.Clear();
                    return;
                }

                MessageBoxResult res = MessageBox.Show("Пользователь добавлен! Повторить?", "Добавление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.No)
                {
                    MainWindow MainWindow = new MainWindow();
                    RegisterWindow.Close();
                    MainWindow.Show();
                }
                else
                {
                    tbFN.Clear();
                    tbLN.Clear();
                    tbLog.Clear();
                    pbPassw.Clear();
                }

            }
        }
    }*/
}
    
