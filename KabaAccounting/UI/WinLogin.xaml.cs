using KabaAccounting.BLL;
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

namespace KabaAccounting.UI
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

        LoginBLL loginBLL = new LoginBLL();
        LoginDAL loginDAL = new LoginDAL();
        public static string loggedIn;
        public static string loggedInPosition;
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            loginBLL.Username = txtUserName.Text.Trim();
            loginBLL.Password = pswUserPassword.Password;
            loginBLL.UserType = cboUserType.Text.Trim();

            //Checking the login credentials
            bool isSuccess = loginDAL.CheckLogin(loginBLL);

            if (isSuccess==true)
            {
                //Login Successful
                MessageBox.Show("Login Successful");
                loggedIn = loginBLL.Username;
                loggedInPosition = loginBLL.UserType;

                switch (loginBLL.UserType)
                {
                    case "Admin":
                        {
                            WinAdminDashboard winAdmin = new WinAdminDashboard();
                            winAdmin.Show();
                            this.Hide();
                        }
                        break;

                    case "User":
                        {
                            WinUserDashboard winUser = new WinUserDashboard();
                            winUser.Show();
                            this.Hide();
                        }
                        break;

                    default:
                        MessageBox.Show("Invalid User Type.");
                        break;
                }
            }
            else
            {
                //Login Failed
                MessageBox.Show("Login failed.");
            }
        }
    }
}
