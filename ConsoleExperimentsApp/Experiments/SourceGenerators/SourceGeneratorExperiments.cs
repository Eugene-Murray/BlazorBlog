using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace ConsoleExperimentsApp.Experiments.SourceGenerators
{
    public static class SourceGeneratorExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Source Generator Experiments ===");
            Console.WriteLine("Description: Demonstrates various Source Generator patterns and use cases");
            Console.ResetColor();

            // Example 1: Auto-property notification (INotifyPropertyChanged pattern)
            DemoAutoNotifyPropertyChanged();

            // Example 2: ToString Generator
            DemoToStringGenerator();

            // Example 3: Builder Pattern Generator
            DemoBuilderPattern();

            // Example 4: Enum Extensions Generator
            DemoEnumExtensions();

            // Example 5: Validation Generator
            DemoValidation();

            // Example 6: Mapper Generator
            DemoMapper();

            // Example 7: Logging Generator
            DemoLogging();

            // Example 8: Singleton Generator
            DemoSingleton();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        #region Example 1: Auto-property notification (INotifyPropertyChanged)

        // Marker attribute for source generator
        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public class NotifyAttribute : Attribute { }

        // Example class using auto-notification
        public partial class PersonViewModel : INotifyPropertyChanged
        {
            [Notify]
            private string _firstName = string.Empty;

            [Notify]
            private string _lastName = string.Empty;

            [Notify]
            private int _age;

            public event PropertyChangedEventHandler? PropertyChanged;

            // Source generator would generate properties like:
            // public string FirstName
            // {
            //     get => _firstName;
            //     set
            //     {
            //         if (_firstName != value)
            //         {
            //             _firstName = value;
            //             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
            //         }
            //     }
            // }
        }

        private static void DemoAutoNotifyPropertyChanged()
        {
            Console.WriteLine("\n[1] Auto-Notify Property Changed Generator:");
            Console.WriteLine("    - Automatically implements INotifyPropertyChanged");
            Console.WriteLine("    - Generates properties from [Notify] fields");
            Console.WriteLine("    - Reduces boilerplate code in MVVM patterns");

            var person = new PersonViewModel();
            person.PropertyChanged += (s, e) => Console.WriteLine($"    Property changed: {e.PropertyName}");

            // These would use generated properties
            Console.WriteLine("    Setting properties would trigger PropertyChanged events...");
        }

        #endregion

        #region Example 2: ToString Generator

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class GenerateToStringAttribute : Attribute { }

        [GenerateToString]
        public partial class Product
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateTime CreatedDate { get; set; }

            // Source generator would create:
            // public override string ToString()
            // {
            //     return $"Product {{ Id = {Id}, Name = {Name}, Price = {Price}, CreatedDate = {CreatedDate} }}";
            // }
        }

        private static void DemoToStringGenerator()
        {
            Console.WriteLine("\n[2] ToString Generator:");
            Console.WriteLine("    - Automatically generates ToString() methods");
            Console.WriteLine("    - Includes all public properties");
            Console.WriteLine("    - Customizable formatting options");

            var product = new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 999.99m,
                CreatedDate = DateTime.Now
            };
            Console.WriteLine($"    Generated: {product}");
        }

        #endregion

        #region Example 3: Builder Pattern Generator

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class GenerateBuilderAttribute : Attribute { }

        [GenerateBuilder]
        public partial class Order
        {
            public int OrderId { get; init; }
            public string CustomerName { get; init; } = string.Empty;
            public List<string> Items { get; init; } = new();
            public decimal TotalAmount { get; init; }
            public DateTime OrderDate { get; init; }

            // Source generator would create:
            // public class Builder
            // {
            //     private int _orderId;
            //     private string _customerName = string.Empty;
            //     private List<string> _items = new();
            //     private decimal _totalAmount;
            //     private DateTime _orderDate;
            //
            //     public Builder WithOrderId(int orderId) { _orderId = orderId; return this; }
            //     public Builder WithCustomerName(string customerName) { _customerName = customerName; return this; }
            //     public Builder WithItems(List<string> items) { _items = items; return this; }
            //     public Builder WithTotalAmount(decimal totalAmount) { _totalAmount = totalAmount; return this; }
            //     public Builder WithOrderDate(DateTime orderDate) { _orderDate = orderDate; return this; }
            //     public Order Build() => new() { OrderId = _orderId, CustomerName = _customerName, ... };
            // }
        }

        private static void DemoBuilderPattern()
        {
            Console.WriteLine("\n[3] Builder Pattern Generator:");
            Console.WriteLine("    - Generates fluent builder pattern");
            Console.WriteLine("    - Useful for complex object construction");
            Console.WriteLine("    - Supports immutable objects with init properties");

            // var order = new Order.Builder()
            //     .WithOrderId(100)
            //     .WithCustomerName("John Doe")
            //     .WithItems(new List<string> { "Item1", "Item2" })
            //     .WithTotalAmount(150.00m)
            //     .WithOrderDate(DateTime.Now)
            //     .Build();

            Console.WriteLine("    Would create: Order with fluent API");
        }

        #endregion

        #region Example 4: Enum Extensions Generator

        [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
        public class GenerateEnumExtensionsAttribute : Attribute { }

        [GenerateEnumExtensions]
        public enum OrderStatus
        {
            [Description("Order is pending")]
            Pending = 0,

            [Description("Order is being processed")]
            Processing = 1,

            [Description("Order has been shipped")]
            Shipped = 2,

            [Description("Order has been delivered")]
            Delivered = 3,

            [Description("Order has been cancelled")]
            Cancelled = 4
        }

        // Source generator would create extension methods:
        // public static class OrderStatusExtensions
        // {
        //     public static string ToDescription(this OrderStatus status) => status switch
        //     {
        //         OrderStatus.Pending => "Order is pending",
        //         OrderStatus.Processing => "Order is being processed",
        //         ...
        //     };
        //
        //     public static bool IsDefined(this OrderStatus status) => ...;
        //     public static OrderStatus[] GetValues() => ...;
        //     public static string[] GetNames() => ...;
        // }

        private static void DemoEnumExtensions()
        {
            Console.WriteLine("\n[4] Enum Extensions Generator:");
            Console.WriteLine("    - Generates extension methods for enums");
            Console.WriteLine("    - Provides ToDescription(), IsDefined(), etc.");
            Console.WriteLine("    - Improves enum usability");

            var status = OrderStatus.Processing;
            Console.WriteLine($"    Status: {status}");
            Console.WriteLine("    Generated methods: ToDescription(), GetValues(), GetNames(), IsDefined()");
        }

        #endregion

        #region Example 5: Validation Generator

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class GenerateValidationAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public class RequiredAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public class RangeAttribute : Attribute
        {
            public RangeAttribute(int min, int max) { }
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public class EmailAttribute : Attribute { }

        [GenerateValidation]
        public partial class UserRegistration
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [Email]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Range(8, 100)]
            public string Password { get; set; } = string.Empty;

            [Range(18, 120)]
            public int Age { get; set; }

            // Source generator would create:
            // public ValidationResult Validate()
            // {
            //     var errors = new List<string>();
            //     if (string.IsNullOrWhiteSpace(Username)) errors.Add("Username is required");
            //     if (string.IsNullOrWhiteSpace(Email)) errors.Add("Email is required");
            //     if (!IsValidEmail(Email)) errors.Add("Email format is invalid");
            //     if (Password.Length < 8 || Password.Length > 100) errors.Add("Password must be 8-100 characters");
            //     if (Age < 18 || Age > 120) errors.Add("Age must be between 18 and 120");
            //     return new ValidationResult(errors.Count == 0, errors);
            // }
        }

        private static void DemoValidation()
        {
            Console.WriteLine("\n[5] Validation Generator:");
            Console.WriteLine("    - Generates validation methods from attributes");
            Console.WriteLine("    - Supports Required, Range, Email, etc.");
            Console.WriteLine("    - Returns ValidationResult with all errors");

            var registration = new UserRegistration
            {
                Username = "john_doe",
                Email = "john@example.com",
                Password = "SecurePass123",
                Age = 25
            };
            Console.WriteLine("    Generated Validate() method would check all constraints");
        }

        #endregion

        #region Example 6: Mapper Generator

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        public class MapToAttribute : Attribute
        {
            public MapToAttribute(Type targetType) { }
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public class MapPropertyAttribute : Attribute
        {
            public string TargetPropertyName { get; }
            public MapPropertyAttribute(string targetPropertyName)
            {
                TargetPropertyName = targetPropertyName;
            }
        }

        public class CustomerDto
        {
            public int CustomerId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;
        }

        [MapTo(typeof(CustomerDto))]
        public partial class Customer
        {
            public int Id { get; set; }

            [MapProperty("FullName")]
            public string Name { get; set; } = string.Empty;

            [MapProperty("EmailAddress")]
            public string Email { get; set; } = string.Empty;

            // Source generator would create:
            // public CustomerDto ToCustomerDto()
            // {
            //     return new CustomerDto
            //     {
            //         CustomerId = this.Id,
            //         FullName = this.Name,
            //         EmailAddress = this.Email
            //     };
            // }
        }

        private static void DemoMapper()
        {
            Console.WriteLine("\n[6] Mapper Generator:");
            Console.WriteLine("    - Generates mapping methods between types");
            Console.WriteLine("    - Supports property name mapping");
            Console.WriteLine("    - Compile-time type safety");

            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com"
            };
            Console.WriteLine("    Generated ToCustomerDto() method would map properties");
        }

        #endregion

        #region Example 7: Logging Generator

        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        public class LogExecutionTimeAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
        public class LogMethodCallAttribute : Attribute { }

        public partial class DataService
        {
            [LogExecutionTime]
            [LogMethodCall]
            public async Task<List<string>> GetDataAsync(int count)
            {
                await Task.Delay(100);
                return Enumerable.Range(1, count).Select(i => $"Item{i}").ToList();
            }

            // Source generator would wrap the method with:
            // public async Task<List<string>> GetDataAsync(int count)
            // {
            //     var sw = Stopwatch.StartNew();
            //     Console.WriteLine($"[LOG] Calling GetDataAsync with parameters: count={count}");
            //     try
            //     {
            //         var result = await GetDataAsyncCore(count);
            //         Console.WriteLine($"[LOG] GetDataAsync completed in {sw.ElapsedMilliseconds}ms");
            //         return result;
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"[LOG] GetDataAsync failed: {ex.Message}");
            //         throw;
            //     }
            // }
            // private async Task<List<string>> GetDataAsyncCore(int count) { ... original method ... }
        }

        private static void DemoLogging()
        {
            Console.WriteLine("\n[7] Logging Generator:");
            Console.WriteLine("    - Automatically adds logging to methods");
            Console.WriteLine("    - Tracks execution time and parameters");
            Console.WriteLine("    - Implements aspect-oriented programming");

            var service = new DataService();
            Console.WriteLine("    Generated logging wrapper would track method execution");
        }

        #endregion

        #region Example 8: Singleton Generator

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public class SingletonAttribute : Attribute { }

        [Singleton]
        public partial class ConfigurationManager
        {
            private readonly Dictionary<string, string> _settings = new();

            // Source generator would create:
            // private static readonly Lazy<ConfigurationManager> _instance = 
            //     new Lazy<ConfigurationManager>(() => new ConfigurationManager());
            // 
            // public static ConfigurationManager Instance => _instance.Value;
            //
            // private ConfigurationManager() { }

            public void SetSetting(string key, string value)
            {
                _settings[key] = value;
            }

            public string? GetSetting(string key)
            {
                return _settings.TryGetValue(key, out var value) ? value : null;
            }
        }

        private static void DemoSingleton()
        {
            Console.WriteLine("\n[8] Singleton Generator:");
            Console.WriteLine("    - Generates thread-safe singleton pattern");
            Console.WriteLine("    - Creates Instance property");
            Console.WriteLine("    - Makes constructor private");

            // var config = ConfigurationManager.Instance;
            // config.SetSetting("ApiUrl", "https://api.example.com");
            Console.WriteLine("    Generated Instance property provides singleton access");
        }

        #endregion

        #region Additional Source Generator Concepts

        // Other useful source generator patterns:

        // 9. Dependency Injection Registration
        // [RegisterService(ServiceLifetime.Scoped)]
        // - Automatically registers services in DI container

        // 10. JSON Serialization Optimizations
        // [JsonSerializable(typeof(MyClass))]
        // - Generates optimized JSON serializers (System.Text.Json)

        // 11. Strong-Typed Configuration
        // [GenerateOptions("AppSettings")]
        // - Creates strongly-typed options classes from configuration

        // 12. Repository Pattern Generator
        // [GenerateRepository]
        // - Creates CRUD operations for entities

        // 13. Mediator Pattern (CQRS)
        // [GenerateHandler]
        // - Creates command/query handlers automatically

        // 14. API Client Generator
        // [GenerateClient("https://api.example.com/swagger.json")]
        // - Generates API client from OpenAPI/Swagger spec

        // 15. Mock Generator
        // [GenerateMock]
        // - Creates test mocks for interfaces

        #endregion
    }
}
