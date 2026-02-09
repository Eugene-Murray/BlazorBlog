using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ConsoleExperimentsApp.Experiments.BackgroundWork
{
    public static class BackgroundWorkExperiments
    {
        public static async Task Run()
        {
            Console.WriteLine("=== Background Work Experiments ===\n");

            await Experiment1_BasicBackgroundWorker();
            Console.WriteLine();

            await Experiment2_WithProgressReporting();
            Console.WriteLine();

            await Experiment3_WithCancellation();
            Console.WriteLine();

            Console.WriteLine("All background work experiments completed.\n");
        }

        private static Task Experiment1_BasicBackgroundWorker()
        {
            Console.WriteLine("Experiment 1: Basic BackgroundWorker");
            var tcs = new TaskCompletionSource();

            var worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                Console.WriteLine("  Background work started...");
                Thread.Sleep(2000); // Simulate work
                e.Result = "Work completed successfully";
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    Console.WriteLine($"  Error: {e.Error.Message}");
                }
                else
                {
                    Console.WriteLine($"  Result: {e.Result}");
                }
                tcs.SetResult();
            };

            worker.RunWorkerAsync();
            return tcs.Task;
        }

        private static Task Experiment2_WithProgressReporting()
        {
            Console.WriteLine("Experiment 2: BackgroundWorker with Progress Reporting");
            var tcs = new TaskCompletionSource();

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            worker.DoWork += (sender, e) =>
            {
                Console.WriteLine("  Processing items...");
                for (int i = 1; i <= 10; i++)
                {
                    Thread.Sleep(300);
                    worker.ReportProgress(i * 10, $"Processing item {i}");
                }
                e.Result = "All items processed";
            };

            worker.ProgressChanged += (sender, e) =>
            {
                Console.WriteLine($"  Progress: {e.ProgressPercentage}% - {e.UserState}");
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                Console.WriteLine($"  Completed: {e.Result}");
                tcs.SetResult();
            };

            worker.RunWorkerAsync();
            return tcs.Task;
        }

        private static Task Experiment3_WithCancellation()
        {
            Console.WriteLine("Experiment 3: BackgroundWorker with Cancellation");
            var tcs = new TaskCompletionSource();

            var worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            worker.DoWork += (sender, e) =>
            {
                Console.WriteLine("  Long running operation started...");
                for (int i = 1; i <= 20; i++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        Console.WriteLine("  Cancellation detected, cleaning up...");
                        return;
                    }

                    Thread.Sleep(200);
                    worker.ReportProgress(i * 5);

                    if (i == 8)
                    {
                        Console.WriteLine("  Requesting cancellation...");
                        worker.CancelAsync();
                    }
                }
                e.Result = "Operation completed without cancellation";
            };

            worker.ProgressChanged += (sender, e) =>
            {
                Console.Write($"\r  Progress: {e.ProgressPercentage}%");
            };

            worker.RunWorkerCompleted += (sender, e) =>
            {
                Console.WriteLine();
                if (e.Cancelled)
                {
                    Console.WriteLine("  Operation was cancelled");
                }
                else if (e.Error != null)
                {
                    Console.WriteLine($"  Error: {e.Error.Message}");
                }
                else
                {
                    Console.WriteLine($"  Result: {e.Result}");
                }
                tcs.SetResult();
            };

            worker.RunWorkerAsync();
            return tcs.Task;
        }
    }
}
