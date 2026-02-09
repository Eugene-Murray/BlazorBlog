using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleExperimentsApp.Experiments.ExtensionMethod
{
    public class ExtensionMethodExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Extension Methods and Blocks Experiments ===");
            Console.ResetColor();

            // Example 1: Basic Extension Methods on String
            Console.WriteLine("\n1. Basic String Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates how to extend the built-in string type with custom");
            Console.WriteLine("   methods like WordCount, ReverseString, and IsPalindrome.");
            Console.ResetColor();
            string text = "Hello World";
            Console.WriteLine($"Original: {text}");
            Console.WriteLine($"Word Count: {text.WordCount()}");
            Console.WriteLine($"Reverse: {text.ReverseString()}");
            Console.WriteLine($"Is Palindrome: {text.IsPalindrome()}");
            Console.WriteLine($"Is Palindrome ('racecar'): {"racecar".IsPalindrome()}");

            // Example 2: Extension Methods on Int
            Console.WriteLine("\n2. Integer Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to add mathematical and utility methods to integer types");
            Console.WriteLine("   such as Factorial, IsEven, IsPrime, and Square.");
            Console.ResetColor();
            int number = 5;
            Console.WriteLine($"{number}! = {number.Factorial()}");
            Console.WriteLine($"Is {number} Even? {number.IsEven()}");
            Console.WriteLine($"Is {number} Prime? {number.IsPrime()}");
            Console.WriteLine($"{number} squared = {number.Square()}");

            // Example 3: Extension Methods on Collections
            Console.WriteLine("\n3. Collection Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates extending collection types with custom algorithms");
            Console.WriteLine("   like Median, StandardDeviation, and Shuffle operations.");
            Console.ResetColor();
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Console.WriteLine($"Original: {string.Join(", ", numbers)}");
            Console.WriteLine($"Median: {numbers.Median()}");
            Console.WriteLine($"Standard Deviation: {numbers.StandardDeviation():F2}");
            numbers.Shuffle();
            Console.WriteLine($"Shuffled: {string.Join(", ", numbers)}");

            // Example 4: Extension Methods on Custom Types
            Console.WriteLine("\n4. Extension Methods on Custom Types:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to extend user-defined classes with additional");
            Console.WriteLine("   functionality without modifying the original class definition.");
            Console.ResetColor();
            var person = new PersionExtensions { FirstName = "John", LastName = "Doe", Age = 30 };
            Console.WriteLine(person.GetFullDetails());
            Console.WriteLine($"Is Adult: {person.IsAdult()}");
            Console.WriteLine($"Age Category: {person.GetAgeCategory()}");

            // Example 5: Chaining Extension Methods
            Console.WriteLine("\n5. Chaining Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates how extension methods that return the same type can be");
            Console.WriteLine("   chained together to create fluent, readable code.");
            Console.ResetColor();
            string sentence = "  this is a test sentence  ";
            var result = sentence.Trim()
                                .ToTitleCase()
                                .WrapInBrackets()
                                .AddPrefix("Result: ");
            Console.WriteLine(result);

            // Example 6: Generic Extension Methods
            Console.WriteLine("\n6. Generic Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to create extension methods using generic type parameters");
            Console.WriteLine("   that work with any type while maintaining type safety.");
            Console.ResetColor();
            var list1 = new List<int> { 1, 2, 3 };
            var list2 = new List<string> { "a", "b", "c" };
            Console.WriteLine($"Int list is null or empty: {list1.IsNullOrEmpty()}");
            Console.WriteLine($"First or default with predicate: {list1.FirstOrDefault(x => x > 2)}");
            list2.ForEach(item => Console.WriteLine($"  Item: {item}"));

            // Example 7: Extension Methods on Interfaces
            Console.WriteLine("\n7. Extension Methods on Interfaces:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates extending interface types, which applies the extension");
            Console.WriteLine("   to all implementations of that interface.");
            Console.ResetColor();
            IEnumerable<int> enumerable = new[] { 1, 2, 3, 4, 5 };
            Console.WriteLine($"Sum using extension: {enumerable.SumValues()}");
            Console.WriteLine($"Comma separated: {enumerable.ToCommaSeparated()}");

            // Example 8: Null-Safe Extension Methods
            Console.WriteLine("\n8. Null-Safe Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to create extension methods that safely handle null values");
            Console.WriteLine("   using nullable reference types and defensive programming.");
            Console.ResetColor();
            string? nullString = null;
            string? validString = "test";
            Console.WriteLine($"Null string is null or empty: {nullString.IsNullOrEmpty()}");
            Console.WriteLine($"Valid string is null or empty: {validString.IsNullOrEmpty()}");
            Console.WriteLine($"Null string with default: {nullString.OrDefault("N/A")}");

            // Example 9: Extension Methods with Multiple Parameters
            Console.WriteLine("\n9. Extension Methods with Multiple Parameters:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates extension methods that accept additional parameters");
            Console.WriteLine("   beyond the 'this' parameter for more complex operations.");
            Console.ResetColor();
            string template = "Hello, {0}! Welcome to {1}.";
            Console.WriteLine(template.FormatWith("Alice", "C# World"));
            var range = 10.To(20);
            Console.WriteLine($"Range 10-20: {string.Join(", ", range)}");

            // Example 10: Extension Block Scope (C# 14 feature)
            Console.WriteLine("\n10. Extension Block Scope (C# 14):");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates C# 14's extension block feature that allows defining");
            Console.WriteLine("   extension methods with limited scope, reducing namespace pollution.");
            Console.ResetColor();
            DemonstrateExtensionBlocks();

            // Example 11: Fluent API using Extension Methods
            Console.WriteLine("\n11. Fluent API using Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to use extension methods to create fluent interfaces");
            Console.WriteLine("   with method chaining for building objects in a readable way.");
            Console.ResetColor();
            var fluentResult = FluentBuilder
                .Create()
                .WithName("My Object")
                .WithValue(42)
                .WithEnabled(true)
                .Build();
            Console.WriteLine(fluentResult);

            // Example 12: LINQ-style Extension Methods
            Console.WriteLine("\n12. Custom LINQ-style Extension Methods:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates creating custom LINQ-like extension methods that provide");
            Console.WriteLine("   deferred execution and composability for data processing pipelines.");
            Console.ResetColor();
            var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var filtered = data.WhereEven().TakeWhileLessThan(7);
            Console.WriteLine($"Even numbers less than 7: {string.Join(", ", filtered)}");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static void DemonstrateExtensionBlocks()
        {
            Console.WriteLine("Extension blocks allow scoped extensions:");
            var value = 42;
            Console.WriteLine($"Value: {value}");
            Console.WriteLine($"Doubled: {value.Double()}");
            Console.WriteLine($"As currency: {value.ToCurrency()}");
        }
    }

    // Extension Methods for String
    public static class StringExtensions
    {
        public static int WordCount(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0;
            return str.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string ReverseString(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public static bool IsPalindrome(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            str = str.ToLower().Replace(" ", "");
            return str == str.ReverseString();
        }

        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = str.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }

        public static string WrapInBrackets(this string str)
        {
            return $"[{str}]";
        }

        public static string AddPrefix(this string str, string prefix)
        {
            return prefix + str;
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string OrDefault(this string? str, string defaultValue)
        {
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }
    }

    // Extension Methods for Int
    public static class IntExtensions
    {
        public static long Factorial(this int n)
        {
            if (n < 0) throw new ArgumentException("Number must be non-negative");
            if (n == 0) return 1;
            long result = 1;
            for (int i = 1; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        public static bool IsEven(this int n)
        {
            return n % 2 == 0;
        }

        public static bool IsPrime(this int n)
        {
            if (n <= 1) return false;
            if (n == 2) return true;
            if (n % 2 == 0) return false;

            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        public static int Square(this int n)
        {
            return n * n;
        }

        public static IEnumerable<int> To(this int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                yield return i;
            }
        }

        public static int Double(this int n)
        {
            return n * 2;
        }

        public static string ToCurrency(this int n)
        {
            return $"${n:N2}";
        }
    }

    // Extension Methods for Collections
    public static class CollectionExtensions
    {
        public static double Median(this IEnumerable<int> source)
        {
            var sorted = source.OrderBy(x => x).ToList();
            int count = sorted.Count;
            if (count == 0) return 0;

            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            else
            {
                return sorted[count / 2];
            }
        }

        public static double StandardDeviation(this IEnumerable<int> source)
        {
            var list = source.ToList();
            if (list.Count == 0) return 0;

            double avg = list.Average();
            double sumOfSquares = list.Sum(x => Math.Pow(x - avg, 2));
            return Math.Sqrt(sumOfSquares / list.Count);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        {
            return source == null || !source.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<int> WhereEven(this IEnumerable<int> source)
        {
            return source.Where(x => x % 2 == 0);
        }

        public static IEnumerable<int> TakeWhileLessThan(this IEnumerable<int> source, int threshold)
        {
            return source.TakeWhile(x => x < threshold);
        }
    }

    // Extension Methods for Interfaces
    public static class InterfaceExtensions
    {
        public static int SumValues(this IEnumerable<int> source)
        {
            return source.Sum();
        }

        public static string ToCommaSeparated<T>(this IEnumerable<T> source)
        {
            return string.Join(", ", source);
        }
    }

    // Custom Type for Demonstration
    public class PersionExtensions
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    // Extension Methods for Custom Types
    public static class PersionExtensionsExtensions
    {
        public static string GetFullDetails(this PersionExtensions person)
        {
            return $"{person.FirstName} {person.LastName}, Age: {person.Age}";
        }

        public static bool IsAdult(this PersionExtensions person)
        {
            return person.Age >= 18;
        }

        public static string GetAgeCategory(this PersionExtensions person)
        {
            return person.Age switch
            {
                < 13 => "Child",
                < 20 => "Teenager",
                < 65 => "Adult",
                _ => "Senior"
            };
        }
    }

    // Fluent Builder Pattern using Extension Methods
    public class FluentBuilder
    {
        private string _name = string.Empty;
        private int _value;
        private bool _enabled;

        private FluentBuilder() { }

        public static FluentBuilder Create() => new FluentBuilder();

        public FluentBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FluentBuilder WithValue(int value)
        {
            _value = value;
            return this;
        }

        public FluentBuilder WithEnabled(bool enabled)
        {
            _enabled = enabled;
            return this;
        }

        public string Build()
        {
            return $"FluentObject: Name={_name}, Value={_value}, Enabled={_enabled}";
        }
    }
}
