using KabaAccounting.UI;
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

namespace KabaAccounting
{
    /// <summary>
    /// Interaction logic for WinUserDashboard.xaml
    /// </summary>
    public partial class WinUserDashboard : Window
    {
        public WinUserDashboard()
        {
            InitializeComponent();
            lblLoggedInUser.Content = WinLogin.loggedIn;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            WinLogin winLogin = new WinLogin();
            winLogin.Show();
            this.Hide();
        }


    }
}
