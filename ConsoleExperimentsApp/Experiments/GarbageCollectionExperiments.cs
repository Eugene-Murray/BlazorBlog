using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;
using System.Threading;
using System.Linq;

namespace ConsoleExperimentsApp.Experiments
{
    public static class GarbageCollectionExperiments
    {

            public static void Run()
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("GarbageCollectionExperiments");
                Console.ResetColor();

                // Run all 20 GC examples
                Example1_BasicGCCollection();
                Example2_WeakReferences();
                Example3_LargeObjectHeap();
                Example4_GenerationsDemo();
                Example5_FinalizersDemo();
                Example6_DisposablePattern();
                Example7_MemoryPressure();
                Example8_SuppressFinalize();
                Example9_GCNotifications();
                Example10_ConcurrentGC();
                Example11_StringInterning();
                Example12_ArrayPooling();
                Example13_EventHandlerLeaks();
                Example14_BoxingUnboxing();
                Example15_ValueVsReferenceTypes();
                Example16_StaticReferences();
                Example17_CircularReferences();
                Example18_WaitForFinalizers();
                Example19_TotalMemoryTracking();
                Example20_AllocationPatterns();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Press Enter to exit...");
                Console.ResetColor();
            }

            // Example 1: Basic GC Collection
            static void Example1_BasicGCCollection()
            {
                Console.WriteLine("\n=== Example 1: Basic GC Collection ===");

                long memoryBefore = GC.GetTotalMemory(false);
                Console.WriteLine($"Memory before: {memoryBefore:N0} bytes");

                // Create objects
                for (int i = 0; i < 100000; i++)
                {
                    var obj = new string('A', 100);
                }

                long memoryAfter = GC.GetTotalMemory(false);
                Console.WriteLine($"Memory after allocation: {memoryAfter:N0} bytes");

                // Force garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();

                long memoryAfterGC = GC.GetTotalMemory(false);
                Console.WriteLine($"Memory after GC: {memoryAfterGC:N0} bytes");
                Console.WriteLine($"Memory freed: {memoryAfter - memoryAfterGC:N0} bytes");
            }

            // Example 2: Weak References
            static void Example2_WeakReferences()
            {
                Console.WriteLine("\n=== Example 2: Weak References ===");

                var strongRef = new LargeObject();
                var weakRef = new WeakReference(strongRef);

                Console.WriteLine($"Strong ref alive: {strongRef != null}");
                Console.WriteLine($"Weak ref alive: {weakRef.IsAlive}");

                strongRef = null; // Remove strong reference

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine($"After GC - Weak ref alive: {weakRef.IsAlive}");
            }

            // Example 3: Large Object Heap (LOH)
            static void Example3_LargeObjectHeap()
            {
                Console.WriteLine("\n=== Example 3: Large Object Heap ===");

                Console.WriteLine($"Gen 0 collections before: {GC.CollectionCount(0)}");
                Console.WriteLine($"Gen 1 collections before: {GC.CollectionCount(1)}");
                Console.WriteLine($"Gen 2 collections before: {GC.CollectionCount(2)}");

                // Allocate large objects (>85KB go to LOH)
                var largeArrays = new List<byte[]>();
                for (int i = 0; i < 10; i++)
                {
                    largeArrays.Add(new byte[100_000]); // 100KB each
                }

                GC.Collect();

                Console.WriteLine($"Gen 0 collections after: {GC.CollectionCount(0)}");
                Console.WriteLine($"Gen 1 collections after: {GC.CollectionCount(1)}");
                Console.WriteLine($"Gen 2 collections after: {GC.CollectionCount(2)}");
            }

            // Example 4: Generations Demo
            static void Example4_GenerationsDemo()
            {
                Console.WriteLine("\n=== Example 4: Generations Demo ===");

                var obj = new object();
                Console.WriteLine($"Object generation: {GC.GetGeneration(obj)}");

                GC.Collect(0);
                Console.WriteLine($"After Gen 0 GC: {GC.GetGeneration(obj)}");

                GC.Collect(1);
                Console.WriteLine($"After Gen 1 GC: {GC.GetGeneration(obj)}");

                GC.Collect(2);
                Console.WriteLine($"After Gen 2 GC: {GC.GetGeneration(obj)}");
            }

            // Example 5: Finalizers Demo
            static void Example5_FinalizersDemo()
            {
                Console.WriteLine("\n=== Example 5: Finalizers Demo ===");

                CreateFinalizableObjects();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect(); // Second collection to clean up finalizable objects

                Console.WriteLine("Finalizers should have run");
            }

            static void CreateFinalizableObjects()
            {
                for (int i = 0; i < 5; i++)
                {
                    var obj = new FinalizableObject(i);
                }
            }

            // Example 6: Disposable Pattern
            static void Example6_DisposablePattern()
            {
                Console.WriteLine("\n=== Example 6: Disposable Pattern ===");

                using (var resource = new DisposableResource())
                {
                    Console.WriteLine("Using disposable resource");
                } // Dispose called automatically here

                Console.WriteLine("Resource disposed");
            }

            // Example 7: Memory Pressure
            static void Example7_MemoryPressure()
            {
                Console.WriteLine("\n=== Example 7: Memory Pressure ===");

                const long pressureAmount = 10_000_000; // 10MB

                Console.WriteLine($"Adding memory pressure: {pressureAmount:N0} bytes");
                GC.AddMemoryPressure(pressureAmount);

                Console.WriteLine($"Gen 0 collections: {GC.CollectionCount(0)}");

                // Simulate cleanup
                Thread.Sleep(100);

                GC.RemoveMemoryPressure(pressureAmount);
                Console.WriteLine("Memory pressure removed");
            }

            // Example 8: Suppress Finalize
            static void Example8_SuppressFinalize()
            {
                Console.WriteLine("\n=== Example 8: Suppress Finalize ===");

                var obj1 = new FinalizableDisposable("Object1");
                var obj2 = new FinalizableDisposable("Object2");

                obj1.Dispose(); // This will suppress finalization
                // obj2 will be finalized

                obj1 = null;
                obj2 = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // Example 9: GC Notifications (simplified)
            static void Example9_GCNotifications()
            {
                Console.WriteLine("\n=== Example 9: GC Notifications ===");

                Console.WriteLine($"GC Latency Mode: {GCSettings.LatencyMode}");
                Console.WriteLine($"Is Server GC: {GCSettings.IsServerGC}");

                // Change latency mode temporarily
                var originalMode = GCSettings.LatencyMode;
                GCSettings.LatencyMode = GCLatencyMode.LowLatency;
                Console.WriteLine($"Changed to: {GCSettings.LatencyMode}");

                GCSettings.LatencyMode = originalMode;
                Console.WriteLine($"Restored to: {GCSettings.LatencyMode}");
            }

            // Example 10: Concurrent GC Demonstration
            static void Example10_ConcurrentGC()
            {
                Console.WriteLine("\n=== Example 10: Concurrent GC ===");

                var tasks = new List<System.Threading.Tasks.Task>();

                for (int i = 0; i < 3; i++)
                {
                    int taskId = i;
                    tasks.Add(System.Threading.Tasks.Task.Run(() => AllocateMemory(taskId)));
                }

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                Console.WriteLine("All allocation tasks completed");
            }

            static void AllocateMemory(int taskId)
            {
                for (int i = 0; i < 10000; i++)
                {
                    var obj = new byte[1000];
                }
                Console.WriteLine($"Task {taskId} completed allocation");
            }

            // Example 11: String Interning
            static void Example11_StringInterning()
            {
                Console.WriteLine("\n=== Example 11: String Interning ===");

                string str1 = "Hello World";
                string str2 = "Hello" + " " + "World";
                string str3 = new string("Hello World".ToCharArray());

                Console.WriteLine($"str1 == str2: {object.ReferenceEquals(str1, str2)}");
                Console.WriteLine($"str1 == str3: {object.ReferenceEquals(str1, str3)}");

                string internedStr3 = string.Intern(str3);
                Console.WriteLine($"str1 == interned str3: {object.ReferenceEquals(str1, internedStr3)}");
            }

            // Example 12: Array Pooling
            static void Example12_ArrayPooling()
            {
                Console.WriteLine("\n=== Example 12: Array Pooling ===");

                var pool = System.Buffers.ArrayPool<byte>.Shared;

                var array1 = pool.Rent(1000);
                Console.WriteLine($"Rented array length: {array1.Length}");

                pool.Return(array1);
                Console.WriteLine("Array returned to pool");

                var array2 = pool.Rent(1000);
                Console.WriteLine($"Second rental same reference: {object.ReferenceEquals(array1, array2)}");

                pool.Return(array2);
            }

            // Example 13: Event Handler Memory Leaks
            static void Example13_EventHandlerLeaks()
            {
                Console.WriteLine("\n=== Example 13: Event Handler Leaks ===");

                var publisher = new EventPublisher();
                var subscriber = new EventSubscriber();

                // Subscribe - this creates a strong reference
                publisher.SomethingHappened += subscriber.HandleEvent;

                Console.WriteLine("Event subscription created");

                // Even if we null the subscriber, it won't be GC'd due to event handler
                subscriber = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                publisher.FireEvent();

                // Proper cleanup
                //publisher.SomethingHappened -= subscriber?.HandleEvent;
            }

            // Example 14: Boxing and Unboxing
            static void Example14_BoxingUnboxing()
            {
                Console.WriteLine("\n=== Example 14: Boxing and Unboxing ===");

                long memoryBefore = GC.GetTotalMemory(false);

                // Boxing creates objects on the heap
                var boxedValues = new List<object>();
                for (int i = 0; i < 10000; i++)
                {
                    boxedValues.Add(i); // Boxing
                }

                long memoryAfter = GC.GetTotalMemory(false);
                Console.WriteLine($"Memory used by boxing: {memoryAfter - memoryBefore:N0} bytes");

                // Unboxing
                int sum = 0;
                foreach (object boxedValue in boxedValues)
                {
                    sum += (int)boxedValue; // Unboxing
                }
                Console.WriteLine($"Sum after unboxing: {sum:N0}");
            }

            // Example 15: Value Types vs Reference Types
            static void Example15_ValueVsReferenceTypes()
            {
                Console.WriteLine("\n=== Example 15: Value vs Reference Types ===");

                long memoryBefore = GC.GetTotalMemory(false);

                // Value types (on stack, no GC pressure)
                var valueTypes = new List<int>();
                for (int i = 0; i < 100000; i++)
                {
                    valueTypes.Add(i);
                }

                long memoryAfterValues = GC.GetTotalMemory(false);

                // Reference types (on heap, GC pressure)
                var referenceTypes = new List<string>();
                for (int i = 0; i < 100000; i++)
                {
                    referenceTypes.Add($"String {i}");
                }

                long memoryAfterReferences = GC.GetTotalMemory(false);

                Console.WriteLine($"Memory for value types: {memoryAfterValues - memoryBefore:N0} bytes");
                Console.WriteLine($"Memory for reference types: {memoryAfterReferences - memoryAfterValues:N0} bytes");
            }

            // Example 16: Static References Keep Objects Alive
            static void Example16_StaticReferences()
            {
                Console.WriteLine("\n=== Example 16: Static References ===");

                StaticReferenceHolder.HoldReference(new LargeObject());

                // Even though we don't have a local reference, the object is kept alive
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine($"Static reference alive: {StaticReferenceHolder.Reference != null}");

                StaticReferenceHolder.ClearReference();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine("Static reference cleared");
            }

            // Example 17: Circular References
            static void Example17_CircularReferences()
            {
                Console.WriteLine("\n=== Example 17: Circular References ===");

                CreateCircularReferences();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Console.WriteLine("Circular references should be collected (GC handles cycles)");
            }

            static void CreateCircularReferences()
            {
                var node1 = new CircularNode("Node1");
                var node2 = new CircularNode("Node2");

                node1.Reference = node2;
                node2.Reference = node1;

                Console.WriteLine("Created circular references");
            }

            // Example 18: Wait for Finalizers
            static void Example18_WaitForFinalizers()
            {
                Console.WriteLine("\n=== Example 18: Wait for Finalizers ===");

                CreateObjectsWithFinalizers();

                Console.WriteLine("Starting GC...");
                GC.Collect();

                Console.WriteLine("Waiting for finalizers...");
                GC.WaitForPendingFinalizers();

                Console.WriteLine("All finalizers completed");
            }

            static void CreateObjectsWithFinalizers()
            {
                for (int i = 0; i < 3; i++)
                {
                    var obj = new SlowFinalizableObject(i);
                }
            }

            // Example 19: Total Memory Tracking
            static void Example19_TotalMemoryTracking()
            {
                Console.WriteLine("\n=== Example 19: Total Memory Tracking ===");

                long startMemory = GC.GetTotalMemory(true); // Force GC first
                Console.WriteLine($"Start memory: {startMemory:N0} bytes");

                var objects = new List<byte[]>();
                for (int i = 0; i < 100; i++)
                {
                    objects.Add(new byte[10000]);

                    if (i % 20 == 0)
                    {
                        long currentMemory = GC.GetTotalMemory(false);
                        Console.WriteLine($"After {i + 1} allocations: {currentMemory:N0} bytes (+{currentMemory - startMemory:N0})");
                    }
                }

                objects.Clear();
                long endMemory = GC.GetTotalMemory(true);
                Console.WriteLine($"End memory after cleanup: {endMemory:N0} bytes");
            }

            // Example 20: Allocation Patterns
            static void Example20_AllocationPatterns()
            {
                Console.WriteLine("\n=== Example 20: Allocation Patterns ===");

                Console.WriteLine("Testing different allocation patterns...");

                // Pattern 1: Rapid small allocations
                var start = DateTime.Now;
                for (int i = 0; i < 100000; i++)
                {
                    var obj = new SmallObject(i);
                }
                var rapidTime = DateTime.Now - start;
                Console.WriteLine($"Rapid small allocations: {rapidTime.TotalMilliseconds:F2}ms");

                GC.Collect();

                // Pattern 2: Fewer large allocations
                start = DateTime.Now;
                for (int i = 0; i < 100; i++)
                {
                    var obj = new LargeObject();
                }
                var largeTime = DateTime.Now - start;
                Console.WriteLine($"Large allocations: {largeTime.TotalMilliseconds:F2}ms");

                // Pattern 3: Pooled allocations
                start = DateTime.Now;
                var pool = new Queue<SmallObject>();
                for (int i = 0; i < 1000; i++)
                {
                    if (pool.Count > 0)
                    {
                        var reused = pool.Dequeue();
                        reused.Reset(i);
                    }
                    else
                    {
                        var obj = new SmallObject(i);
                        pool.Enqueue(obj);
                    }
                }
                var pooledTime = DateTime.Now - start;
                Console.WriteLine($"Pooled allocations: {pooledTime.TotalMilliseconds:F2}ms");

                Console.WriteLine($"\nGC Gen 0: {GC.CollectionCount(0)}, Gen 1: {GC.CollectionCount(1)}, Gen 2: {GC.CollectionCount(2)}");
            }
        }

        // Supporting classes for the examples
        class LargeObject
        {
            private byte[] _data = new byte[50000]; // 50KB
            public LargeObject() { }
        }

        class FinalizableObject
        {
            private int _id;

            public FinalizableObject(int id)
            {
                _id = id;
            }

            ~FinalizableObject()
            {
                Console.WriteLine($"Finalizing object {_id}");
            }
        }

        class DisposableResource : IDisposable
        {
            private bool _disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        Console.WriteLine("DisposableResource: Managed resources cleaned up");
                    }
                    Console.WriteLine("DisposableResource: Unmanaged resources cleaned up");
                    _disposed = true;
                }
            }

            ~DisposableResource()
            {
                Dispose(false);
            }
        }

        class FinalizableDisposable : IDisposable
        {
            private string _name;
            private bool _disposed = false;

            public FinalizableDisposable(string name)
            {
                _name = name;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                Console.WriteLine($"{_name}: Disposed (finalization suppressed)");
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;
                }
            }

            ~FinalizableDisposable()
            {
                Console.WriteLine($"{_name}: Finalized");
            }
        }

        class EventPublisher
        {
            public event Action<string> SomethingHappened;

            public void FireEvent()
            {
                SomethingHappened?.Invoke("Event fired!");
            }
        }

        class EventSubscriber
        {
            public void HandleEvent(string message)
            {
                Console.WriteLine($"EventSubscriber handled: {message}");
            }

            ~EventSubscriber()
            {
                Console.WriteLine("EventSubscriber finalized");
            }
        }

        static class StaticReferenceHolder
        {
            public static object Reference { get; private set; }

            public static void HoldReference(object obj)
            {
                Reference = obj;
            }

            public static void ClearReference()
            {
                Reference = null;
            }
        }

        class CircularNode
        {
            public string Name { get; }
            public CircularNode Reference { get; set; }

            public CircularNode(string name)
            {
                Name = name;
            }

            ~CircularNode()
            {
                Console.WriteLine($"CircularNode {Name} finalized");
            }
        }

        class SlowFinalizableObject
        {
            private int _id;

            public SlowFinalizableObject(int id)
            {
                _id = id;
            }

            ~SlowFinalizableObject()
            {
                Console.WriteLine($"Slow finalizer {_id} starting...");
                Thread.Sleep(100); // Simulate slow cleanup
                Console.WriteLine($"Slow finalizer {_id} completed");
            }
        }

        class SmallObject
        {
            public int Id { get; private set; }
            private int[] _data = new int[10];

            public SmallObject(int id)
            {
                Id = id;
            }

            public void Reset(int newId)
            {
                Id = newId;
            }
        }
    }
