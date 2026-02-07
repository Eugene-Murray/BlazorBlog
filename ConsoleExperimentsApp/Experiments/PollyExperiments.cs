using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleExperimentsApp.Experiments
{
    public static class PollyExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Polly Experiments ===");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }
    }
}
