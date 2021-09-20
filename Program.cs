using BenchmarkDotNet.Running;
using System;

namespace SQLBenchmark
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}