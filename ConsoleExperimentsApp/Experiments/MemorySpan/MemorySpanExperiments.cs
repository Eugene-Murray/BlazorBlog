using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExperimentsApp.Experiments.MemorySpan
{
    public static class MemorySpanExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("MemorySpanExperiments");
            Console.ResetColor();

            // Run all 20 examples
            Example1_BasicSpanCreation();
            Example2_BasicMemoryCreation();
            Example3_SpanSlicing();
            Example4_MemorySlicing();
            Example5_ReadOnlySpanUsage();
            Example6_ReadOnlyMemoryUsage();
            Example7_StringManipulationWithSpan();
            Example8_StackallocWithSpan();
            Example9_MemoryPooling();
            Example10_SpanForParsing();
            Example11_MemoryForAsync().GetAwaiter().GetResult();
            Example12_SpanForBufferOperations();
            Example13_ConvertingBetweenMemoryAndSpan();
            Example14_SpanForStructManipulation();
            Example15_MemoryForStreaming();
            Example16_SpanForArraySegmentReplacement();
            Example17_ReadOnlySpanForConstants();
            Example18_EmptySpanAndMemory();
            Example19_SpanForByteOperations();
            Example20_MemoryOwnerPattern();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
        }

        // Example 1: Basic Span creation from array
        private static void Example1_BasicSpanCreation()
        {
            Console.WriteLine("\n1. Basic Span Creation:");
            int[] array = { 1, 2, 3, 4, 5 };
            Span<int> span = array;

            Console.WriteLine($"Original array length: {array.Length}");
            Console.WriteLine($"Span length: {span.Length}");

            // Modify through span
            span[0] = 10;
            Console.WriteLine($"Modified first element via span: {array[0]}");
        }

        // Example 2: Basic Memory creation from array
        private static void Example2_BasicMemoryCreation()
        {
            Console.WriteLine("\n2. Basic Memory Creation:");
            int[] array = { 10, 20, 30, 40, 50 };
            Memory<int> memory = array;

            Console.WriteLine($"Memory length: {memory.Length}");
            Span<int> span = memory.Span;
            Console.WriteLine($"First element through Memory.Span: {span[0]}");
        }

        // Example 3: Span slicing operations
        private static void Example3_SpanSlicing()
        {
            Console.WriteLine("\n3. Span Slicing:");
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Span<int> fullSpan = array;

            Span<int> slice1 = fullSpan[2..5];  // Elements 3, 4, 5
            Span<int> slice2 = fullSpan[^3..];  // Last 3 elements

            Console.WriteLine($"Full span length: {fullSpan.Length}");
            Console.WriteLine($"Slice1 (2..5) length: {slice1.Length}, first element: {slice1[0]}");
            Console.WriteLine($"Slice2 (^3..) length: {slice2.Length}, first element: {slice2[0]}");
        }

        // Example 4: Memory slicing operations
        private static void Example4_MemorySlicing()
        {
            Console.WriteLine("\n4. Memory Slicing:");
            int[] array = { 11, 22, 33, 44, 55, 66, 77, 88, 99 };
            Memory<int> fullMemory = array;

            Memory<int> slice = fullMemory.Slice(2, 4);  // 4 elements starting at index 2
            Console.WriteLine($"Original memory length: {fullMemory.Length}");
            Console.WriteLine($"Sliced memory length: {slice.Length}");
            Console.WriteLine($"First element of slice: {slice.Span[0]}");
        }

        // Example 5: ReadOnlySpan usage
        private static void Example5_ReadOnlySpanUsage()
        {
            Console.WriteLine("\n5. ReadOnlySpan Usage:");
            string text = "Hello World";
            ReadOnlySpan<char> span = text.AsSpan();

            ReadOnlySpan<char> hello = span[..5];
            ReadOnlySpan<char> world = span[6..];

            Console.WriteLine($"Original: {text}");
            Console.WriteLine($"Hello part: {hello.ToString()}");
            Console.WriteLine($"World part: {world.ToString()}");
        }

        // Example 6: ReadOnlyMemory usage
        private static void Example6_ReadOnlyMemoryUsage()
        {
            Console.WriteLine("\n6. ReadOnlyMemory Usage:");
            string text = "Immutable Data";
            ReadOnlyMemory<char> memory = text.AsMemory();

            ReadOnlySpan<char> span = memory.Span;
            Console.WriteLine($"Memory length: {memory.Length}");
            Console.WriteLine($"Content through span: {span.ToString()}");
        }

        // Example 7: String manipulation with Span
        private static void Example7_StringManipulationWithSpan()
        {
            Console.WriteLine("\n7. String Manipulation with Span:");
            string input = "  Hello World  ";
            ReadOnlySpan<char> span = input.AsSpan();

            ReadOnlySpan<char> trimmed = span.Trim();
            ReadOnlySpan<char> upper = "HELLO".AsSpan();

            Console.WriteLine($"Original: '{input}'");
            Console.WriteLine($"Trimmed: '{trimmed.ToString()}'");
            Console.WriteLine($"Starts with HELLO: {trimmed.StartsWith(upper, StringComparison.OrdinalIgnoreCase)}");
        }

        // Example 8: Stackalloc with Span
        private static void Example8_StackallocWithSpan()
        {
            Console.WriteLine("\n8. Stackalloc with Span:");
            Span<int> buffer = stackalloc int[10];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = i * i;
            }

            Console.WriteLine($"Stack-allocated buffer length: {buffer.Length}");
            Console.WriteLine($"First few squares: {buffer[0]}, {buffer[1]}, {buffer[2]}, {buffer[3]}");
        }

        // Example 9: Memory pooling
        private static void Example9_MemoryPooling()
        {
            Console.WriteLine("\n9. Memory Pooling:");
            var pool = ArrayPool<byte>.Shared;
            byte[] rented = pool.Rent(1024);

            try
            {
                Memory<byte> memory = rented.AsMemory(0, 100); // Use only first 100 bytes
                Span<byte> span = memory.Span;

                // Fill with pattern
                for (int i = 0; i < span.Length; i++)
                {
                    span[i] = (byte)(i % 256);
                }

                Console.WriteLine($"Rented array length: {rented.Length}");
                Console.WriteLine($"Used memory length: {memory.Length}");
                Console.WriteLine($"First few bytes: {span[0]}, {span[1]}, {span[2]}");
            }
            finally
            {
                pool.Return(rented);
            }
        }

        // Example 10: Span for parsing operations
        private static void Example10_SpanForParsing()
        {
            Console.WriteLine("\n10. Span for Parsing:");
            ReadOnlySpan<char> numberText = "12345".AsSpan();
            ReadOnlySpan<char> floatText = "123.45".AsSpan();

            if (int.TryParse(numberText, out int intResult))
            {
                Console.WriteLine($"Parsed integer: {intResult}");
            }

            if (double.TryParse(floatText, out double doubleResult))
            {
                Console.WriteLine($"Parsed double: {doubleResult}");
            }
        }

        // Example 11: Memory for async operations
        private static async Task Example11_MemoryForAsync()
        {
            Console.WriteLine("\n11. Memory for Async Operations:");
            byte[] data = Encoding.UTF8.GetBytes("Hello Async World");
            Memory<byte> memory = data;

            // Simulate async operation that works with Memory<T>
            await ProcessDataAsync(memory);
        }

        private static async Task ProcessDataAsync(Memory<byte> memory)
        {
            await Task.Delay(1); // Simulate async work
            string text = Encoding.UTF8.GetString(memory.Span);
            Console.WriteLine($"Processed async data: {text}");
        }

        // Example 12: Span for buffer operations
        private static void Example12_SpanForBufferOperations()
        {
            Console.WriteLine("\n12. Span for Buffer Operations:");
            Span<byte> buffer1 = new byte[10];
            Span<byte> buffer2 = new byte[10];

            // Fill buffers
            for (int i = 0; i < buffer1.Length; i++)
            {
                buffer1[i] = (byte)(i + 1);
                buffer2[i] = (byte)(i + 1);
            }

            // Compare buffers
            bool areEqual = buffer1.SequenceEqual(buffer2);
            Console.WriteLine($"Buffers are equal: {areEqual}");

            // Copy operation
            Span<byte> destination = new byte[5];
            buffer1[..5].CopyTo(destination);
            Console.WriteLine($"Copied first 5 bytes: {destination[0]}, {destination[1]}, {destination[2]}");
        }

        // Example 13: Converting between Memory and Span
        private static void Example13_ConvertingBetweenMemoryAndSpan()
        {
            Console.WriteLine("\n13. Converting Between Memory and Span:");
            int[] array = { 100, 200, 300, 400, 500 };

            Memory<int> memory = array;
            Span<int> span = memory.Span;
            ReadOnlyMemory<int> readOnlyMemory = memory;
            ReadOnlySpan<int> readOnlySpan = readOnlyMemory.Span;

            Console.WriteLine($"Memory length: {memory.Length}");
            Console.WriteLine($"Span length: {span.Length}");
            Console.WriteLine($"ReadOnlyMemory length: {readOnlyMemory.Length}");
            Console.WriteLine($"ReadOnlySpan length: {readOnlySpan.Length}");
        }

        // Example 14: Span for struct manipulation
        private static void Example14_SpanForStructManipulation()
        {
            Console.WriteLine("\n14. Span for Struct Manipulation:");
            var points = new Point[]
            {
                new Point(1, 2),
                new Point(3, 4),
                new Point(5, 6)
            };

            Span<Point> span = points;

            // Modify structs in place
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = new Point(span[i].X * 2, span[i].Y * 2);
            }

            Console.WriteLine("Doubled points:");
            foreach (var point in points)
            {
                Console.WriteLine($"({point.X}, {point.Y})");
            }
        }

        // Example 15: Memory for streaming operations
        private static void Example15_MemoryForStreaming()
        {
            Console.WriteLine("\n15. Memory for Streaming Operations:");
            byte[] buffer = new byte[1024];
            Memory<byte> memory = buffer;

            // Fill with sample data
            string sampleData = "This is sample streaming data";
            byte[] data = Encoding.UTF8.GetBytes(sampleData);
            data.CopyTo(memory.Span);

            Memory<byte> usedMemory = memory[..data.Length];
            string result = Encoding.UTF8.GetString(usedMemory.Span);
            Console.WriteLine($"Stream data: {result}");
        }

        // Example 16: Span for array segment replacement
        private static void Example16_SpanForArraySegmentReplacement()
        {
            Console.WriteLine("\n16. Span for ArraySegment Replacement:");
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Old way with ArraySegment
            ArraySegment<int> segment = new ArraySegment<int>(array, 2, 5);
            Console.WriteLine($"ArraySegment offset: {segment.Offset}, count: {segment.Count}");

            // New way with Span
            Span<int> span = array.AsSpan(2, 5);
            Console.WriteLine($"Span length: {span.Length}, first element: {span[0]}");

            // Span is more efficient and easier to use
            int sum = 0;
            foreach (int value in span)
            {
                sum += value;
            }
            Console.WriteLine($"Sum of span elements: {sum}");
        }

        // Example 17: ReadOnlySpan for constants
        private static void Example17_ReadOnlySpanForConstants()
        {
            Console.WriteLine("\n17. ReadOnlySpan for Constants:");
            ReadOnlySpan<char> vowels = ['a', 'e', 'i', 'o', 'u'];
            ReadOnlySpan<char> testWord = "hello".AsSpan();

            int vowelCount = 0;
            foreach (char c in testWord)
            {
                if (vowels.Contains(c))
                {
                    vowelCount++;
                }
            }

            Console.WriteLine($"Vowels in 'hello': {vowelCount}");
        }

        // Example 18: Empty Span and Memory
        private static void Example18_EmptySpanAndMemory()
        {
            Console.WriteLine("\n18. Empty Span and Memory:");
            Span<int> emptySpan = Span<int>.Empty;
            Memory<int> emptyMemory = Memory<int>.Empty;

            Console.WriteLine($"Empty span length: {emptySpan.Length}");
            Console.WriteLine($"Empty memory length: {emptyMemory.Length}");
            Console.WriteLine($"Empty span is empty: {emptySpan.IsEmpty}");
            Console.WriteLine($"Empty memory is empty: {emptyMemory.IsEmpty}");
        }

        // Example 19: Span for byte operations
        private static void Example19_SpanForByteOperations()
        {
            Console.WriteLine("\n19. Span for Byte Operations:");
            byte[] data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello" in ASCII
            Span<byte> span = data;

            // Reverse the bytes
            span.Reverse();

            string reversed = Encoding.ASCII.GetString(span);
            Console.WriteLine($"Reversed bytes as string: {reversed}");

            // Fill with pattern
            span.Fill(0xFF);
            Console.WriteLine($"After fill with 0xFF: {span[0]:X2}, {span[1]:X2}, {span[2]:X2}");
        }

        // Example 20: Memory owner pattern
        private static void Example20_MemoryOwnerPattern()
        {
            Console.WriteLine("\n20. Memory Owner Pattern:");
            using IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(1024);
            Memory<byte> memory = owner.Memory[..100]; // Use first 100 bytes

            Span<byte> span = memory.Span;

            // Initialize with sequence
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = (byte)(i % 10);
            }

            Console.WriteLine($"Memory owner length: {owner.Memory.Length}");
            Console.WriteLine($"Used memory length: {memory.Length}");
            Console.WriteLine($"Pattern: {span[0]}, {span[1]}, {span[2]}, ..., {span[9]}, {span[10]}");

            // Memory is automatically returned to pool when owner is disposed
        }

        private struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
