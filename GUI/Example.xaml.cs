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
    public class ExitKey : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Example example = new Example();
            MessageBox.Show("HELLO");
        }
    }

    //public class CommandsContext
    //{
    //    public ICommand RunCommand
    //    {
    //        get
    //        {
    //            return new ExitKey();
    //        }
    //    }
    //}

    /// <summary>
    /// Interaction logic for Example.xaml
    /// </summary>
    public partial class Example : Window
    {
        public Example()
        {
            InitializeComponent();
            this.DataContext = new CommandsContext()
            {
            };
        }
    }
}
