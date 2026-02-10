using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ConsoleExperimentsApp.Experiments.Azure
{
    public static class AzureAppInsightsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Azure Application Insights Experiments ===");
            Console.WriteLine("Description: Comprehensive examples of Application Insights telemetry tracking");
            Console.ResetColor();

            // NOTE: Set your Application Insights connection string
            // You can get this from Azure Portal -> Application Insights -> Properties
            string connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING") 
                                    ?? "InstrumentationKey=00000000-0000-0000-0000-000000000000";

            var config = TelemetryConfiguration.CreateDefault();
            config.ConnectionString = connectionString;

            var telemetryClient = new TelemetryClient(config);

            try
            {
                Console.WriteLine("\n1. Basic Trace Logging");
                await TrackTracesExample(telemetryClient);

                Console.WriteLine("\n2. Custom Events");
                await TrackCustomEventsExample(telemetryClient);

                Console.WriteLine("\n3. Metrics Tracking");
                await TrackMetricsExample(telemetryClient);

                Console.WriteLine("\n4. Exception Tracking");
                await TrackExceptionsExample(telemetryClient);

                Console.WriteLine("\n5. Dependency Tracking");
                await TrackDependenciesExample(telemetryClient);

                Console.WriteLine("\n6. Request Tracking");
                await TrackRequestsExample(telemetryClient);

                Console.WriteLine("\n7. Custom Properties and Measurements");
                await TrackWithCustomPropertiesExample(telemetryClient);

                Console.WriteLine("\n8. Performance Metrics");
                await TrackPerformanceMetricsExample(telemetryClient);

                Console.WriteLine("\n9. User and Session Tracking");
                await TrackUserAndSessionExample(telemetryClient);

                Console.WriteLine("\n10. Availability Testing");
                await TrackAvailabilityExample(telemetryClient);

                // Flush telemetry before exiting
                telemetryClient.Flush();
                await Task.Delay(5000); // Allow time for telemetry to be sent
            }
            finally
            {
                config.Dispose();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static async Task TrackTracesExample(TelemetryClient telemetryClient)
        {
            // Different severity levels
            telemetryClient.TrackTrace("Verbose trace message", SeverityLevel.Verbose);
            telemetryClient.TrackTrace("Information trace message", SeverityLevel.Information);
            telemetryClient.TrackTrace("Warning trace message", SeverityLevel.Warning);
            telemetryClient.TrackTrace("Error trace message", SeverityLevel.Error);
            telemetryClient.TrackTrace("Critical trace message", SeverityLevel.Critical);

            // Trace with custom properties
            var properties = new Dictionary<string, string>
            {
                { "Environment", "Development" },
                { "Component", "ExperimentsApp" },
                { "UserId", "user123" }
            };
            telemetryClient.TrackTrace("Trace with properties", SeverityLevel.Information, properties);

            Console.WriteLine("✓ Tracked traces with different severity levels");
            await Task.CompletedTask;
        }

        private static async Task TrackCustomEventsExample(TelemetryClient telemetryClient)
        {
            // Simple event
            telemetryClient.TrackEvent("ButtonClicked");

            // Event with properties
            var eventProperties = new Dictionary<string, string>
            {
                { "ButtonName", "SubmitButton" },
                { "PageName", "HomePage" },
                { "UserType", "Premium" }
            };
            telemetryClient.TrackEvent("UserAction", eventProperties);

            // Event with properties and measurements
            var eventWithMeasurements = new EventTelemetry("PageLoaded");
            eventWithMeasurements.Properties.Add("ButtonName", "SubmitButton");
            eventWithMeasurements.Properties.Add("PageName", "HomePage");
            eventWithMeasurements.Properties.Add("UserType", "Premium");
            eventWithMeasurements.Properties.Add("LoadTime", "1.23");
            eventWithMeasurements.Properties.Add("ItemCount", "42");
            telemetryClient.TrackEvent(eventWithMeasurements);

            Console.WriteLine("✓ Tracked custom events with properties and measurements");
            await Task.CompletedTask;
        }

        private static async Task TrackMetricsExample(TelemetryClient telemetryClient)
        {
            // Simple metric
            telemetryClient.TrackMetric("QueueLength", 25);

            // Metric with properties
            var metricProperties = new Dictionary<string, string>
            {
                { "Server", "Server01" },
                { "Region", "EastUS" }
            };
            telemetryClient.TrackMetric("ResponseTime", 156.7, metricProperties);

            // Track aggregated metric
            var metric = telemetryClient.GetMetric("ProcessedItems");
            for (int i = 0; i < 10; i++)
            {
                metric.TrackValue(i * 10);
                await Task.Delay(100);
            }

            // Multi-dimensional metric
            var multiDimMetric = telemetryClient.GetMetric("Sales", "Product", "Region", "PaymentMethod");
            multiDimMetric.TrackValue(99.99, "Product1", "US", "CreditCard");
            multiDimMetric.TrackValue(149.99, "Product2", "UK", "PayPal");

            Console.WriteLine("✓ Tracked various metrics");
            await Task.CompletedTask;
        }

        private static async Task TrackExceptionsExample(TelemetryClient telemetryClient)
        {
            // Track handled exception
            try
            {
                throw new InvalidOperationException("This is a test exception");
            }
            catch (Exception ex)
            {
                var exceptionProperties = new Dictionary<string, string>
                {
                    { "Operation", "TestOperation" },
                    { "Handled", "true" }
                };
                telemetryClient.TrackException(ex, exceptionProperties);
            }

            // Track exception with custom measurements
            try
            {
                var result = 10 / int.Parse("0");
            }
            catch (Exception ex)
            {
                var exceptionTelemetry = new ExceptionTelemetry(ex);
                exceptionTelemetry.Properties.Add("Component", "Calculator");
                exceptionTelemetry.Properties.Add("AttemptNumber", "3");
                telemetryClient.TrackException(exceptionTelemetry);
            }

            Console.WriteLine("✓ Tracked exceptions with context");
            await Task.CompletedTask;
        }

        private static async Task TrackDependenciesExample(TelemetryClient telemetryClient)
        {
            // Track HTTP dependency
            var stopwatch = Stopwatch.StartNew();
            bool success = true;

            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
                success = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                success = false;
                telemetryClient.TrackException(ex);
            }
            finally
            {
                stopwatch.Stop();
                telemetryClient.TrackDependency(
                    "HTTP",
                    "jsonplaceholder.typicode.com",
                    "GET /posts/1",
                    DateTime.UtcNow.Subtract(stopwatch.Elapsed),
                    stopwatch.Elapsed,
                    success
                );
            }

            // Track SQL dependency
            var sqlStopwatch = Stopwatch.StartNew();
            await Task.Delay(50); // Simulate SQL query
            sqlStopwatch.Stop();

            telemetryClient.TrackDependency(
                "SQL",
                "myserver.database.windows.net",
                "SELECT * FROM Users",
                DateTime.UtcNow.Subtract(sqlStopwatch.Elapsed),
                sqlStopwatch.Elapsed,
                true
            );

            // Track Azure Storage dependency
            var dependency = new DependencyTelemetry
            {
                Type = "Azure blob",
                Target = "mystorageaccount.blob.core.windows.net",
                Data = "UploadBlob",
                Timestamp = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromMilliseconds(234),
                Success = true
            };
            dependency.Properties.Add("BlobName", "document.pdf");
            dependency.Properties.Add("BlobSize", "1024768");
            telemetryClient.TrackDependency(dependency);

            Console.WriteLine("✓ Tracked HTTP, SQL, and Azure Storage dependencies");
        }

        private static async Task TrackRequestsExample(TelemetryClient telemetryClient)
        {
            // Track a simulated request
            var stopwatch = Stopwatch.StartNew();

            // Simulate processing
            await Task.Delay(Random.Shared.Next(100, 500));

            stopwatch.Stop();

            var requestTelemetry = new RequestTelemetry
            {
                Name = "GET /api/products",
                Timestamp = DateTimeOffset.UtcNow.Subtract(stopwatch.Elapsed),
                Duration = stopwatch.Elapsed,
                ResponseCode = "200",
                Success = true,
                Url = new Uri("https://myapi.azurewebsites.net/api/products")
            };

            requestTelemetry.Properties.Add("RequestId", Guid.NewGuid().ToString());
            requestTelemetry.Properties.Add("UserAgent", "Mozilla/5.0");
            requestTelemetry.Properties.Add("ItemsReturned", "25");

            telemetryClient.TrackRequest(requestTelemetry);

            // Track failed request
            var failedRequest = new RequestTelemetry
            {
                Name = "POST /api/orders",
                Timestamp = DateTimeOffset.UtcNow,
                Duration = TimeSpan.FromMilliseconds(123),
                ResponseCode = "500",
                Success = false
            };
            failedRequest.Properties.Add("ErrorReason", "Database timeout");
            telemetryClient.TrackRequest(failedRequest);

            Console.WriteLine("✓ Tracked successful and failed requests");
        }

        private static async Task TrackWithCustomPropertiesExample(TelemetryClient telemetryClient)
        {
            // Set global properties that will be attached to all telemetry
            telemetryClient.Context.GlobalProperties["ApplicationVersion"] = "2.1.0";
            telemetryClient.Context.GlobalProperties["DeploymentEnvironment"] = "Production";

            // Set user context
            telemetryClient.Context.User.Id = "user123";
            telemetryClient.Context.User.AuthenticatedUserId = "user@example.com";

            // Set location context
            telemetryClient.Context.Location.Ip = "1.2.3.4";

            // Set operation context for distributed tracing
            var operationId = Guid.NewGuid().ToString();
            telemetryClient.Context.Operation.Name = "ProcessOrder";
            telemetryClient.Context.GlobalProperties["OperationId"] = operationId;
            telemetryClient.Context.GlobalProperties["ParentOperationId"] = Guid.NewGuid().ToString();

            telemetryClient.TrackEvent("EventWithContext");

            Console.WriteLine("✓ Tracked telemetry with custom global properties and context");
            await Task.CompletedTask;
        }

        private static async Task TrackPerformanceMetricsExample(TelemetryClient telemetryClient)
        {
            // Track operation performance
            using (var operation = telemetryClient.StartOperation<RequestTelemetry>("ComplexOperation"))
            {
                operation.Telemetry.Properties.Add("OperationType", "BatchProcessing");

                // Simulate work
                await Task.Delay(100);

                // Track nested operation
                using (var childOperation = telemetryClient.StartOperation<DependencyTelemetry>("DatabaseQuery"))
                {
                    childOperation.Telemetry.Type = "SQL";
                    childOperation.Telemetry.Data = "SELECT * FROM Orders";
                    await Task.Delay(50);
                    childOperation.Telemetry.Success = true;
                }

                // Track another nested operation
                using (var childOperation = telemetryClient.StartOperation<DependencyTelemetry>("CacheQuery"))
                {
                    childOperation.Telemetry.Type = "Redis";
                    childOperation.Telemetry.Data = "GET user:123";
                    await Task.Delay(10);
                    childOperation.Telemetry.Success = true;
                }

                operation.Telemetry.Success = true;
            }

            // Track process memory metrics
            var process = Process.GetCurrentProcess();
            telemetryClient.TrackMetric("MemoryUsedMB", process.WorkingSet64 / 1024.0 / 1024.0);
            telemetryClient.TrackMetric("ThreadCount", process.Threads.Count);
            telemetryClient.TrackMetric("CpuTime", process.TotalProcessorTime.TotalMilliseconds);

            Console.WriteLine("✓ Tracked performance metrics and nested operations");
        }

        private static async Task TrackUserAndSessionExample(TelemetryClient telemetryClient)
        {
            // Create a new telemetry client with specific user context
            var userClient = new TelemetryClient(telemetryClient.TelemetryConfiguration);

            // Set user information
            userClient.Context.User.Id = "user-" + Guid.NewGuid().ToString("N").Substring(0, 8);
            userClient.Context.User.AuthenticatedUserId = "john.doe@example.com";

            // Track page view as custom event (useful for web applications)
            var pageView = new EventTelemetry("PageView");
            pageView.Properties.Add("PageName", "HomePage");
            pageView.Properties.Add("Url", "https://myapp.com/home");
            pageView.Properties.Add("Referrer", "google.com");
            pageView.Properties.Add("Duration", "2.5");
            pageView.Properties.Add("ScrollDepth", "75.5");
            userClient.TrackEvent(pageView);

            // Track user actions
            userClient.TrackEvent("UserLogin", new Dictionary<string, string>
            {
                { "Method", "OAuth" },
                { "Provider", "Microsoft" }
            });

            userClient.TrackEvent("FeatureUsed", new Dictionary<string, string>
            {
                { "FeatureName", "AdvancedSearch" },
                { "PlanType", "Premium" }
            });

            Console.WriteLine("✓ Tracked user and session information");
            await Task.CompletedTask;
        }

        private static async Task TrackAvailabilityExample(TelemetryClient telemetryClient)
        {
            var availabilityTestName = "MyApiHealthCheck";
            var stopwatch = Stopwatch.StartNew();
            bool success = false;
            string message = string.Empty;

            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts/1");
                success = response.IsSuccessStatusCode;
                message = success ? "Endpoint is available" : $"Failed with status {response.StatusCode}";
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Exception: {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();

                var availability = new AvailabilityTelemetry
                {
                    Name = availabilityTestName,
                    Duration = stopwatch.Elapsed,
                    Success = success,
                    Message = message,
                    Timestamp = DateTimeOffset.UtcNow,
                    RunLocation = "Local-Development"
                };

                availability.Properties.Add("TestType", "HealthCheck");
                availability.Properties.Add("Endpoint", "https://jsonplaceholder.typicode.com/posts/1");

                telemetryClient.TrackAvailability(availability);
            }

            Console.WriteLine($"✓ Tracked availability test: {(success ? "Success" : "Failed")} - {message}");
        }
    }
}
