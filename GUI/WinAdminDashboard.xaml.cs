using CUL;
using DAL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for WinAdminDashboard.xaml
    /// </summary>
    public partial class WinAdminDashboard : Window
    {
        public static string _loggedInUserName;
        public static string _loggedInUserType;

        public WinAdminDashboard(string loggedInUserName, string loggedInUserType)
        {
            InitializeComponent();

            _loggedInUserName = loggedInUserName;
            _loggedInUserType = loggedInUserType;

            lblLoggedInUser.Content = _loggedInUserName;

            StartClock();
            Authorization();//Permissions of control according to the user authorization.
        }

        private void Authorization()
        {
            if (_loggedInUserType=="User")
            {
                #region SERVICES
                btnFileManagement.Visibility = Visibility.Hidden;
                btnSysPref.Visibility = Visibility.Hidden;
                btnBackup.Visibility = Visibility.Hidden;
                btnUser.Visibility =Visibility.Hidden;
                #endregion

                #region PORTAL
                btnBank.Visibility = Visibility.Hidden;
                btnSupplier.Visibility = Visibility.Hidden;
                btnCustomer.Visibility = Visibility.Hidden;
                btnCategory.Visibility = Visibility.Hidden;
                #endregion
            }

            else
            {
                lblUserType.Content = "[ADMIN]";
            }
        }

        private void StartClock()
        {
            int second = 1;
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(second);
            dispatcherTimer.Tick += TickEvent;
            dispatcherTimer.Start();
        }

        private void TickEvent(object sender, EventArgs e)
        {
            lblCurrentDateTime.Content = DateTime.Now.ToString("HH:mm:ss | dddd | MMM dd, yyyy");
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

        private void btnPosReports_Click(object sender, RoutedEventArgs e)
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

        private void btnInvAdjustment_Click(object sender, RoutedEventArgs e)
        {
            WinInventoryAdjustment winInventoryAdjustment = new WinInventoryAdjustment();
            winInventoryAdjustment.Show();
        }

        private void btnDeposit_Click(object sender, RoutedEventArgs e)
        {
            WinDeposit winDeposit = new WinDeposit();
            winDeposit.Show();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            WinAccount winAccount = new WinAccount();
            winAccount.Show();
        }

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            WinExpense winPayment = new WinExpense();
            winPayment.Show();
        }

        private void btnReceipt_Click(object sender, RoutedEventArgs e)
        {
            WinReceipt winReceipt = new WinReceipt();
            winReceipt.Show();
        }
    }
}