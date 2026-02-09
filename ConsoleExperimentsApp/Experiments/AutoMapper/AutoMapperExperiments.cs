using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using System.Linq;

namespace ConsoleExperimentsApp.Experiments.AutoMapper
{
    public static class AutoMapperExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== AutoMapper Experiments ===");
            Console.WriteLine("Description: Demonstrating object-to-object mapping with AutoMapper including flattening, projections, and custom resolvers.");
            Console.ResetColor();

            // Example 1: Basic Mapping
            Console.WriteLine("\n1. Basic Object-to-Object Mapping:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates simple property-to-property mapping between objects");
            Console.WriteLine("   with matching property names using AutoMapper's convention-based mapping.");
            Console.ResetColor();
            BasicMapping();

            // Example 2: Flattening
            Console.WriteLine("\n2. Flattening Complex Objects:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how AutoMapper can flatten nested objects into a flat structure");
            Console.WriteLine("   by concatenating property names (e.g., Address.City becomes AddressCity).");
            Console.ResetColor();
            FlatteningExample();

            // Example 3: Custom Value Resolvers
            Console.WriteLine("\n3. Custom Value Resolvers:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates using custom value resolvers to implement complex");
            Console.WriteLine("   mapping logic that goes beyond simple property assignments.");
            Console.ResetColor();
            CustomValueResolverExample();

            // Example 4: Projection with LINQ
            Console.WriteLine("\n4. Projection with LINQ:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to use AutoMapper with LINQ queries to project");
            Console.WriteLine("   database entities directly into DTOs for efficient data retrieval.");
            Console.ResetColor();
            ProjectionExample();

            // Example 5: Reverse Mapping
            Console.WriteLine("\n5. Reverse Mapping:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates bidirectional mapping using ReverseMap() to map");
            Console.WriteLine("   from source to destination and vice versa without duplicating configuration.");
            Console.ResetColor();
            ReverseMappingExample();

            // Example 6: Conditional Mapping
            Console.WriteLine("\n6. Conditional Mapping:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to apply conditions to determine when and how properties");
            Console.WriteLine("   are mapped, allowing for selective property mapping based on business rules.");
            Console.ResetColor();
            ConditionalMappingExample();

            // Example 7: Collection Mapping
            Console.WriteLine("\n7. Collection Mapping:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates mapping collections of objects, including lists, arrays,");
            Console.WriteLine("   and IEnumerable types while preserving collection types and relationships.");
            Console.ResetColor();
            CollectionMappingExample();

            Console.WriteLine("\n=== AutoMapper Experiments Completed ===");
        }

        private static void BasicMapping()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<BasicMappingProfile>());
            var mapper = config.CreateMapper();

            var employee = new EmployeeAutoMapper
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Age = 30,
                Department = "Engineering"
            };

            var employeeDto = mapper.Map<EmployeeDto>(employee);

            Console.WriteLine($"ID: {employeeDto.Id}");
            Console.WriteLine($"Name: {employeeDto.FirstName} {employeeDto.LastName}");
            Console.WriteLine($"Email: {employeeDto.Email}");
            Console.WriteLine($"Age: {employeeDto.Age}");
            Console.WriteLine($"Department: {employeeDto.Department}");
        }

        private static void FlatteningExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<FlatteningProfile>());
            var mapper = config.CreateMapper();

            var customer = new Customer
            {
                Id = 1,
                Name = "Jane Smith",
                Address = new Address
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    ZipCode = "10001"
                },
                ContactInfo = new ContactInfo
                {
                    Email = "jane.smith@example.com",
                    Phone = "555-1234"
                }
            };

            var customerDto = mapper.Map<CustomerDto>(customer);

            Console.WriteLine($"Customer: {customerDto.Name}");
            Console.WriteLine($"Address: {customerDto.AddressStreet}, {customerDto.AddressCity}, {customerDto.AddressState} {customerDto.AddressZipCode}");
            Console.WriteLine($"Email: {customerDto.ContactInfoEmail}");
            Console.WriteLine($"Phone: {customerDto.ContactInfoPhone}");
        }

        private static void CustomValueResolverExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ProductMappingProfile>());
            var mapper = config.CreateMapper();

            var product = new Product
            {
                Id = 101,
                Name = "Laptop",
                Category = "Electronics",
                Description = "High-performance laptop",
                Price = 1299.99m,
                InStock = true
            };

            var productDto = mapper.Map<ProductDto>(product);

            Console.WriteLine($"ID: {productDto.Id}");
            Console.WriteLine($"Full Description: {productDto.FullDescription}");
            Console.WriteLine($"Price: {productDto.PriceWithCurrency}");
            Console.WriteLine($"Status: {productDto.Status}");
        }

        private static void ProjectionExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<OrderMappingProfile>());
            var mapper = config.CreateMapper();

            var orders = new List<Order>
            {
                new Order { Id = 1, OrderNumber = "ORD-001", CustomerName = "Alice", TotalAmount = 150.00m, OrderDate = DateTime.Now.AddDays(-5) },
                new Order { Id = 2, OrderNumber = "ORD-002", CustomerName = "Bob", TotalAmount = 200.00m, OrderDate = DateTime.Now.AddDays(-3) },
                new Order { Id = 3, OrderNumber = "ORD-003", CustomerName = "Charlie", TotalAmount = 99.99m, OrderDate = DateTime.Now.AddDays(-1) }
            };

            var orderSummaries = orders.Select(o => mapper.Map<OrderSummaryDto>(o)).ToList();

            foreach (var summary in orderSummaries)
            {
                Console.WriteLine($"Order: {summary.OrderNumber} | Customer: {summary.CustomerName} | Total: ${summary.TotalAmount:F2} | Date: {summary.OrderDate:d}");
            }
        }

        private static void ReverseMappingExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<BookMappingProfile>());
            var mapper = config.CreateMapper();

            var book = new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                ISBN = "978-0132350884",
                PublishedYear = 2008
            };

            var bookDto = mapper.Map<BookDto>(book);
            Console.WriteLine($"DTO: {bookDto.Title} by {bookDto.Author} ({bookDto.PublishedYear})");

            bookDto.Title = "Clean Architecture";
            bookDto.PublishedYear = 2017;

            var updatedBook = mapper.Map<Book>(bookDto);
            Console.WriteLine($"Entity: {updatedBook.Title} by {updatedBook.Author} ({updatedBook.PublishedYear})");
        }

        private static void ConditionalMappingExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
            var mapper = config.CreateMapper();

            var user1 = new User
            {
                Id = 1,
                Username = "jdoe",
                Nickname = "Johnny",
                DateOfBirth = new DateTime(1990, 5, 15),
                LastLoginDate = DateTime.Now.AddDays(-5)
            };

            var user2 = new User
            {
                Id = 2,
                Username = "asmith",
                Nickname = null,
                DateOfBirth = null,
                LastLoginDate = DateTime.Now.AddDays(-45)
            };

            var profile1 = mapper.Map<UserProfileDto>(user1);
            var profile2 = mapper.Map<UserProfileDto>(user2);

            Console.WriteLine($"User 1: {profile1.DisplayName}, Age: {profile1.Age}, Active: {profile1.IsActive}");
            Console.WriteLine($"User 2: {profile2.DisplayName}, Age: {profile2.Age?.ToString() ?? "N/A"}, Active: {profile2.IsActive}");
        }

        private static void CollectionMappingExample()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<StudentMappingProfile>());
            var mapper = config.CreateMapper();

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Alice", Grade = 95.5 },
                new Student { Id = 2, Name = "Bob", Grade = 87.3 },
                new Student { Id = 3, Name = "Charlie", Grade = 92.1 }
            };

            var studentDtos = mapper.Map<List<StudentDto>>(students);

            Console.WriteLine("Students:");
            foreach (var student in studentDtos)
            {
                Console.WriteLine($"  {student.Name}: {student.Grade}%");
            }
        }
    }

    #region Mapping Profiles

    public class BasicMappingProfile : Profile
    {
        public BasicMappingProfile()
        {
            CreateMap<EmployeeAutoMapper, EmployeeDto>();
        }
    }

    public class FlatteningProfile : Profile
    {
        public FlatteningProfile()
        {
            CreateMap<Customer, CustomerDto>();
        }
    }

    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.FullDescription,
                    opt => opt.MapFrom(src => $"{src.Name} - {src.Category}: {src.Description}"))
                .ForMember(dest => dest.PriceWithCurrency,
                    opt => opt.MapFrom(src => $"${src.Price:F2}"))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.InStock ? "Available" : "Out of Stock"));
        }
    }

    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderSummaryDto>();
        }
    }

    public class BookMappingProfile : Profile
    {
        public BookMappingProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }

    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.DisplayName,
                    opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.Nickname) ? src.Nickname : src.Username))
                .ForMember(dest => dest.Age,
                    opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? DateTime.Now.Year - src.DateOfBirth.Value.Year : (int?)null))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom(src => src.LastLoginDate.HasValue && src.LastLoginDate.Value > DateTime.Now.AddDays(-30)));
        }
    }

    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<Student, StudentDto>();
            CreateMap<Course, CourseDto>();
        }
    }

    #endregion

    #region Models

    public class EmployeeAutoMapper
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public int Age { get; set; }
        public required string Department { get; set; }
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public int Age { get; set; }
        public required string Department { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Address Address { get; set; }
        public required ContactInfo ContactInfo { get; set; }
    }

    public class Address
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
    }

    public class ContactInfo
    {
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string AddressStreet { get; set; }
        public required string AddressCity { get; set; }
        public required string AddressState { get; set; }
        public required string AddressZipCode { get; set; }
        public required string ContactInfoEmail { get; set; }
        public required string ContactInfoPhone { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public required string FullDescription { get; set; }
        public required string PriceWithCurrency { get; set; }
        public required string Status { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public required string OrderNumber { get; set; }
        public required string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class OrderSummaryDto
    {
        public required string OrderNumber { get; set; }
        public required string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
    }

    public class BookDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required string ISBN { get; set; }
        public int PublishedYear { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string? Nickname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class UserProfileDto
    {
        public required string DisplayName { get; set; }
        public int? Age { get; set; }
        public bool IsActive { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double Grade { get; set; }
    }

    public class StudentDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double Grade { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public required string CourseName { get; set; }
    }

    public class CourseDto
    {
        public int Id { get; set; }
        public required string CourseName { get; set; }
    }

    #endregion
}
