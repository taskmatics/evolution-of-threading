using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallellStacksTest
{
    class Program
    {
        private static readonly Random _r = new Random();

        static void Main(string[] args)
        {
            Enumerable
                .Range(0, 1000)
                .AsParallel()
                .ForAll(DoWork);
        }

        private static void DoWork(int obj)
        {
            if (_r.Next(1000) % 2 == 0)
                A();
            else
                B();
        }

        private static void A()
        {
            if (_r.Next(1000) % 2 == 0)
                C();
            else
                D();
        }

        private static void B()
        {
            if (_r.Next(1000) % 2 == 0)
                D();
            else
                C();
        }

        private static void C()
        {
            Thread.Sleep(60000);
        }

        private static void D()
        {
            Thread.Sleep(60000);
        }
    }
}
