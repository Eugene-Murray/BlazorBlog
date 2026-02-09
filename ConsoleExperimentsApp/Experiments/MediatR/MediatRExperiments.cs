using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleExperimentsApp.Experiments.MediatR
{
    // ===== COMMAND PATTERN (IRequest with void/Unit response) =====
    public record CreateUserCommand(string Name, string Email) : IRequest<int>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Creating user: {request.Name} ({request.Email})");
            var userId = new Random().Next(1, 1000);
            Console.WriteLine($"✓ User created with ID: {userId}");
            return Task.FromResult(userId);
        }
    }

    // ===== QUERY PATTERN (IRequest with return value) =====
    public record GetUserByIdQuery(int UserId) : IRequest<UserDto?>;

    public record UserDto(int Id, string Name, string Email);

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        public Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Fetching user with ID: {request.UserId}");

            // Simulate database lookup
            if (request.UserId > 0)
            {
                var user = new UserDto(request.UserId, $"User{request.UserId}", $"user{request.UserId}@example.com");
                Console.WriteLine($"✓ User found: {user.Name}");
                return Task.FromResult<UserDto?>(user);
            }

            Console.WriteLine("✗ User not found");
            return Task.FromResult<UserDto?>(null);
        }
    }

    // ===== NOTIFICATION PATTERN (INotification with multiple handlers) =====
    public record UserCreatedNotification(int UserId, string Name) : INotification;

    public class EmailNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"  📧 Sending welcome email to {notification.Name}...");
            return Task.CompletedTask;
        }
    }

    public class LoggingNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"  📝 Logging user creation: {notification.Name} (ID: {notification.UserId})");
            return Task.CompletedTask;
        }
    }

    public class AnalyticsNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"  📊 Sending analytics event for user: {notification.UserId}");
            return Task.CompletedTask;
        }
    }

    // ===== PIPELINE BEHAVIORS (Cross-cutting concerns) =====
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            Console.WriteLine($"[Pipeline] → Handling {requestName}");

            var response = await next();

            Console.WriteLine($"[Pipeline] ← Handled {requestName}");
            return response;
        }
    }

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            Console.WriteLine($"[Validation] Validating {requestName}");

            // Simple validation example
            if (request is CreateUserCommand command)
            {
                if (string.IsNullOrWhiteSpace(command.Name))
                {
                    throw new ArgumentException("Name cannot be empty");
                }
            }

            return await next();
        }
    }

    // ===== STREAM REQUEST (IStreamRequest for async enumerable) =====
    public record GetUsersStreamQuery(int Count) : IStreamRequest<UserDto>;

    public class GetUsersStreamQueryHandler : IStreamRequestHandler<GetUsersStreamQuery, UserDto>
    {
        public async IAsyncEnumerable<UserDto> Handle(
            GetUsersStreamQuery request, 
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Streaming {request.Count} users...");

            for (int i = 1; i <= request.Count; i++)
            {
                await Task.Delay(100, cancellationToken); // Simulate delay
                yield return new UserDto(i, $"User{i}", $"user{i}@example.com");
            }
        }
    }

    public static class MediatRExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== MediatR Experiments ===");
            Console.WriteLine("Description: Demonstrating the Mediator pattern for CQRS with commands, queries, notifications, and pipeline behaviors.");
            Console.ResetColor();

            // Setup DI container
            var services = new ServiceCollection();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRExperiments).Assembly));

            // Register pipeline behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var serviceProvider = services.BuildServiceProvider();
            var sender = serviceProvider.GetRequiredService<ISender>();
            var publisher = serviceProvider.GetRequiredService<IPublisher>();

            // Example 1: Command Pattern
            Console.WriteLine("\n1️⃣  COMMAND PATTERN EXAMPLE");
            Console.WriteLine("─────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates using IRequest for commands that modify state");
            Console.WriteLine("and return a result, following the CQRS command pattern.");
            Console.ResetColor();
            var userId = await sender.Send(new CreateUserCommand("John Doe", "john@example.com"));
            Console.WriteLine($"Returned User ID: {userId}\n");

            // Example 2: Query Pattern
            Console.WriteLine("2️⃣  QUERY PATTERN EXAMPLE");
            Console.WriteLine("─────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows IRequest for queries that retrieve data without modifying state,");
            Console.WriteLine("implementing the CQRS query pattern.");
            Console.ResetColor();
            var user = await sender.Send(new GetUserByIdQuery(userId));
            if (user != null)
            {
                Console.WriteLine($"Retrieved: {user.Name} - {user.Email}");
            }
            Console.WriteLine();

            // Example 3: Notification Pattern (One-to-Many)
            Console.WriteLine("3️⃣  NOTIFICATION PATTERN EXAMPLE");
            Console.WriteLine("─────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates INotification for publishing events to multiple handlers,");
            Console.WriteLine("implementing the observer pattern for decoupled event handling.");
            Console.ResetColor();
            Console.WriteLine("Publishing notification (will trigger multiple handlers):");
            await publisher.Publish(new UserCreatedNotification(userId, "John Doe"));
            Console.WriteLine();

            // Example 4: Stream Request Pattern
            Console.WriteLine("4️⃣  STREAM REQUEST PATTERN EXAMPLE");
            Console.WriteLine("─────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows IStreamRequest for returning asynchronous streams of data,");
            Console.WriteLine("useful for processing large datasets efficiently with IAsyncEnumerable.");
            Console.ResetColor();
            await foreach (var streamUser in sender.CreateStream(new GetUsersStreamQuery(3)))
            {
                Console.WriteLine($"  → Received: {streamUser.Name}");
            }
            Console.WriteLine();

            // Example 5: Pipeline Behaviors Demo
            Console.WriteLine("5️⃣  PIPELINE BEHAVIORS EXAMPLE");
            Console.WriteLine("────────────────────────────────");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates IPipelineBehavior for implementing cross-cutting concerns");
            Console.WriteLine("like logging, validation, and error handling that run for every request.");
            Console.ResetColor();
            Console.WriteLine("Notice the [Pipeline] and [Validation] logs above - they run for every request!");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ All MediatR examples completed successfully!");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }
    }
}
