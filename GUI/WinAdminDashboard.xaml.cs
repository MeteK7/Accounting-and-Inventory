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

namespace GUI
{
    /// <summary>
    /// Interaction logic for WinAdminDashboard.xaml
    /// </summary>
    public partial class WinAdminDashboard : Window
    {
        public WinAdminDashboard()
        {
            InitializeComponent();
            lblLoggedInUser.Content = WinLogin.loggedIn;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            WinLogin winLogin = new WinLogin();
            winLogin.Show();
        }


        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            WinProducts winProducts = new WinProducts();
            winProducts.Show();
        }

        private void btnCategory_Click(object sender, RoutedEventArgs e)
        {
            WinCategory winCategories = new WinCategory();
            winCategories.Show();
        }

        private void btnCustomer_Click(object sender, RoutedEventArgs e)
        {
            WinCustomer winCustomer = new WinCustomer();
            winCustomer.Show();
        }

        private void btnSupplier_Click(object sender, RoutedEventArgs e)
        {
            WinSupplier winSupplier = new WinSupplier();
            winSupplier.Show();
        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            WinUser winUser = new WinUser();
            winUser.Show();
        }

        private void btnBank_Click(object sender, RoutedEventArgs e)
        {
            WinBank winBank = new WinBank();
            winBank.Show();
        }

        private void btnPointOfSale_Click(object sender, RoutedEventArgs e)
        {
            WinPointOfSale winPointOfSale = new WinPointOfSale();
            winPointOfSale.Show();
        }

        private void btnPosReport_Click(object sender, RoutedEventArgs e)
        {
            WinPosReport winPosReport = new WinPosReport();
            winPosReport.Show();
        }

        private void btnPointOfPurchase_Click(object sender, RoutedEventArgs e)
        {
            WinPointOfPurchase winPointOfPurchase = new WinPointOfPurchase();
            winPointOfPurchase.Show();
        }

        private void btnPhyInventory_Click(object sender, RoutedEventArgs e)
        {
            WinPhyInventory winPhyInventory = new WinPhyInventory();
            winPhyInventory.Show();
        }
    }
}