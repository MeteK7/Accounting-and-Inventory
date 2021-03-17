using KabaAccounting.CUL;
using KabaAccounting.DAL;
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
    /// Interaction logic for WinLogin.xaml
    /// </summary>
    public partial class WinLogin : Window
    {
        public WinLogin()
        {
            InitializeComponent();
        }
        LoginCUL loginCUL = new LoginCUL();
        LoginDAL loginDAL = new LoginDAL();
        public static string loggedInUserName;
        public static string loggedInUserType;
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            loginCUL.Username = txtUserName.Text.Trim();
            loginCUL.Password = pswUserPassword.Password;
            loginCUL.UserType = cboUserType.Text.Trim();

            //Checking the login credentials
            bool isSuccess = loginDAL.CheckLogin(loginCUL);

            if (isSuccess == true)
            {
                //Login Successful
                MessageBox.Show("Login Successful");
                loggedInUserName = loginCUL.Username;
                loggedInUserType = loginCUL.UserType;

                this.Hide();
                WinAdminDashboard winAdmin = new WinAdminDashboard(loggedInUserName, loggedInUserType);
                winAdmin.Show();

                //switch (loginCUL.UserType)
                //{
                //    case "Admin":
                //        {
                //            this.Hide();
                //            WinAdminDashboard winAdmin = new WinAdminDashboard();
                //            winAdmin.Show();
                //        }
                //        break;

                //    case "User":
                //        {
                //            this.Hide();
                //            WinUserDashboard winUser = new WinUserDashboard();
                //            winUser.Show();
                //        }
                //        break;

                //    default:
                //        MessageBox.Show("Invalid User Type.");
                //        break;
                //}
            }
            else
            {
                //Login Failed
                MessageBox.Show("Login failed.");
            }
        }
    }
}
