using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleExperimentsApp.Experiments
{
    public static class TPLExperiments
    {
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("TPLExperiments");
            Console.ResetColor();

            try
            {
                Example1_BasicTaskRun();
                Example2_TaskFactoryStartNew();
                Example3_TaskWithReturnValue();
                Example4_TaskWait();
                Example5_TaskWaitAll();
                Example6_TaskWhenAll();
                Example7_TaskWhenAny();
                Example8_ParallelFor();
                Example9_ParallelForEach();
                Example10_ParallelInvoke();
                Example11_TaskWithCancellation();
                Example12_TaskWithTimeout();
                Example13_TaskContinuation();
                Example14_TaskDelay();
                Example15_ProducerConsumer();
                Example16_ConcurrentCollections();
                Example17_Partitioner();
                Example18_PLINQ();
                Example19_TaskExceptionHandling();
                Example20_AsyncAwaitTask();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error in TPL experiments: {ex.Message}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        // Example 1: Basic Task.Run
        private static void Example1_BasicTaskRun()
        {
            Console.WriteLine("\n1. Basic Task.Run:");

            Task task = Task.Run(() =>
            {
                Console.WriteLine($"Task running on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000);
                Console.WriteLine("Task completed");
            });

            task.Wait();
            Console.WriteLine("Basic Task.Run completed");
        }

        // Example 2: Task.Factory.StartNew
        private static void Example2_TaskFactoryStartNew()
        {
            Console.WriteLine("\n2. Task.Factory.StartNew:");

            Task task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Factory task running on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(500);
                Console.WriteLine("Factory task completed");
            }, TaskCreationOptions.LongRunning);

            task.Wait();
            Console.WriteLine("Task.Factory.StartNew completed");
        }

        // Example 3: Task with return value
        private static void Example3_TaskWithReturnValue()
        {
            Console.WriteLine("\n3. Task with return value:");

            Task<int> task = Task.Run(() =>
            {
                Thread.Sleep(500);
                return 42;
            });

            int result = task.Result;
            Console.WriteLine($"Task returned: {result}");
        }

        // Example 4: Task.Wait
        private static void Example4_TaskWait()
        {
            Console.WriteLine("\n4. Task.Wait:");

            Task task = Task.Run(() =>
            {
                Console.WriteLine("Task starting...");
                Thread.Sleep(1000);
                Console.WriteLine("Task finished");
            });

            Console.WriteLine("Waiting for task to complete...");
            task.Wait();
            Console.WriteLine("Task.Wait completed");
        }

        // Example 5: Task.WaitAll
        private static void Example5_TaskWaitAll()
        {
            Console.WriteLine("\n5. Task.WaitAll:");

            Task[] tasks = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    Thread.Sleep(500 + taskId * 200);
                    Console.WriteLine($"Task {taskId} completed");
                });
            }

            Task.WaitAll(tasks);
            Console.WriteLine("All tasks completed via WaitAll");
        }

        // Example 6: Task.WhenAll
        private static void Example6_TaskWhenAll()
        {
            Console.WriteLine("\n6. Task.WhenAll:");

            Task[] tasks = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    Thread.Sleep(300 + taskId * 100);
                    Console.WriteLine($"WhenAll Task {taskId} completed");
                });
            }

            Task.WhenAll(tasks).Wait();
            Console.WriteLine("All tasks completed via WhenAll");
        }

        // Example 7: Task.WhenAny
        private static void Example7_TaskWhenAny()
        {
            Console.WriteLine("\n7. Task.WhenAny:");

            Task[] tasks = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    Thread.Sleep(1000 + taskId * 500);
                    Console.WriteLine($"WhenAny Task {taskId} completed");
                    return taskId;
                });
            }

            Task.WhenAny(tasks).Wait();
            Console.WriteLine("First task completed via WhenAny");
        }

        // Example 8: Parallel.For
        private static void Example8_ParallelFor()
        {
            Console.WriteLine("\n8. Parallel.For:");

            Parallel.For(0, 5, i =>
            {
                Console.WriteLine($"Parallel.For iteration {i} on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(200);
            });

            Console.WriteLine("Parallel.For completed");
        }

        // Example 9: Parallel.ForEach
        private static void Example9_ParallelForEach()
        {
            Console.WriteLine("\n9. Parallel.ForEach:");

            var items = new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

            Parallel.ForEach(items, item =>
            {
                Console.WriteLine($"Processing {item} on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(300);
            });

            Console.WriteLine("Parallel.ForEach completed");
        }

        // Example 10: Parallel.Invoke
        private static void Example10_ParallelInvoke()
        {
            Console.WriteLine("\n10. Parallel.Invoke:");

            Parallel.Invoke(
                () => {
                    Console.WriteLine("Action 1 executing");
                    Thread.Sleep(500);
                    Console.WriteLine("Action 1 completed");
                },
                () => {
                    Console.WriteLine("Action 2 executing");
                    Thread.Sleep(300);
                    Console.WriteLine("Action 2 completed");
                },
                () => {
                    Console.WriteLine("Action 3 executing");
                    Thread.Sleep(400);
                    Console.WriteLine("Action 3 completed");
                }
            );

            Console.WriteLine("Parallel.Invoke completed");
        }

        // Example 11: Task with CancellationToken
        private static void Example11_TaskWithCancellation()
        {
            Console.WriteLine("\n11. Task with CancellationToken:");

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            try
            {
                Task task = Task.Run(() =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        Console.WriteLine($"Working... {i}");
                        Thread.Sleep(500);
                    }
                }, cts.Token);

                task.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Task was cancelled: {ex.InnerException?.GetType().Name}");
            }
        }

        // Example 12: Task with timeout
        private static void Example12_TaskWithTimeout()
        {
            Console.WriteLine("\n12. Task with timeout:");

            Task task = Task.Run(() =>
            {
                Thread.Sleep(3000);
                return "Task completed";
            });

            bool completed = task.Wait(TimeSpan.FromSeconds(1));
            Console.WriteLine($"Task completed within timeout: {completed}");
        }

        // Example 13: Task continuation with ContinueWith
        private static void Example13_TaskContinuation()
        {
            Console.WriteLine("\n13. Task continuation:");

            Task<int> task = Task.Run(() =>
            {
                Console.WriteLine("Initial task running");
                Thread.Sleep(500);
                return 10;
            });

            Task<int> continuationTask = task.ContinueWith(t =>
            {
                Console.WriteLine($"Continuation task received: {t.Result}");
                return t.Result * 2;
            });

            int result = continuationTask.Result;
            Console.WriteLine($"Final result: {result}");
        }

        // Example 14: Task.Delay
        private static void Example14_TaskDelay()
        {
            Console.WriteLine("\n14. Task.Delay:");

            Console.WriteLine("Starting delay...");
            Task.Delay(1000).Wait();
            Console.WriteLine("Delay completed");
        }

        // Example 15: Producer-Consumer with BlockingCollection
        private static void Example15_ProducerConsumer()
        {
            Console.WriteLine("\n15. Producer-Consumer pattern:");

            using var collection = new BlockingCollection<int>();

            Task producer = Task.Run(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    collection.Add(i);
                    Console.WriteLine($"Produced: {i}");
                    Thread.Sleep(200);
                }
                collection.CompleteAdding();
            });

            Task consumer = Task.Run(() =>
            {
                foreach (int item in collection.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Consumed: {item}");
                    Thread.Sleep(300);
                }
            });

            Task.WaitAll(producer, consumer);
            Console.WriteLine("Producer-Consumer completed");
        }

        // Example 16: Concurrent Collections
        private static void Example16_ConcurrentCollections()
        {
            Console.WriteLine("\n16. Concurrent Collections:");

            var concurrentBag = new ConcurrentBag<int>();
            var concurrentQueue = new ConcurrentQueue<string>();

            Parallel.For(0, 5, i =>
            {
                concurrentBag.Add(i);
                concurrentQueue.Enqueue($"Item {i}");
                Console.WriteLine($"Added item {i} on thread {Thread.CurrentThread.ManagedThreadId}");
            });

            Console.WriteLine($"ConcurrentBag count: {concurrentBag.Count}");
            Console.WriteLine($"ConcurrentQueue count: {concurrentQueue.Count}");
        }

        // Example 17: Partitioner
        private static void Example17_Partitioner()
        {
            Console.WriteLine("\n17. Partitioner:");

            var source = Enumerable.Range(0, 20).ToList();
            var partitioner = Partitioner.Create(source, true);

            Parallel.ForEach(partitioner, item =>
            {
                Console.WriteLine($"Processing {item} on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(100);
            });

            Console.WriteLine("Partitioner example completed");
        }

        // Example 18: PLINQ (Parallel LINQ)
        private static void Example18_PLINQ()
        {
            Console.WriteLine("\n18. PLINQ:");

            var numbers = Enumerable.Range(1, 10);

            var evenSquares = numbers
                .AsParallel()
                .Where(x => x % 2 == 0)
                .Select(x => x * x)
                .ToList();

            Console.WriteLine($"Even squares: {string.Join(", ", evenSquares)}");
        }

        // Example 19: Task exception handling
        private static void Example19_TaskExceptionHandling()
        {
            Console.WriteLine("\n19. Task exception handling:");

            Task task = Task.Run(() =>
            {
                Thread.Sleep(500);
                throw new InvalidOperationException("Something went wrong!");
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Caught exception: {ex.InnerException?.Message}");
            }
        }

        // Example 20: Async/await with Task
        private static void Example20_AsyncAwaitTask()
        {
            Console.WriteLine("\n20. Async/await with Task:");

            async Task DoAsyncWork()
            {
                Console.WriteLine("Async work starting");
                await Task.Delay(500);
                Console.WriteLine("Async work completed");
                return;
            }

            DoAsyncWork().Wait();
            Console.WriteLine("Async/await example completed");
        }
    }
}
