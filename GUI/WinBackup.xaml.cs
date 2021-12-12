using CUL;
using DAL;
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
    /// Interaction logic for WinBackup.xaml
    /// </summary>
    public partial class WinBackup : Window
    {
        BackupDAL backupDAL = new BackupDAL();
        BackupCUL backupCUL = new BackupCUL();
        public WinBackup()
        {
            InitializeComponent();
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                lblPath.Content = dialog.SelectedPath;
            }
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            backupCUL.DatabasePath = lblPath.Content.ToString();
            backupCUL.DatabaseName = "KabaAccounting" + DateTime.Now.ToString("M-dd-yyyy-HH-mm-ss");
            string isSuccess=backupDAL.BackupDatabase(backupCUL);

            MessageBox.Show(isSuccess);

        }
    }
}
