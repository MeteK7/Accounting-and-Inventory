using BLL;
using CUL;
using CUL.Enums;
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
        EnumBLL enumBLL = new EnumBLL();
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
            backupCUL.DatabaseName = enumBLL.GetDescription(Names.AccountingAndInventory) + DateTime.Now.ToString("M-dd-yyyy-HH-mm-ss")+ enumBLL.GetDescription(FileExtensions.Bak);
            string isSuccess=backupDAL.BackupDatabase(backupCUL);

            MessageBox.Show(isSuccess);
        }
    }
}
