using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterfaceDemo
{
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            InitializeComponent();
        }

        private void DoWork_Click(object sender, RoutedEventArgs e)
        {
            DoWork();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DoWork()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(30);
                    OutputLabel.Text = "Processing..." + i + "% Complete";
                }

                var result = 42;
                OutputLabel.Text = "The answer is " + result + ".";
            }
            catch (Exception)
            {
                OutputLabel.Text = "An error occured while processing the request.";
            }
        }
    }
}
