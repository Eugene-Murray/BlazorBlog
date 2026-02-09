using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;

namespace ConsoleExperimentsApp.Experiments.Channels
{
    public class ChannelsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== System.Threading.Channels Experiments ===");
            Console.WriteLine("Description: Demonstrating producer-consumer patterns using System.Threading.Channels for efficient data flow.");
            Console.ResetColor();

            await ChannelExample();
            await BoundedChannelExample();
            await MultipleProducersExample();
            await MultipleConsumersExample();
            await ChannelWithOptionsExample();
            await ErrorHandlingChannelExample();
            await CancellationChannelExample();
            await StringChannelExample();
            await ChannelTransformationExample();
            await PriorityChannelExample();
            await BatchProcessingChannelExample();
            await TimeoutChannelExample();
            await SingleReaderWriterExample();
            await DropOldestChannelExample();
            await WaitToWriteExample();
            await TryWriteReadExample();
            await ChannelClosedExample();
            await BackpressureExample();
            await FanOutExample();
            await PipelineExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        public static async Task ChannelExample()
        {
            Console.WriteLine("\n=== Example 1: Basic Unbounded Channel ===");
            var channel = Channel.CreateUnbounded<int>();

            // Producer
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                }
                channel.Writer.Complete();
            });

            // Consumer
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item}");
            }
        }

        public static async Task BoundedChannelExample()
        {
            Console.WriteLine("\n=== Example 2: Bounded Channel ===");
            var channel = Channel.CreateBounded<int>(3);

            // Producer
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                    await Task.Delay(100);
                }
                channel.Writer.Complete();
            });

            // Consumer (slower than producer)
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item}");
                await Task.Delay(300);
            }
        }

        public static async Task MultipleProducersExample()
        {
            Console.WriteLine("\n=== Example 3: Multiple Producers ===");
            var channel = Channel.CreateUnbounded<string>();

            var producers = new List<Task>();
            for (int producerId = 0; producerId < 3; producerId++)
            {
                int id = producerId;
                producers.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < 3; i++)
                    {
                        await channel.Writer.WriteAsync($"Producer{id}-Item{i}");
                        Console.WriteLine($"Producer {id} produced item {i}");
                        await Task.Delay(200);
                    }
                }));
            }

            _ = Task.Run(async () =>
            {
                await Task.WhenAll(producers);
                channel.Writer.Complete();
            });

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item}");
            }
        }

        public static async Task MultipleConsumersExample()
        {
            Console.WriteLine("\n=== Example 4: Multiple Consumers ===");
            var channel = Channel.CreateUnbounded<int>();

            // Producer
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 12; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                    await Task.Delay(100);
                }
                channel.Writer.Complete();
            });

            // Multiple consumers
            var consumers = new List<Task>();
            for (int consumerId = 0; consumerId < 3; consumerId++)
            {
                int id = consumerId;
                consumers.Add(Task.Run(async () =>
                {
                    await foreach (var item in channel.Reader.ReadAllAsync())
                    {
                        Console.WriteLine($"Consumer {id} consumed: {item}");
                        await Task.Delay(300);
                    }
                }));
            }

            await Task.WhenAll(consumers);
        }

        public static async Task ChannelWithOptionsExample()
        {
            Console.WriteLine("\n=== Example 5: Channel with Options ===");
            var options = new BoundedChannelOptions(5)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            var channel = Channel.CreateBounded<int>(options);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 8; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                }
                channel.Writer.Complete();
            });

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item}");
                await Task.Delay(200);
            }
        }

        public static async Task ErrorHandlingChannelExample()
        {
            Console.WriteLine("\n=== Example 6: Error Handling ===");
            var channel = Channel.CreateUnbounded<int>();

            _ = Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == 3) throw new InvalidOperationException("Producer error!");
                        await channel.Writer.WriteAsync(i);
                        Console.WriteLine($"Produced: {i}");
                    }
                    channel.Writer.Complete();
                }
                catch (Exception ex)
                {
                    channel.Writer.Complete(ex);
                    Console.WriteLine($"Producer failed: {ex.Message}");
                }
            });

            try
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    Console.WriteLine($"Consumed: {item}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Consumer caught exception: {ex.Message}");
            }
        }

        public static async Task CancellationChannelExample()
        {
            Console.WriteLine("\n=== Example 7: Cancellation ===");
            var channel = Channel.CreateUnbounded<int>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            _ = Task.Run(async () =>
            {
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        await channel.Writer.WriteAsync(i, cts.Token);
                        Console.WriteLine($"Produced: {i}");
                        await Task.Delay(300, cts.Token);
                    }
                    channel.Writer.Complete();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Producer was cancelled");
                    channel.Writer.Complete();
                }
            });

            try
            {
                await foreach (var item in channel.Reader.ReadAllAsync(cts.Token))
                {
                    Console.WriteLine($"Consumed: {item}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer was cancelled");
            }
        }

        public static async Task StringChannelExample()
        {
            Console.WriteLine("\n=== Example 8: String Channel ===");
            var channel = Channel.CreateUnbounded<string>();

            _ = Task.Run(async () =>
            {
                string[] words = { "Hello", "World", "Channel", "Example" };
                foreach (var word in words)
                {
                    await channel.Writer.WriteAsync(word);
                    Console.WriteLine($"Produced: {word}");
                    await Task.Delay(200);
                }
                channel.Writer.Complete();
            });

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item.ToUpper()}");
            }
        }

        public static async Task ChannelTransformationExample()
        {
            Console.WriteLine("\n=== Example 9: Channel Transformation ===");
            var inputChannel = Channel.CreateUnbounded<int>();
            var outputChannel = Channel.CreateUnbounded<string>();

            // Producer
            _ = Task.Run(async () =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    await inputChannel.Writer.WriteAsync(i);
                    Console.WriteLine($"Input: {i}");
                }
                inputChannel.Writer.Complete();
            });

            // Transformer
            _ = Task.Run(async () =>
            {
                await foreach (var item in inputChannel.Reader.ReadAllAsync())
                {
                    var transformed = $"Square of {item} is {item * item}";
                    await outputChannel.Writer.WriteAsync(transformed);
                    Console.WriteLine($"Transformed: {transformed}");
                }
                outputChannel.Writer.Complete();
            });

            // Consumer
            await foreach (var item in outputChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Output: {item}");
            }
        }

        public static async Task PriorityChannelExample()
        {
            Console.WriteLine("\n=== Example 10: Priority Channel Simulation ===");
            var highPriorityChannel = Channel.CreateUnbounded<string>();
            var lowPriorityChannel = Channel.CreateUnbounded<string>();

            _ = Task.Run(async () =>
            {
                await highPriorityChannel.Writer.WriteAsync("High Priority Task 1");
                await lowPriorityChannel.Writer.WriteAsync("Low Priority Task 1");
                await highPriorityChannel.Writer.WriteAsync("High Priority Task 2");
                await lowPriorityChannel.Writer.WriteAsync("Low Priority Task 2");

                highPriorityChannel.Writer.Complete();
                lowPriorityChannel.Writer.Complete();
            });

            // Process high priority first
            await foreach (var item in highPriorityChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Processing: {item}");
                await Task.Delay(100);
            }

            // Then process low priority
            await foreach (var item in lowPriorityChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Processing: {item}");
                await Task.Delay(100);
            }
        }

        public static async Task BatchProcessingChannelExample()
        {
            Console.WriteLine("\n=== Example 11: Batch Processing ===");
            var channel = Channel.CreateUnbounded<int>();

            _ = Task.Run(async () =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Queued: {i}");
                    await Task.Delay(50);
                }
                channel.Writer.Complete();
            });

            var batch = new List<int>();
            const int batchSize = 3;

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                batch.Add(item);
                if (batch.Count >= batchSize)
                {
                    Console.WriteLine($"Processing batch: [{string.Join(", ", batch)}]");
                    batch.Clear();
                    await Task.Delay(200);
                }
            }

            if (batch.Count > 0)
            {
                Console.WriteLine($"Processing final batch: [{string.Join(", ", batch)}]");
            }
        }

        public static async Task TimeoutChannelExample()
        {
            Console.WriteLine("\n=== Example 12: Channel with Timeout ===");
            var channel = Channel.CreateUnbounded<int>();

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(1000); // Slow producer
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                }
                channel.Writer.Complete();
            });

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2.5));
            try
            {
                await foreach (var item in channel.Reader.ReadAllAsync(cts.Token))
                {
                    Console.WriteLine($"Consumed: {item}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer timed out");
            }
        }

        public static async Task SingleReaderWriterExample()
        {
            Console.WriteLine("\n=== Example 13: Single Reader/Writer ===");
            var options = new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = true
            };

            var channel = Channel.CreateUnbounded<int>(options);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Single writer produced: {i}");
                }
                channel.Writer.Complete();
            });

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Single reader consumed: {item}");
            }
        }

        public static async Task DropOldestChannelExample()
        {
            Console.WriteLine("\n=== Example 14: Drop Oldest Channel ===");
            var options = new BoundedChannelOptions(3)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            };

            var channel = Channel.CreateBounded<int>(options);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                    await Task.Delay(50);
                }
                channel.Writer.Complete();
            });

            await Task.Delay(500); // Let some items get dropped

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Consumed: {item}");
            }
        }

        public static async Task WaitToWriteExample()
        {
            Console.WriteLine("\n=== Example 15: WaitToWrite ===");
            var channel = Channel.CreateBounded<int>(2);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    await channel.Writer.WaitToWriteAsync();
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Wrote: {i}");
                }
                channel.Writer.Complete();
            });

            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Read: {item}");
                await Task.Delay(300);
            }
        }

        public static async Task TryWriteReadExample()
        {
            Console.WriteLine("\n=== Example 16: TryWrite/TryRead ===");
            var channel = Channel.CreateBounded<int>(3);

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 8; i++)
                {
                    if (channel.Writer.TryWrite(i))
                    {
                        Console.WriteLine($"Successfully wrote: {i}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to write: {i}");
                        await Task.Delay(200);
                        i--; // Retry
                    }
                }
                channel.Writer.Complete();
            });

            while (await channel.Reader.WaitToReadAsync())
            {
                if (channel.Reader.TryRead(out var item))
                {
                    Console.WriteLine($"Successfully read: {item}");
                    await Task.Delay(300);
                }
            }
        }

        public static async Task ChannelClosedExample()
        {
            Console.WriteLine("\n=== Example 17: Channel Completion Detection ===");
            var channel = Channel.CreateUnbounded<int>();

            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Produced: {i}");
                    await Task.Delay(100);
                }
                Console.WriteLine("Writer completing channel...");
                channel.Writer.Complete();
            });

            while (await channel.Reader.WaitToReadAsync())
            {
                while (channel.Reader.TryRead(out var item))
                {
                    Console.WriteLine($"Consumed: {item}");
                }
            }

            Console.WriteLine("Channel is now closed, no more items to read.");
        }

        public static async Task BackpressureExample()
        {
            Console.WriteLine("\n=== Example 18: Backpressure Handling ===");
            var channel = Channel.CreateBounded<int>(2);

            var producer = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"Attempting to produce: {i}");
                    await channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Successfully produced: {i}");
                }
                channel.Writer.Complete();
            });

            var consumer = Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    Console.WriteLine($"Consuming: {item}");
                    await Task.Delay(500); // Slow consumer creates backpressure
                }
            });

            await Task.WhenAll(producer, consumer);
        }

        public static async Task FanOutExample()
        {
            Console.WriteLine("\n=== Example 19: Fan-Out Pattern ===");
            var inputChannel = Channel.CreateUnbounded<int>();
            var outputChannels = new[]
            {
                Channel.CreateUnbounded<int>(),
                Channel.CreateUnbounded<int>(),
                Channel.CreateUnbounded<int>()
            };

            // Producer
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 9; i++)
                {
                    await inputChannel.Writer.WriteAsync(i);
                    Console.WriteLine($"Input: {i}");
                }
                inputChannel.Writer.Complete();
            });

            // Fan-out distributor
            _ = Task.Run(async () =>
            {
                int channelIndex = 0;
                await foreach (var item in inputChannel.Reader.ReadAllAsync())
                {
                    await outputChannels[channelIndex].Writer.WriteAsync(item);
                    Console.WriteLine($"Routed {item} to channel {channelIndex}");
                    channelIndex = (channelIndex + 1) % outputChannels.Length;
                }

                foreach (var channel in outputChannels)
                {
                    channel.Writer.Complete();
                }
            });

            // Multiple consumers
            var consumerTasks = new List<Task>();
            for (int i = 0; i < outputChannels.Length; i++)
            {
                int channelId = i;
                consumerTasks.Add(Task.Run(async () =>
                {
                    await foreach (var item in outputChannels[channelId].Reader.ReadAllAsync())
                    {
                        Console.WriteLine($"Channel {channelId} processed: {item}");
                    }
                }));
            }

            await Task.WhenAll(consumerTasks);
        }

        public static async Task PipelineExample()
        {
            Console.WriteLine("\n=== Example 20: Pipeline Pattern ===");
            var stage1Channel = Channel.CreateUnbounded<int>();
            var stage2Channel = Channel.CreateUnbounded<int>();
            var stage3Channel = Channel.CreateUnbounded<string>();

            // Stage 1: Input generator
            _ = Task.Run(async () =>
            {
                for (int i = 1; i <= 5; i++)
                {
                    await stage1Channel.Writer.WriteAsync(i);
                    Console.WriteLine($"Stage 1 input: {i}");
                    await Task.Delay(100);
                }
                stage1Channel.Writer.Complete();
            });

            // Stage 2: Square numbers
            _ = Task.Run(async () =>
            {
                await foreach (var item in stage1Channel.Reader.ReadAllAsync())
                {
                    var squared = item * item;
                    await stage2Channel.Writer.WriteAsync(squared);
                    Console.WriteLine($"Stage 2 squared: {item} -> {squared}");
                }
                stage2Channel.Writer.Complete();
            });

            // Stage 3: Format as string
            _ = Task.Run(async () =>
            {
                await foreach (var item in stage2Channel.Reader.ReadAllAsync())
                {
                    var formatted = $"Result: {item}";
                    await stage3Channel.Writer.WriteAsync(formatted);
                    Console.WriteLine($"Stage 3 formatted: {formatted}");
                }
                stage3Channel.Writer.Complete();
            });

            // Final consumer
            await foreach (var item in stage3Channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"Pipeline output: {item}");
            }
        }
    }


}
