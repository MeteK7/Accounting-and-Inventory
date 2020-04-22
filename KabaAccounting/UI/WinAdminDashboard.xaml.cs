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
    /// Interaction logic for WinAdminDashboard.xaml
    /// </summary>
    public partial class WinAdminDashboard : Window
    {
        public WinAdminDashboard()
        {
            InitializeComponent();
        }

        private void menuItemUsers_Click(object sender, RoutedEventArgs e)
        {
            WinUsers user = new WinUsers();
            user.Show();
        }
    }
}
