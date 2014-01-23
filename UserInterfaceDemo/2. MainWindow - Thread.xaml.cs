using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterfaceDemo
{
    public partial class MainWindow2 : Window
    {
        private AutoResetEvent _doWorkCompletedSignal = new AutoResetEvent(true);
        private volatile bool _cancellationRequested;
        private Thread _worker;

        public MainWindow2()
        {
            InitializeComponent();
        }

        private void DoWork_Click(object sender, RoutedEventArgs e)
        {
            if (!_doWorkCompletedSignal.WaitOne(0))
                return;

            _worker = new Thread(() => DoWork());
            _worker.Start();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_worker != null)
                _cancellationRequested = true;
        }

        private void DoWork()
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (_cancellationRequested)
                    {
                        MarshalUpdateOutputLabelText("The request was cancelled.");
                        return;
                    }

                    Thread.Sleep(30);
                    MarshalUpdateOutputLabelText("Processing..." + i + "% Complete");
                }

                var result = 42;
                MarshalUpdateOutputLabelText("The answer is " + result + ".");
            }
            catch (Exception)
            {
                MarshalUpdateOutputLabelText("An error occured while processing the request.");
            }
            finally
            {
                _cancellationRequested = false;
                _worker = null;
                _doWorkCompletedSignal.Set();
            }
        }

        private void MarshalUpdateOutputLabelText(string text)
        {
            OutputLabel.Dispatcher.Invoke(new Action(() =>
            {
                OutputLabel.Text = text;
            }));
        }
    }
}
