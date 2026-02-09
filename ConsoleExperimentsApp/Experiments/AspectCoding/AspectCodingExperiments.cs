using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConsoleExperimentsApp.Experiments.AspectCoding
{
    public static class AspectCodingExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Aspect-Oriented Programming Experiments ===");
            Console.WriteLine("Description: Demonstrating various AOP techniques using attributes and interceptors.");
            Console.ResetColor();

            Console.WriteLine("\n1. Logging Aspect with Attributes:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates how to use custom attributes and CallerMemberName");
            Console.WriteLine("   to automatically log method entry and exit points without modifying business logic.");
            Console.ResetColor();
            await DemoLoggingAspect();

            Console.WriteLine("\n2. Performance Measurement Aspect:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to track method execution time using a disposable");
            Console.WriteLine("   PerformanceTracker that leverages the using statement for clean timing measurement.");
            Console.ResetColor();
            await DemoPerformanceAspect();

            Console.WriteLine("\n3. Exception Handling Aspect:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Illustrates centralized exception handling patterns using attributes");
            Console.WriteLine("   to mark methods that require consistent error handling across both sync and async operations.");
            Console.ResetColor();
            await DemoExceptionHandlingAspect();

            Console.WriteLine("\n4. Caching Aspect:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates implementing a simple caching layer to avoid expensive");
            Console.WriteLine("   operations by storing and retrieving results based on method parameters.");
            Console.ResetColor();
            await DemoCachingAspect();

            Console.WriteLine("\n5. Validation Aspect:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to create custom validation attributes for method parameters");
            Console.WriteLine("   to enforce business rules like email format, non-empty values, and numeric ranges.");
            Console.ResetColor();
            await DemoValidationAspect();

            Console.WriteLine("\n6. Interceptor Pattern (C# 12+):");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates Proxy and Decorator patterns for method interception,");
            Console.WriteLine("   allowing you to add cross-cutting concerns like logging and timing around method calls.");
            Console.ResetColor();
            await DemoInterceptorPattern();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        #region Logging Aspect
        private static async Task DemoLoggingAspect()
        {
            var service = new BusinessService();
            await service.ProcessDataAsync("Sample Data");
            service.CalculateValue(42);
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class LogAttribute : Attribute
        {
            public string Message { get; set; } = "";
        }

        public class BusinessService
        {
            [Log(Message = "Processing async data")]
            public async Task ProcessDataAsync(string data)
            {
                LogMethodEntry();
                await Task.Delay(100); // Simulate work
                Console.WriteLine($"   Processing: {data}");
                LogMethodExit();
            }

            [Log(Message = "Calculating numerical value")]
            public int CalculateValue(int input)
            {
                LogMethodEntry();
                var result = input * 2 + 10;
                Console.WriteLine($"   Calculation result: {result}");
                LogMethodExit();
                return result;
            }

            private static void LogMethodEntry([CallerMemberName] string methodName = "")
            {
                Console.WriteLine($"   → Entering {methodName}");
            }

            private static void LogMethodExit([CallerMemberName] string methodName = "")
            {
                Console.WriteLine($"   ← Exiting {methodName}");
            }
        }
        #endregion

        #region Performance Aspect
        private static async Task DemoPerformanceAspect()
        {
            var service = new PerformanceService();
            await service.SlowOperationAsync();
            service.FastOperation();
        }

        public class PerformanceService
        {
            [Performance]
            public async Task SlowOperationAsync()
            {
                using var perf = new PerformanceTracker("SlowOperationAsync");
                await Task.Delay(200); // Simulate slow work
                Console.WriteLine("   Completed slow operation");
            }

            [Performance]
            public void FastOperation()
            {
                using var perf = new PerformanceTracker("FastOperation");
                Thread.Sleep(50); // Simulate fast work
                Console.WriteLine("   Completed fast operation");
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class PerformanceAttribute : Attribute { }

        public class PerformanceTracker : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _operationName;

            public PerformanceTracker(string operationName)
            {
                _operationName = operationName;
                _stopwatch = Stopwatch.StartNew();
                Console.WriteLine($"   ⏱️ Started timing: {_operationName}");
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                Console.WriteLine($"   ⏱️ {_operationName} took: {_stopwatch.ElapsedMilliseconds}ms");
            }
        }
        #endregion

        #region Exception Handling Aspect
        private static async Task DemoExceptionHandlingAspect()
        {
            var service = new RiskyService();

            try
            {
                await service.RiskyOperationAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Caught: {ex.Message}");
            }

            try
            {
                service.AnotherRiskyOperation(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Caught: {ex.Message}");
            }
        }

        public class RiskyService
        {
            [ExceptionHandler]
            public async Task RiskyOperationAsync(bool shouldFail)
            {
                Console.WriteLine("   Executing risky async operation...");
                await Task.Delay(50);

                if (shouldFail)
                    throw new InvalidOperationException("Async operation failed!");

                Console.WriteLine("   Async operation succeeded!");
            }

            [ExceptionHandler]
            public void AnotherRiskyOperation(bool shouldFail)
            {
                Console.WriteLine("   Executing another risky operation...");

                if (shouldFail)
                    throw new ArgumentException("Sync operation failed!");

                Console.WriteLine("   Sync operation succeeded!");
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class ExceptionHandlerAttribute : Attribute { }
        #endregion

        #region Caching Aspect
        private static async Task DemoCachingAspect()
        {
            var service = new DataService();

            // First call - cache miss
            await service.GetExpensiveDataAsync("key1");

            // Second call - cache hit
            await service.GetExpensiveDataAsync("key1");

            // Different key - cache miss
            await service.GetExpensiveDataAsync("key2");
        }

        public class DataService
        {
            private static readonly Dictionary<string, object> _cache = new();

            [Cacheable]
            public async Task<string> GetExpensiveDataAsync(string key)
            {
                // Check cache first
                if (_cache.TryGetValue($"async_{key}", out var cachedValue))
                {
                    Console.WriteLine($"   💾 Cache HIT for key: {key}");
                    return (string)cachedValue;
                }

                Console.WriteLine($"   🔄 Cache MISS for key: {key} - fetching data...");
                await Task.Delay(150); // Simulate expensive operation

                var result = $"Data for {key} at {DateTime.Now:HH:mm:ss}";
                _cache[$"async_{key}"] = result;

                Console.WriteLine($"   ✅ Data cached for key: {key}");
                return result;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class CacheableAttribute : Attribute 
        {
            public int ExpirationMinutes { get; set; } = 5;
        }
        #endregion

        #region Validation Aspect
        private static async Task DemoValidationAspect()
        {
            var service = new UserService();

            try
            {
                await service.CreateUserAsync("John Doe", "john@example.com", 25);
                Console.WriteLine("   ✅ User created successfully");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"   ❌ Validation failed: {ex.Message}");
            }

            try
            {
                await service.CreateUserAsync("", "invalid-email", -5);
                Console.WriteLine("   ✅ User created successfully");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"   ❌ Validation failed: {ex.Message}");
            }
        }

        public class UserService
        {
            [Validate]
            public async Task CreateUserAsync(
                [NotEmpty] string name, 
                [Email] string email, 
                [Range(0, 150)] int age)
            {
                // Simulate validation
                ValidateParameter(name, nameof(name), new NotEmptyAttribute());
                ValidateParameter(email, nameof(email), new EmailAttribute());
                ValidateParameter(age, nameof(age), new RangeAttribute(0, 150));

                Console.WriteLine($"   Creating user: {name}, {email}, Age: {age}");
                await Task.Delay(50); // Simulate user creation
            }

            private static void ValidateParameter(object value, string paramName, ValidationAttribute attribute)
            {
                if (!attribute.IsValid(value))
                {
                    throw new ArgumentException($"Invalid {paramName}: {attribute.ErrorMessage}");
                }
            }
        }

        [AttributeUsage(AttributeTargets.Parameter)]
        public abstract class ValidationAttribute : Attribute
        {
            public string ErrorMessage { get; set; } = "Validation failed";
            public abstract bool IsValid(object value);
        }

        public class NotEmptyAttribute : ValidationAttribute
        {
            public NotEmptyAttribute() => ErrorMessage = "Value cannot be null or empty";
            public override bool IsValid(object value) => !string.IsNullOrWhiteSpace(value?.ToString());
        }

        public class EmailAttribute : ValidationAttribute
        {
            public EmailAttribute() => ErrorMessage = "Invalid email format";
            public override bool IsValid(object value) => value?.ToString()?.Contains("@") == true;
        }

        public class RangeAttribute : ValidationAttribute
        {
            private readonly int _min, _max;
            public RangeAttribute(int min, int max) 
            { 
                _min = min; 
                _max = max; 
                ErrorMessage = $"Value must be between {min} and {max}";
            }
            public override bool IsValid(object value) => 
                value is int intVal && intVal >= _min && intVal <= _max;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class ValidateAttribute : Attribute { }
        #endregion

        #region Interceptor Pattern (C# 12+ Feature)
        private static async Task DemoInterceptorPattern()
        {
            Console.WriteLine("   Demonstrating modern interceptor concepts:");

            // Proxy pattern for method interception
            var calculator = new CalculatorProxy(new Calculator());
            var result = await calculator.AddAsync(5, 3);
            Console.WriteLine($"   Result: {result}");

            // Decorator pattern
            var decoratedCalculator = new LoggingCalculatorDecorator(new Calculator());
            result = await decoratedCalculator.AddAsync(10, 7);
            Console.WriteLine($"   Decorated result: {result}");
        }

        public interface ICalculator
        {
            Task<int> AddAsync(int a, int b);
        }

        public class Calculator : ICalculator
        {
            public async Task<int> AddAsync(int a, int b)
            {
                await Task.Delay(25); // Simulate async work
                return a + b;
            }
        }

        // Proxy pattern implementation
        public class CalculatorProxy : ICalculator
        {
            private readonly ICalculator _target;

            public CalculatorProxy(ICalculator target)
            {
                _target = target;
            }

            public async Task<int> AddAsync(int a, int b)
            {
                Console.WriteLine($"   🔧 Proxy: Before AddAsync({a}, {b})");
                var sw = Stopwatch.StartNew();

                var result = await _target.AddAsync(a, b);

                sw.Stop();
                Console.WriteLine($"   🔧 Proxy: After AddAsync - took {sw.ElapsedMilliseconds}ms");
                return result;
            }
        }

        // Decorator pattern implementation
        public class LoggingCalculatorDecorator : ICalculator
        {
            private readonly ICalculator _calculator;

            public LoggingCalculatorDecorator(ICalculator calculator)
            {
                _calculator = calculator;
            }

            public async Task<int> AddAsync(int a, int b)
            {
                Console.WriteLine($"   🎨 Decorator: Logging call to AddAsync({a}, {b})");
                try
                {
                    var result = await _calculator.AddAsync(a, b);
                    Console.WriteLine($"   🎨 Decorator: AddAsync completed successfully with result: {result}");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   🎨 Decorator: AddAsync failed with error: {ex.Message}");
                    throw;
                }
            }
        }
        #endregion
    }
}
