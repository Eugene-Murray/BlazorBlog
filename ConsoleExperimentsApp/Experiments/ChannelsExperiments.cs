using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace ConsoleExperimentsApp.Experiments
{
    public class ChannelsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ChannelsExperiments");
            Console.ResetColor();

            await ChannelExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        public static async Task ChannelExample()
        {
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
    }


}
