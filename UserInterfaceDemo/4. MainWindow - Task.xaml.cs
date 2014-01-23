using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterfaceDemo
{
    public partial class MainWindow4 : Window
    {
        private volatile CancellationTokenSource _cts;

        public MainWindow4()
        {
            InitializeComponent();
        }

        private void DoWork_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
                return;

            _cts = new CancellationTokenSource();
            Task
                .Factory
                .StartNew(() => DoWork(_cts.Token), _cts.Token)
                .ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.Canceled)
                        OutputLabel.Text = "The request was cancelled.";
                    else if (t.Exception != null)
                        OutputLabel.Text = "An error occured while processing the request.";
                    else
                        OutputLabel.Text = "The answer is " + t.Result + ".";

                    _cts = null;

                }, TaskScheduler.FromCurrentSynchronizationContext());
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
