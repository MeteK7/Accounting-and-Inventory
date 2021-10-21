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
    /// Interaction logic for WinReceipt.xaml
    /// </summary>
    public partial class WinReceipt : Window
    {
        public WinReceipt()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            int lastExpenseId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblReceiptId.Content);

            if (currentInvoiceId != lastExpenseId)
            {
                int nextInvoice = currentInvoiceId + unitValue;

                clickedArrow = clickedNext;//1 means customer has clicked the next button.
                ClearTools();
                LoadPastReceipt(nextInvoice, clickedArrow);
            }
        }
    }
}
