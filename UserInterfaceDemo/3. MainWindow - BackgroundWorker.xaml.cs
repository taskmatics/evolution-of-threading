using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterfaceDemo
{
    public partial class MainWindow3 : Window
    {
        private readonly BackgroundWorker _worker;

        public MainWindow3()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void DoWork_Click(object sender, RoutedEventArgs e)
        {
            if (_worker.IsBusy)
                return;

            _worker.RunWorkerAsync();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _worker.CancelAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 100; i++)
            {
                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(30);
                _worker.ReportProgress(i);
            }

            e.Result = 42;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            OutputLabel.Text = "Processing..." + e.ProgressPercentage + "% Complete";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                OutputLabel.Text = "The request was cancelled.";
            else if (e.Error != null)
                OutputLabel.Text = "An error occured while processing the request.";
            else
                OutputLabel.Text = "The answer is " + e.Result + ".";
        }
    }
}
