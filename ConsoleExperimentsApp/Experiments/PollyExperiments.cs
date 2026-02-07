using System;
using System.Collections.Generic;
using System.Text;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace ConsoleExperimentsApp.Experiments
{
    public static class PollyExperiments
    {
        private static int _attemptCount = 0;

        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Polly Experiments ===");
            Console.ResetColor();

            await RetryPolicyExample();
            await WaitAndRetryExample();
            await CircuitBreakerExample();
            await TimeoutExample();
            await FallbackExample();
            await CombinedPoliciesExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static async Task RetryPolicyExample()
        {
            Console.WriteLine("\n--- 1. Basic Retry Policy ---");
            _attemptCount = 0;

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .RetryAsync(3, onRetry: (exception, retryCount) =>
                {
                    Console.WriteLine($"  Retry {retryCount} due to: {exception.Message}");
                });

            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    _attemptCount++;
                    Console.WriteLine($"  Attempt {_attemptCount}");

                    if (_attemptCount < 3)
                    {
                        throw new HttpRequestException("Simulated network error");
                    }

                    Console.WriteLine("  ✓ Success!");
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Failed: {ex.Message}");
            }
        }

        private static async Task WaitAndRetryExample()
        {
            Console.WriteLine("\n--- 2. Wait and Retry with Exponential Backoff ---");
            _attemptCount = 0;

            var waitAndRetryPolicy = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetryAsync(
                    retryCount: 4,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"  Retry {retryCount} after {timeSpan.TotalMilliseconds}ms delay");
                    });

            try
            {
                await waitAndRetryPolicy.ExecuteAsync(async () =>
                {
                    _attemptCount++;
                    Console.WriteLine($"  Attempt {_attemptCount} at {DateTime.Now:HH:mm:ss.fff}");

                    if (_attemptCount < 3)
                    {
                        throw new InvalidOperationException("Service temporarily unavailable");
                    }

                    Console.WriteLine("  ✓ Operation succeeded!");
                    await Task.CompletedTask;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ All retries exhausted: {ex.Message}");
            }
        }

        private static async Task CircuitBreakerExample()
        {
            Console.WriteLine("\n--- 3. Circuit Breaker Pattern ---");

            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(3),
                    onBreak: (exception, duration) =>
                    {
                        Console.WriteLine($"  ⚠ Circuit OPEN for {duration.TotalSeconds}s due to: {exception.Message}");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("  ✓ Circuit CLOSED - Back to normal");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("  ⚡ Circuit HALF-OPEN - Testing...");
                    });

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    await circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        Console.WriteLine($"  Request {i}");

                        if (i <= 3)
                        {
                            throw new Exception("Service failure");
                        }

                        Console.WriteLine($"  ✓ Request {i} succeeded");
                        await Task.CompletedTask;
                    });
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"  ✗ Request {i} rejected - Circuit is OPEN");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Request {i} failed: {ex.Message}");
                }

                await Task.Delay(500);
            }
        }

        private static async Task TimeoutExample()
        {
            Console.WriteLine("\n--- 4. Timeout Policy ---");

            var timeoutPolicy = Policy
                .TimeoutAsync(
                    seconds: 2,
                    timeoutStrategy: TimeoutStrategy.Pessimistic,
                    onTimeoutAsync: (context, timespan, task) =>
                    {
                        Console.WriteLine($"  ⏱ Timeout after {timespan.TotalSeconds}s");
                        return Task.CompletedTask;
                    });

            try
            {
                await timeoutPolicy.ExecuteAsync(async () =>
                {
                    Console.WriteLine("  Starting long-running operation...");
                    await Task.Delay(1000);
                    Console.WriteLine("  ✓ Completed within timeout");
                });
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine("  ✗ Operation timed out");
            }

            try
            {
                await timeoutPolicy.ExecuteAsync(async () =>
                {
                    Console.WriteLine("  Starting operation that will timeout...");
                    await Task.Delay(3000);
                    Console.WriteLine("  This won't be reached");
                });
            }
            catch (TimeoutRejectedException)
            {
                Console.WriteLine("  ✗ Operation timed out (as expected)");
            }
        }

        private static async Task FallbackExample()
        {
            Console.WriteLine("\n--- 5. Fallback Policy ---");

            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: "Fallback data from cache",
                    onFallbackAsync: (result, context) =>
                    {
                        Console.WriteLine($"  ⚠ Fallback triggered due to: {result.Exception.Message}");
                        return Task.CompletedTask;
                    });

            var result1 = await fallbackPolicy.ExecuteAsync(async () =>
            {
                Console.WriteLine("  Attempting primary operation...");
                throw new Exception("Primary service unavailable");
            });
            Console.WriteLine($"  Result: {result1}");

            var result2 = await fallbackPolicy.ExecuteAsync(async () =>
            {
                Console.WriteLine("  Attempting primary operation...");
                await Task.CompletedTask;
                return "Data from primary source";
            });
            Console.WriteLine($"  ✓ Result: {result2}");
        }

        private static async Task CombinedPoliciesExample()
        {
            Console.WriteLine("\n--- 6. Combined Policies (Wrap) ---");
            _attemptCount = 0;

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(2, 
                    retryAttempt => TimeSpan.FromMilliseconds(200),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"  Retry {retryCount}");
                    });

            var timeoutPolicy = Policy
                .TimeoutAsync(
                    seconds: 1,
                    timeoutStrategy: TimeoutStrategy.Pessimistic);

            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: "Default response",
                    onFallbackAsync: (result, context) =>
                    {
                        Console.WriteLine($"  Using fallback value");
                        return Task.CompletedTask;
                    });

            var policyWrap = Policy.WrapAsync(
                fallbackPolicy,
                retryPolicy.AsAsyncPolicy<string>(),
                timeoutPolicy.AsAsyncPolicy<string>());

            var result = await policyWrap.ExecuteAsync(async () =>
            {
                _attemptCount++;
                Console.WriteLine($"  Attempt {_attemptCount}");

                if (_attemptCount < 2)
                {
                    throw new HttpRequestException("Temporary failure");
                }

                await Task.Delay(100);
                return "Success response";
            });

            Console.WriteLine($"  ✓ Final result: {result}");
        }
    }
}
