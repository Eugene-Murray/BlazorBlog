using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

namespace ConsoleExperimentsApp.Experiments
{
    public static class NServiceBusExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== NService Bus Experiments ===");
            Console.ResetColor();

            try
            {
                // Experiment 1: Basic Endpoint Configuration
                await BasicEndpointConfiguration();

                // Experiment 2: Send and Publish Messages
                await SendAndPublishMessages();

                // Experiment 3: Message Handler Patterns
                await MessageHandlerPatterns();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static async Task BasicEndpointConfiguration()
        {
            Console.WriteLine("\n--- Experiment 1: Basic Endpoint Configuration ---");

            var endpointConfiguration = new EndpointConfiguration("NServiceBusExperiments");

            // Use Learning Transport (no infrastructure required)
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(".learningtransport");

            // Enable installers for development
            endpointConfiguration.EnableInstallers();

            // Configure error queue
            endpointConfiguration.SendFailedMessagesTo("error");

            // Configure audit queue
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            Console.WriteLine("✓ Endpoint configured with Learning Transport");
            Console.WriteLine("✓ Storage directory: .learningtransport");
            Console.WriteLine("✓ Error queue: error");
            Console.WriteLine("✓ Audit queue: audit");

            // Start endpoint
            var endpointInstance = await Endpoint.Start(endpointConfiguration);
            Console.WriteLine("✓ Endpoint started successfully");

            await endpointInstance.Stop();
            Console.WriteLine("✓ Endpoint stopped");
        }

        private static async Task SendAndPublishMessages()
        {
            Console.WriteLine("\n--- Experiment 2: Send and Publish Messages ---");

            var endpointConfiguration = new EndpointConfiguration("NServiceBusSender");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(".learningtransport");
            endpointConfiguration.SendFailedMessagesTo("error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            // Send a command
            var command = new PlaceOrderCommand
            {
                OrderId = Guid.NewGuid(),
                ProductName = "NServiceBus License",
                Quantity = 1,
                TotalAmount = 999.99m
            };

            Console.WriteLine($"Sending command: PlaceOrderCommand (OrderId: {command.OrderId})");
            await endpointInstance.Send(command);
            Console.WriteLine("✓ Command sent");

            // Publish an event
            var orderEvent = new OrderPlacedEvent
            {
                OrderId = command.OrderId,
                Timestamp = DateTime.UtcNow
            };

            Console.WriteLine($"Publishing event: OrderPlacedEvent (OrderId: {orderEvent.OrderId})");
            await endpointInstance.Publish(orderEvent);
            Console.WriteLine("✓ Event published");

            await endpointInstance.Stop();
        }

        private static async Task MessageHandlerPatterns()
        {
            Console.WriteLine("\n--- Experiment 3: Message Handler Patterns ---");
            Console.WriteLine("Handler patterns:");
            Console.WriteLine("✓ IHandleMessages<T> - Standard message handler");
            Console.WriteLine("✓ IHandleMessages<IEvent> - Polymorphic handler");
            Console.WriteLine("✓ IHandleMessages<ICommand> - Command handler");
            Console.WriteLine("✓ Saga - Long-running business process");
            Console.WriteLine("\nHandler execution order:");
            Console.WriteLine("1. Message arrives at endpoint");
            Console.WriteLine("2. Transport dequeues message");
            Console.WriteLine("3. Pipeline processing begins");
            Console.WriteLine("4. Handler invoked");
            Console.WriteLine("5. Transaction committed");
            Console.WriteLine("6. Message acknowledged");
        }
    }

    #region Message Definitions

    // Commands - tell system to do something
    public class PlaceOrderCommand : NServiceBus.ICommand
    {
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CancelOrderCommand : NServiceBus.ICommand
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }

    // Events - notify that something happened
    public class OrderPlacedEvent : IEvent
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class OrderCancelledEvent : IEvent
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
    }

    #endregion

    #region Message Handlers

    // Command Handler
    public class PlaceOrderCommandHandler : IHandleMessages<PlaceOrderCommand>
    {
        public async Task Handle(PlaceOrderCommand message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Handling PlaceOrderCommand: OrderId={message.OrderId}");

            // Simulate order processing
            await Task.Delay(100);

            // Publish event to notify other systems
            await context.Publish(new OrderPlacedEvent
            {
                OrderId = message.OrderId,
                Timestamp = DateTime.UtcNow
            });

            Console.WriteLine("✓ Order placed successfully");
        }
    }

    // Event Handler
    public class OrderPlacedEventHandler : IHandleMessages<OrderPlacedEvent>
    {
        public async Task Handle(OrderPlacedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Handling OrderPlacedEvent: OrderId={message.OrderId}");

            // Simulate notification sending
            await Task.Delay(50);

            Console.WriteLine("✓ Order confirmation email sent");
        }
    }

    // Saga - Long-running process coordinator
    public class OrderProcessingSaga : Saga<OrderProcessingSagaData>,
        IAmStartedByMessages<OrderPlacedEvent>,
        IHandleMessages<OrderCancelledEvent>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderProcessingSagaData> mapper)
        {
            mapper.MapSaga(saga => saga.OrderId)
                .ToMessage<OrderPlacedEvent>(message => message.OrderId)
                .ToMessage<OrderCancelledEvent>(message => message.OrderId);
        }

        public async Task Handle(OrderPlacedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Saga started for OrderId: {message.OrderId}");
            Data.OrderId = message.OrderId;
            Data.Status = "Placed";
            Data.StartedAt = DateTime.UtcNow;

            await Task.CompletedTask;
        }

        public async Task Handle(OrderCancelledEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Saga handling cancellation for OrderId: {message.OrderId}");
            Data.Status = "Cancelled";

            // Mark saga as complete
            MarkAsComplete();

            await Task.CompletedTask;
        }
    }

    // Saga Data
    public class OrderProcessingSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
    }

    #endregion
}
