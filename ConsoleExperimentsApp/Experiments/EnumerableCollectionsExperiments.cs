using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExperimentsApp.Experiments
{
    public static class EnumerableCollectionsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Enumerable Collections Experiments ===");
            Console.ResetColor();

            await DemonstrateNonThreadSafeCollections();
            await DemonstrateThreadSafeCollections();
            await ComparePerformanceThreadSafeVsNonThreadSafe();
            await DemonstrateConcurrentOperations();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static async Task DemonstrateNonThreadSafeCollections()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== NON-THREAD-SAFE COLLECTIONS ===");
            Console.ResetColor();

            // List<T>
            Console.WriteLine("\n--- List<T> ---");
            var list = new List<int> { 1, 2, 3, 4, 5 };
            Console.WriteLine($"Initial List: [{string.Join(", ", list)}]");
            list.Add(6);
            list.RemoveAt(0);
            Console.WriteLine($"After Add(6) and RemoveAt(0): [{string.Join(", ", list)}]");
            Console.WriteLine($"Count: {list.Count}, Capacity: {list.Capacity}");

            // Dictionary<TKey, TValue>
            Console.WriteLine("\n--- Dictionary<TKey, TValue> ---");
            var dictionary = new Dictionary<string, int>
            {
                ["apple"] = 5,
                ["banana"] = 3,
                ["orange"] = 8
            };
            Console.WriteLine("Initial Dictionary:");
            foreach (var kvp in dictionary)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
            dictionary["grape"] = 12;
            dictionary.Remove("banana");
            Console.WriteLine($"After adding grape and removing banana: Count = {dictionary.Count}");

            // HashSet<T>
            Console.WriteLine("\n--- HashSet<T> ---");
            var hashSet = new HashSet<string> { "red", "green", "blue" };
            Console.WriteLine($"Initial HashSet: [{string.Join(", ", hashSet)}]");
            hashSet.Add("yellow");
            hashSet.Add("red"); // Duplicate - won't be added
            Console.WriteLine($"After adding yellow and red(duplicate): [{string.Join(", ", hashSet)}]");
            Console.WriteLine($"Contains 'green': {hashSet.Contains("green")}");

            // Queue<T>
            Console.WriteLine("\n--- Queue<T> ---");
            var queue = new Queue<string>();
            queue.Enqueue("first");
            queue.Enqueue("second");
            queue.Enqueue("third");
            Console.WriteLine($"Queue contents: [{string.Join(", ", queue)}]");
            var dequeued = queue.Dequeue();
            Console.WriteLine($"Dequeued: {dequeued}, Remaining: [{string.Join(", ", queue)}]");
            Console.WriteLine($"Peek: {queue.Peek()}");

            // Stack<T>
            Console.WriteLine("\n--- Stack<T> ---");
            var stack = new Stack<int>();
            stack.Push(10);
            stack.Push(20);
            stack.Push(30);
            Console.WriteLine($"Stack contents: [{string.Join(", ", stack)}]");
            var popped = stack.Pop();
            Console.WriteLine($"Popped: {popped}, Remaining: [{string.Join(", ", stack)}]");
            Console.WriteLine($"Peek: {stack.Peek()}");

            // LinkedList<T>
            Console.WriteLine("\n--- LinkedList<T> ---");
            var linkedList = new LinkedList<char>();
            linkedList.AddLast('A');
            linkedList.AddLast('C');
            linkedList.AddFirst('Z');
            var nodeB = linkedList.AddAfter(linkedList.First!, 'B');
            Console.WriteLine($"LinkedList: [{string.Join(", ", linkedList)}]");
            linkedList.Remove(nodeB);
            Console.WriteLine($"After removing B: [{string.Join(", ", linkedList)}]");

            // SortedList<TKey, TValue>
            Console.WriteLine("\n--- SortedList<TKey, TValue> ---");
            var sortedList = new SortedList<int, string>
            {
                [3] = "three",
                [1] = "one",
                [4] = "four",
                [2] = "two"
            };
            Console.WriteLine("SortedList (automatically sorted by key):");
            foreach (var kvp in sortedList)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            // SortedDictionary<TKey, TValue>
            Console.WriteLine("\n--- SortedDictionary<TKey, TValue> ---");
            var sortedDictionary = new SortedDictionary<string, int>
            {
                ["zebra"] = 26,
                ["alpha"] = 1,
                ["beta"] = 2,
                ["gamma"] = 3
            };
            Console.WriteLine("SortedDictionary (automatically sorted by key):");
            foreach (var kvp in sortedDictionary)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            // SortedSet<T>
            Console.WriteLine("\n--- SortedSet<T> ---");
            var sortedSet = new SortedSet<int> { 5, 2, 8, 1, 9, 3 };
            Console.WriteLine($"SortedSet: [{string.Join(", ", sortedSet)}]");
            Console.WriteLine($"Min: {sortedSet.Min}, Max: {sortedSet.Max}");
        }

        private static async Task DemonstrateThreadSafeCollections()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== THREAD-SAFE COLLECTIONS ===");
            Console.ResetColor();

            // ConcurrentQueue<T>
            Console.WriteLine("\n--- ConcurrentQueue<T> ---");
            var concurrentQueue = new ConcurrentQueue<int>();
            concurrentQueue.Enqueue(1);
            concurrentQueue.Enqueue(2);
            concurrentQueue.Enqueue(3);

            Console.WriteLine($"ConcurrentQueue Count: {concurrentQueue.Count}");
            if (concurrentQueue.TryDequeue(out int result))
            {
                Console.WriteLine($"Dequeued: {result}");
            }
            if (concurrentQueue.TryPeek(out int peek))
            {
                Console.WriteLine($"Peek: {peek}");
            }

            // ConcurrentStack<T>
            Console.WriteLine("\n--- ConcurrentStack<T> ---");
            var concurrentStack = new ConcurrentStack<string>();
            concurrentStack.Push("first");
            concurrentStack.Push("second");
            concurrentStack.Push("third");

            Console.WriteLine($"ConcurrentStack Count: {concurrentStack.Count}");
            if (concurrentStack.TryPop(out string? stackResult))
            {
                Console.WriteLine($"Popped: {stackResult}");
            }
            if (concurrentStack.TryPeek(out string? stackPeek))
            {
                Console.WriteLine($"Peek: {stackPeek}");
            }

            // ConcurrentDictionary<TKey, TValue>
            Console.WriteLine("\n--- ConcurrentDictionary<TKey, TValue> ---");
            var concurrentDict = new ConcurrentDictionary<string, int>();
            concurrentDict.TryAdd("apple", 5);
            concurrentDict.TryAdd("banana", 3);
            concurrentDict.AddOrUpdate("orange", 8, (key, oldValue) => oldValue + 1);

            Console.WriteLine("ConcurrentDictionary contents:");
            foreach (var kvp in concurrentDict)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            var newValue = concurrentDict.GetOrAdd("grape", 12);
            Console.WriteLine($"GetOrAdd 'grape': {newValue}");

            concurrentDict.TryUpdate("apple", 10, 5);
            Console.WriteLine($"Updated apple value: {concurrentDict["apple"]}");

            // ConcurrentBag<T>
            Console.WriteLine("\n--- ConcurrentBag<T> ---");
            var concurrentBag = new ConcurrentBag<int>();
            concurrentBag.Add(1);
            concurrentBag.Add(2);
            concurrentBag.Add(3);
            concurrentBag.Add(2); // Duplicates allowed

            Console.WriteLine($"ConcurrentBag Count: {concurrentBag.Count}");
            Console.WriteLine($"Contents: [{string.Join(", ", concurrentBag)}]");

            if (concurrentBag.TryTake(out int bagResult))
            {
                Console.WriteLine($"TryTake result: {bagResult}");
                Console.WriteLine($"Remaining count: {concurrentBag.Count}");
            }

            // BlockingCollection<T>
            Console.WriteLine("\n--- BlockingCollection<T> ---");
            using var blockingCollection = new BlockingCollection<int>(3); // Bounded capacity

            // Producer task
            var producer = Task.Run(() =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    try
                    {
                        blockingCollection.Add(i);
                        Console.WriteLine($"Producer added: {i}");
                        Task.Delay(100).Wait();
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Producer: Collection was closed");
                        break;
                    }
                }
                blockingCollection.CompleteAdding();
                Console.WriteLine("Producer: Completed adding");
            });

            // Consumer task
            var consumer = Task.Run(() =>
            {
                try
                {
                    foreach (var item in blockingCollection.GetConsumingEnumerable())
                    {
                        Console.WriteLine($"Consumer took: {item}");
                        Task.Delay(150).Wait();
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Consumer: Collection was closed");
                }
                Console.WriteLine("Consumer: Finished consuming");
            });

            await Task.WhenAll(producer, consumer);
        }

        private static async Task ComparePerformanceThreadSafeVsNonThreadSafe()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== PERFORMANCE COMPARISON ===");
            Console.ResetColor();

            const int iterations = 100_000;
            var sw = new Stopwatch();

            // Dictionary vs ConcurrentDictionary - Single Thread
            Console.WriteLine("\n--- Single Thread Performance ---");

            // Regular Dictionary
            sw.Start();
            var dict = new Dictionary<int, string>();
            for (int i = 0; i < iterations; i++)
            {
                dict[i] = $"value_{i}";
            }
            sw.Stop();
            Console.WriteLine($"Dictionary (single thread): {sw.ElapsedMilliseconds} ms");

            // ConcurrentDictionary
            sw.Restart();
            var concurrentDict = new ConcurrentDictionary<int, string>();
            for (int i = 0; i < iterations; i++)
            {
                concurrentDict[i] = $"value_{i}";
            }
            sw.Stop();
            Console.WriteLine($"ConcurrentDictionary (single thread): {sw.ElapsedMilliseconds} ms");

            // List vs ConcurrentBag - Single Thread
            sw.Restart();
            var list = new List<int>();
            for (int i = 0; i < iterations; i++)
            {
                list.Add(i);
            }
            sw.Stop();
            Console.WriteLine($"List (single thread): {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            var concurrentBag = new ConcurrentBag<int>();
            for (int i = 0; i < iterations; i++)
            {
                concurrentBag.Add(i);
            }
            sw.Stop();
            Console.WriteLine($"ConcurrentBag (single thread): {sw.ElapsedMilliseconds} ms");
        }

        private static async Task DemonstrateConcurrentOperations()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n=== CONCURRENT OPERATIONS DEMONSTRATION ===");
            Console.ResetColor();

            Console.WriteLine("\n--- Thread Safety Issues with Non-Thread-Safe Collections ---");

            // Demonstrate race condition with regular Dictionary
            var unsafeDict = new Dictionary<int, int>();
            var tasks = new List<Task>();

            Console.WriteLine("Attempting concurrent operations on Dictionary (may cause exceptions):");

            for (int t = 0; t < 5; t++)
            {
                int taskId = t;
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        for (int i = taskId * 1000; i < (taskId + 1) * 1000; i++)
                        {
                            unsafeDict[i] = i * 2;
                        }
                        Console.WriteLine($"Task {taskId}: Completed successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Task {taskId}: Exception - {ex.GetType().Name}");
                    }
                }));
            }

            try
            {
                await Task.WhenAll(tasks);
                Console.WriteLine($"Dictionary final count: {unsafeDict.Count} (may be incorrect due to race conditions)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dictionary operations failed: {ex.GetType().Name}");
            }

            Console.WriteLine("\n--- Safe Concurrent Operations with Thread-Safe Collections ---");

            // Demonstrate safe operations with ConcurrentDictionary
            var safeDict = new ConcurrentDictionary<int, int>();
            var safeTasks = new List<Task>();

            for (int t = 0; t < 5; t++)
            {
                int taskId = t;
                safeTasks.Add(Task.Run(() =>
                {
                    for (int i = taskId * 1000; i < (taskId + 1) * 1000; i++)
                    {
                        safeDict.TryAdd(i, i * 2);
                    }
                    Console.WriteLine($"Safe Task {taskId}: Completed successfully");
                }));
            }

            await Task.WhenAll(safeTasks);
            Console.WriteLine($"ConcurrentDictionary final count: {safeDict.Count} (guaranteed correct)");

            // Demonstrate Producer-Consumer pattern with BlockingCollection
            Console.WriteLine("\n--- Producer-Consumer Pattern ---");
            using var buffer = new BlockingCollection<string>(5);

            var producer1 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    buffer.Add($"Producer1-Item{i}");
                    Console.WriteLine($"Producer1 added: Item{i}");
                    await Task.Delay(50);
                }
                Console.WriteLine("Producer1 finished");
            });

            var producer2 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    buffer.Add($"Producer2-Item{i}");
                    Console.WriteLine($"Producer2 added: Item{i}");
                    await Task.Delay(75);
                }
                Console.WriteLine("Producer2 finished");
            });

            var consumer = Task.Run(async () =>
            {
                int itemCount = 0;
                try
                {
                    foreach (var item in buffer.GetConsumingEnumerable())
                    {
                        Console.WriteLine($"Consumer processed: {item}");
                        itemCount++;
                        await Task.Delay(25);

                        if (itemCount >= 20) // Stop after processing 20 items
                            break;
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Consumer: Collection completed");
                }
                Console.WriteLine($"Consumer processed {itemCount} items");
            });

            await Task.WhenAll(producer1, producer2);
            buffer.CompleteAdding();
            await consumer;

            Console.WriteLine("\n--- Thread-Safe Collection Methods Comparison ---");
            var concurrentDict2 = new ConcurrentDictionary<string, int>();

            // AddOrUpdate
            var value1 = concurrentDict2.AddOrUpdate("counter", 1, (key, oldValue) => oldValue + 1);
            Console.WriteLine($"AddOrUpdate result: {value1}");

            var value2 = concurrentDict2.AddOrUpdate("counter", 1, (key, oldValue) => oldValue + 1);
            Console.WriteLine($"AddOrUpdate result (increment): {value2}");

            // GetOrAdd
            var value3 = concurrentDict2.GetOrAdd("new_key", 42);
            Console.WriteLine($"GetOrAdd result: {value3}");

            var value4 = concurrentDict2.GetOrAdd("new_key", 100); // Should return existing value
            Console.WriteLine($"GetOrAdd existing result: {value4}");

            // TryUpdate
            bool updated = concurrentDict2.TryUpdate("counter", 999, 2);
            Console.WriteLine($"TryUpdate success: {updated}, new value: {concurrentDict2["counter"]}");
        }
    }
}
