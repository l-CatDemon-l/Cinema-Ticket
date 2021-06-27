using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Cinema_Ticket
{
    /// <summary>
    /// Логика взаимодействия для timing.xaml
    /// </summary>
    public partial class timing : Window
    {
        public timing()
        {
            InitializeComponent();
            brdTitle.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot_MouseLeftButtonDown);

        }
        void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

       /* private void btnS1_Click(object sender, RoutedEventArgs e)
        {
          
            
            this.Hide();
            Place Place = new Place();
            Place.Show();

        }*/

        private void btnRet_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            
        }

        private void btnS1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
