using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;

namespace ConsoleExperimentsApp.Experiments
{
    public class SerilogExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("SerilogExperiments");
            Console.ResetColor();

            SerilogFileLogExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        private static void SerilogFileLogExample()
        {
            Console.WriteLine("Setting up Serilog file logging...");

            // Configure Serilog to write to both console and file
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/serilog-example-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 7) // Keep logs for 7 days
                .CreateLogger();

            try
            {
                // Demonstrate different log levels
                Log.Debug("This is a debug message");
                Log.Information("Application started successfully");
                Log.Warning("This is a warning message");

                // Structured logging example
                var user = "John Doe";
                var itemCount = 42;
                Log.Information("User {UserName} processed {ItemCount} items", user, itemCount);

                // Log with properties
                Log.ForContext("SourceContext", "SerilogExample")
                   .Information("Processing completed at {ProcessTime}", DateTime.Now);

                // Simulate an error scenario
                try
                {
                    throw new InvalidOperationException("This is a test exception");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during processing");
                }

                Log.Information("Serilog file logging example completed. Check the 'logs' folder for output files.");
            }
            finally
            {
                // Ensure all log events are flushed before closing
                Log.CloseAndFlush();
            }
        }
    }
}
