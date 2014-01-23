using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkItemProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new WorkItemProcessor<int>(DoWork, 3);
            p.QueueWorkItem(4000);
            p.QueueWorkItem(1000);
            p.QueueWorkItem(1000);
            p.Complete();

            try
            {
                p.Completion.Wait();
                Console.WriteLine("Processing complete.");
            }
            catch (Exception)
            {
                Console.WriteLine("caught");
            }

            Console.ReadLine();
        }

        private static void DoWork(int i)
        {
            if (i >= 5000)
                throw new ArgumentOutOfRangeException("Cannot handle item with value 5000 or more.");

            Thread.Sleep(i);
            Console.WriteLine("Done processing item: " + i);
        }
    }

    public class WorkItemProcessor<T>
    {
        private readonly int _maxConcurrentWorkItems;
        private readonly BlockingCollection<T> _workItems;
        private readonly Action<T> _action;
        private readonly SemaphoreSlim _maxWorkItemsSignal;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _completion;

        public WorkItemProcessor(Action<T> action, int maxConcurrentWorkItems = 1)
        {
            _maxConcurrentWorkItems = maxConcurrentWorkItems;
            _workItems = new BlockingCollection<T>();
            _action = action;
            _maxWorkItemsSignal = new SemaphoreSlim(maxConcurrentWorkItems);
            _cancellationTokenSource = new CancellationTokenSource();
            _completion = Process();
        }

        public Task Completion
        {
            get { return _completion; }
        }

        private Task Process()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var workItem in _workItems.GetConsumingEnumerable(_cancellationTokenSource.Token))
                        ProcessWorkItem(workItem);
                }
                catch (OperationCanceledException) { }
            });
        }

        private void ProcessWorkItem(T workItem)
        {
            _maxWorkItemsSignal.Wait();

            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _action(workItem);
                }
                catch (Exception)
                {
                    _workItems.CompleteAdding();
                    _cancellationTokenSource.Cancel();
                    throw;
                }
                finally
                {
                    _maxWorkItemsSignal.Release();
                }
            }, TaskCreationOptions.AttachedToParent);
        }

        public void Complete()
        {
            _workItems.CompleteAdding();
        }

        public bool QueueWorkItem(T workItem)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    _workItems.Add(workItem);
                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
