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

namespace WpfApp_PropertyGridPractice
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionWindow : Window
    {
        MessageVM vm;
        public TransactionWindow()
        {
            InitializeComponent();
            Object[] obj = { "ICAPL", "OMS", null };
            vm = new MessageVM(obj);
         //   this.DataContext = vm;
            Trafix.ReportClient.View.MessagesUC uc = new Trafix.ReportClient.View.MessagesUC();
           this.Content = uc;
            
                }
    }
}
