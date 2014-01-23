using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterfaceDemo
{
    public partial class MainWindow5 : Window
    {
        private volatile CancellationTokenSource _cts;

        public MainWindow5()
        {
            InitializeComponent();
        }

        private async void DoWork_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
                return;

            _cts = new CancellationTokenSource();

            try
            {
                var result = await Task.Factory.StartNew(() => DoWork(_cts.Token), _cts.Token);
                OutputLabel.Text = "The answer is " + result + ".";
            }
            catch (OperationCanceledException)
            {
                OutputLabel.Text = "The request was cancelled.";
            }
            catch (Exception)
            {
                OutputLabel.Text = "An error occured while processing the request.";
            }
            finally
            {
                _cts = null;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
                _cts.Cancel();
        }

        private int DoWork(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Thread.Sleep(30);
                OutputLabel.Dispatcher.Invoke(new Action(() => UpdateProgressLabel(i)));
            }

            return 42;
        }

        private void UpdateProgressLabel(int percentage)
        {
            OutputLabel.Text = "Processing..." + percentage + "% Complete";
        }
    }
}
