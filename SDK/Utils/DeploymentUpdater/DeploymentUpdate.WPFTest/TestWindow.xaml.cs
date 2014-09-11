using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ServiceModel;
using DeploymentUpdate.WPFTest.Configuration;
using DeploymentUpdate.Service.UI.Interface;
using DeploymentUpdate.Service.UI.Interface.Configuration;

namespace DeploymentUpdate.WPFTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private List<MailAddressSurrogate> Recipients = new List<MailAddressSurrogate>();
        public TestWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            AddRecipient ar = new AddRecipient();

            w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            w.WindowStyle = WindowStyle.ToolWindow;
            w.SizeToContent = SizeToContent.WidthAndHeight;
            w.Content = ar;
            if (w.ShowDialog() == true)
                Recipients.Add(ar.Recipient);

        }
    }


}
