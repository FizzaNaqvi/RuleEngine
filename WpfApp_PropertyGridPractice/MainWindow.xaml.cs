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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trafix.CLRCommon;
using System.Configuration;
using Trafix.ReportClient.Model;
using Trafix.Client.Controls;
using Trafix;

namespace WpfApp_PropertyGridPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SKWindow
    {
        MessageVM messageVM;
        public MainWindow()
        {
            InitializeComponent();
            Logger.Init(AppDomain.CurrentDomain.BaseDirectory);
            editor.Focus();
            Object[] obj = { "FIRMB", "OMS", null }; //ICAPL
            messageVM = new MessageVM(obj);
            this.DataContext = messageVM;
          
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var rules = editor.Text;
           CustomMessage msg= messageVM.ApplyRules(rules);
            if (msg != null)
            {
                PGMsgDetailsUpdated.SelectedObject = msg;
                pnlMessageDetailsUpdated.Visibility = Visibility.Visible;
            }
            else
                MessageBox.Show("Oops! Something went wrong.", "Error");
        }

        private void PGMsgDetails_SelectionChanged(object sender, RoutedEventArgs e)
        {
           var msg = grd.SelectedItem as CustomMessage;
            if (msg != null)
            {
                messageVM.initMsg(msg);
                pnlMessageDetailsUpdated.Visibility = Visibility.Collapsed;
            }

        }
    }
}
