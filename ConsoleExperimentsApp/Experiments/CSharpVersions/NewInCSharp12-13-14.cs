using System;
using System.Collections.Generic;
using System.Text;



namespace ConsoleExperimentsApp.Experiments.CSharpVersions
{

    // C# 12: Using alias for tuple types
    using Coordinate = (double Latitude, double Longitude);
    using NameAge = (string Name, int Age);
    public static class NewInCSharp12_13_14
    {
        public static async Task Run()
        {
            Console.WriteLine("=== New in C# 12, 13, and 14 Experiments ===\n");

            // C# 12 Features
            CSharp12_PrimaryConstructors();
            CSharp12_CollectionExpressions();
            CSharp12_RefReadonlyParameters();
            CSharp12_AliasAnyType();
            CSharp12_DefaultLambdaParameters();

            // C# 13 Features
            CSharp13_ParamsCollections();
            CSharp13_EscapeSequence();
            CSharp13_ImplicitIndexerAccess();
            await CSharp13_RefInAsyncMethods();
            CSharp13_NewLockObject();

            Console.WriteLine("\n=== C# 12, 13, and 14 experiments completed ===");
        }

        #region C# 12 Features

        // C# 12: Primary Constructors for Classes
        private static void CSharp12_PrimaryConstructors()
        {
            Console.WriteLine("\n--- C# 12: Primary Constructors ---");
            var person = new PersonNew("Alice", 30);
            person.Display();

            var point = new Point3D(10, 20, 30);
            Console.WriteLine($"Point: {point}");
        }

        // C# 12: Collection Expressions
        private static void CSharp12_CollectionExpressions()
        {
            Console.WriteLine("\n--- C# 12: Collection Expressions ---");

            // Array initialization
            int[] numbers = [1, 2, 3, 4, 5];
            Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");

            // Spread operator
            int[] moreNumbers = [..numbers, 6, 7, 8];
            Console.WriteLine($"More numbers: {string.Join(", ", moreNumbers)}");

            // List initialization
            List<string> fruits = ["apple", "banana", "cherry"];
            Console.WriteLine($"Fruits: {string.Join(", ", fruits)}");

            // Combining collections
            string[] moreFruits = ["date", "elderberry"];
            List<string> allFruits = [..fruits, ..moreFruits];
            Console.WriteLine($"All fruits: {string.Join(", ", allFruits)}");
        }

        // C# 12: ref readonly parameters
        private static void CSharp12_RefReadonlyParameters()
        {
            Console.WriteLine("\n--- C# 12: ref readonly Parameters ---");

            var largeStruct = new LargeStruct { Value1 = 100, Value2 = 200, Value3 = 300 };
            int sum = CalculateSum(in largeStruct);
            Console.WriteLine($"Sum of struct values: {sum}");
        }

        private static int CalculateSum(ref readonly LargeStruct data)
        {
            // data cannot be modified (readonly)
            return data.Value1 + data.Value2 + data.Value3;
        }

        // C# 12: Alias any type (including tuples, pointers, etc.)
        private static void CSharp12_AliasAnyType()
        {
            Console.WriteLine("\n--- C# 12: Alias Any Type ---");

            Coordinate coord = (45.5, -122.6);
            Console.WriteLine($"Coordinate: Latitude={coord.Item1}, Longitude={coord.Item2}");

            NameAge person = ("Bob", 25);
            Console.WriteLine($"Person: {person.Name}, Age {person.Age}");
        }

        // C# 12: Default lambda parameters
        private static void CSharp12_DefaultLambdaParameters()
        {
            Console.WriteLine("\n--- C# 12: Default Lambda Parameters ---");

            var increment = (int value, int amount = 1) => value + amount;

            Console.WriteLine($"Increment 10: {increment(10)}");
            Console.WriteLine($"Increment 10 by 5: {increment(10, 5)}");

            var greet = (string name, string greeting = "Hello") => $"{greeting}, {name}!";
            Console.WriteLine(greet("Alice"));
            Console.WriteLine(greet("Bob", "Welcome"));
        }

        #endregion

        #region C# 13 Features

        // C# 13: params collections (not just arrays)
        private static void CSharp13_ParamsCollections()
        {
            Console.WriteLine("\n--- C# 13: params Collections ---");

            PrintItems("First", "Second", "Third");
            PrintItems("One");

            ConcatenateSpan("Hello", " ", "World", "!");
        }

        private static void PrintItems(params IEnumerable<string> items)
        {
            Console.WriteLine($"Items: {string.Join(", ", items)}");
        }

        private static void ConcatenateSpan(params ReadOnlySpan<string> items)
        {
            var result = string.Concat(items.ToArray());
            Console.WriteLine($"Concatenated: {result}");
        }

        // C# 13: New escape sequence \e
        private static void CSharp13_EscapeSequence()
        {
            Console.WriteLine("\n--- C# 13: Escape Sequence \\e ---");

            // \e represents the ESC character (0x1B)
            string coloredText = "\e[32mGreen Text\e[0m";
            Console.WriteLine($"ANSI escape sequence: {coloredText}");
            Console.WriteLine("(ESC character code: 0x1B)");
        }

        // C# 13: Implicit indexer access in object initializers
        private static void CSharp13_ImplicitIndexerAccess()
        {
            Console.WriteLine("\n--- C# 13: Implicit Indexer Access ---");

            var timeSeries = new TimeSeries
            {
                [0] = 10.5,
                [1] = 20.3,
                [2] = 15.7
            };

            Console.WriteLine($"Time series values: {timeSeries[0]}, {timeSeries[1]}, {timeSeries[2]}");
        }

        // C# 13: ref and unsafe in async methods
        private static async Task CSharp13_RefInAsyncMethods()
        {
            Console.WriteLine("\n--- C# 13: ref in async methods ---");

            var data = new DataProcessor();
            await data.ProcessAsync();
            Console.WriteLine($"Processed value: {data.GetValue()}");
        }

        // C# 13: New Lock object and pattern-based lock
        private static void CSharp13_NewLockObject()
        {
            Console.WriteLine("\n--- C# 13: New Lock Object ---");

            var counter = new ThreadSafeCounter();
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => Task.Run(() => counter.Increment()))
                .ToArray();

            Task.WaitAll(tasks);
            Console.WriteLine($"Final counter value: {counter.GetValue()}");
        }

        #endregion

        #region Supporting Types

        // C# 12: Primary Constructor for class
        private class PersonNew(string name, int age)
        {
            public string Name { get; } = name;
            public int Age { get; } = age;

            public void Display() => Console.WriteLine($"Person: {Name}, Age: {Age}");
        }

        // C# 12: Primary Constructor for struct
        private readonly struct Point3D(int x, int y, int z)
        {
            public int X { get; } = x;
            public int Y { get; } = y;
            public int Z { get; } = z;

            public override string ToString() => $"({X}, {Y}, {Z})";
        }

        private struct LargeStruct
        {
            public int Value1;
            public int Value2;
            public int Value3;
        }

        private class TimeSeries
        {
            private readonly Dictionary<int, double> _data = new();

            public double this[int index]
            {
                get => _data.TryGetValue(index, out var value) ? value : 0;
                set => _data[index] = value;
            }
        }

        private class DataProcessor
        {
            private int _value = 0;

            public async Task ProcessAsync()
            {
                await Task.Delay(10);
                ref int valueRef = ref _value;
                valueRef = 42;
            }

            public int GetValue() => _value;
        }

        private class ThreadSafeCounter
        {
            private readonly Lock _lock = new();
            private int _count = 0;

            public void Increment()
            {
                lock (_lock)
                {
                    _count++;
                }
            }

            public int GetValue()
            {
                lock (_lock)
                {
                    return _count;
                }
            }
        }

        #endregion
    }
}
