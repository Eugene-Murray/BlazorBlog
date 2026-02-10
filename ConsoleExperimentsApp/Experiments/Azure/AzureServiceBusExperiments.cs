using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace ConsoleExperimentsApp.Experiments.Azure
{
    public static class AzureServiceBusExperiments
    {
        // Replace with your Service Bus connection string
        private const string ConnectionString = "<your-service-bus-connection-string>";
        private const string QueueName = "sample-queue";
        private const string TopicName = "sample-topic";
        private const string SubscriptionName = "sample-subscription";
        private const string SessionQueueName = "session-queue";

        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Azure Service Bus Comprehensive Examples ===");
            Console.WriteLine("Description: Demonstrates various Azure Service Bus patterns and features");
            Console.ResetColor();

            try
            {
                // Uncomment the examples you want to run
                await ReceiveMessagesFromQueueExample();
                await SendMessageToQueueExample();
                await SendBatchMessagesToQueueExample();
                await SendMessageToTopicExample();
                await ReceiveMessagesFromSubscriptionExample();
                await SendScheduledMessageExample();
                await SessionMessagesExample();
                await DeadLetterQueueExample();
                await MessagePropertiesExample();
                await TransactionExample();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        #region Queue Operations

        private static async Task SendMessageToQueueExample()
        {
            Console.WriteLine("\n--- Send Message to Queue Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(QueueName);

            try
            {
                var message = new ServiceBusMessage("Hello, Azure Service Bus!")
                {
                    ContentType = "text/plain",
                    MessageId = Guid.NewGuid().ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5)
                };

                await sender.SendMessageAsync(message);
                Console.WriteLine($"✓ Message sent: {message.MessageId}");
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        private static async Task ReceiveMessagesFromQueueExample()
        {
            Console.WriteLine("\n--- Receive Messages from Queue Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(QueueName);

            try
            {
                // Receive messages with a timeout
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));

                foreach (var message in receivedMessages)
                {
                    Console.WriteLine($"✓ Received: {message.Body} (MessageId: {message.MessageId})");

                    // Complete the message to remove it from the queue
                    await receiver.CompleteMessageAsync(message);
                    Console.WriteLine($"  Message completed: {message.MessageId}");
                }

                if (receivedMessages.Count == 0)
                {
                    Console.WriteLine("No messages available.");
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        private static async Task SendBatchMessagesToQueueExample()
        {
            Console.WriteLine("\n--- Send Batch Messages to Queue Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(QueueName);

            try
            {
                // Create a batch
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                for (int i = 1; i <= 5; i++)
                {
                    var message = new ServiceBusMessage($"Batch message {i}")
                    {
                        MessageId = Guid.NewGuid().ToString()
                    };

                    if (!messageBatch.TryAddMessage(message))
                    {
                        Console.WriteLine($"⚠ Message {i} is too large to fit in the batch.");
                    }
                    else
                    {
                        Console.WriteLine($"✓ Added message {i} to batch");
                    }
                }

                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"✓ Batch sent with {messageBatch.Count} messages");
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        #endregion

        #region Topic/Subscription Operations

        private static async Task SendMessageToTopicExample()
        {
            Console.WriteLine("\n--- Send Message to Topic Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(TopicName);

            try
            {
                var orderData = new
                {
                    OrderId = 12345,
                    CustomerName = "John Doe",
                    TotalAmount = 299.99,
                    OrderDate = DateTime.UtcNow
                };

                string messageBody = JsonSerializer.Serialize(orderData);
                var message = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = "OrderCreated"
                };

                // Add custom properties
                message.ApplicationProperties.Add("OrderType", "Online");
                message.ApplicationProperties.Add("Priority", "High");

                await sender.SendMessageAsync(message);
                Console.WriteLine($"✓ Message sent to topic: {message.MessageId}");
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        private static async Task ReceiveMessagesFromSubscriptionExample()
        {
            Console.WriteLine("\n--- Receive Messages from Subscription Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(TopicName, SubscriptionName);

            try
            {
                var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));

                foreach (var message in receivedMessages)
                {
                    Console.WriteLine($"✓ Received: {message.Body}");
                    Console.WriteLine($"  Subject: {message.Subject}");
                    Console.WriteLine($"  ContentType: {message.ContentType}");

                    // Display custom properties
                    if (message.ApplicationProperties.Any())
                    {
                        Console.WriteLine("  Custom Properties:");
                        foreach (var prop in message.ApplicationProperties)
                        {
                            Console.WriteLine($"    {prop.Key}: {prop.Value}");
                        }
                    }

                    await receiver.CompleteMessageAsync(message);
                }

                if (receivedMessages.Count == 0)
                {
                    Console.WriteLine("No messages available.");
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        #endregion

        #region Advanced Scenarios

        private static async Task SendScheduledMessageExample()
        {
            Console.WriteLine("\n--- Send Scheduled Message Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(QueueName);

            try
            {
                var message = new ServiceBusMessage("This is a scheduled message")
                {
                    MessageId = Guid.NewGuid().ToString()
                };

                // Schedule message to be available in 2 minutes
                DateTimeOffset scheduledEnqueueTime = DateTimeOffset.UtcNow.AddMinutes(2);
                long sequenceNumber = await sender.ScheduleMessageAsync(message, scheduledEnqueueTime);

                Console.WriteLine($"✓ Message scheduled for: {scheduledEnqueueTime}");
                Console.WriteLine($"  Sequence Number: {sequenceNumber}");

                // You can cancel a scheduled message using:
                // await sender.CancelScheduledMessageAsync(sequenceNumber);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        private static async Task SessionMessagesExample()
        {
            Console.WriteLine("\n--- Session Messages Example ---");

            await using var client = new ServiceBusClient(ConnectionString);

            // Send messages with session
            ServiceBusSender sender = client.CreateSender(SessionQueueName);
            try
            {
                string sessionId = "session-1";

                for (int i = 1; i <= 3; i++)
                {
                    var message = new ServiceBusMessage($"Session message {i}")
                    {
                        SessionId = sessionId,
                        MessageId = Guid.NewGuid().ToString()
                    };

                    await sender.SendMessageAsync(message);
                    Console.WriteLine($"✓ Sent message {i} to session: {sessionId}");
                }
            }
            finally
            {
                await sender.DisposeAsync();
            }

            // Receive messages from session
            ServiceBusSessionReceiver sessionReceiver = await client.AcceptNextSessionAsync(SessionQueueName);
            try
            {
                Console.WriteLine($"✓ Accepted session: {sessionReceiver.SessionId}");

                var messages = await sessionReceiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));

                foreach (var message in messages)
                {
                    Console.WriteLine($"✓ Received from session: {message.Body}");
                    await sessionReceiver.CompleteMessageAsync(message);
                }

                // Set session state
                await sessionReceiver.SetSessionStateAsync(new BinaryData("Session processing complete"));
                Console.WriteLine("✓ Session state updated");

                // Get session state
                BinaryData sessionState = await sessionReceiver.GetSessionStateAsync();
                Console.WriteLine($"✓ Session state: {sessionState}");
            }
            finally
            {
                await sessionReceiver.DisposeAsync();
            }
        }

        private static async Task DeadLetterQueueExample()
        {
            Console.WriteLine("\n--- Dead Letter Queue Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(QueueName);

            try
            {
                var messages = await receiver.ReceiveMessagesAsync(maxMessages: 1, maxWaitTime: TimeSpan.FromSeconds(5));

                if (messages.Any())
                {
                    var message = messages.First();
                    Console.WriteLine($"✓ Received message: {message.MessageId}");

                    // Simulate processing failure and send to dead letter queue
                    await receiver.DeadLetterMessageAsync(
                        message,
                        deadLetterReason: "Processing failed",
                        deadLetterErrorDescription: "Simulated error for demonstration");

                    Console.WriteLine("✓ Message moved to dead letter queue");
                }

                // Process dead letter queue
                ServiceBusReceiver deadLetterReceiver = client.CreateReceiver(
                    QueueName,
                    new ServiceBusReceiverOptions
                    {
                        SubQueue = SubQueue.DeadLetter
                    });

                try
                {
                    var deadLetterMessages = await deadLetterReceiver.ReceiveMessagesAsync(maxMessages: 10, maxWaitTime: TimeSpan.FromSeconds(5));

                    foreach (var dlMessage in deadLetterMessages)
                    {
                        Console.WriteLine($"✓ Dead letter message: {dlMessage.MessageId}");
                        Console.WriteLine($"  Reason: {dlMessage.DeadLetterReason}");
                        Console.WriteLine($"  Description: {dlMessage.DeadLetterErrorDescription}");

                        // Complete to remove from dead letter queue
                        await deadLetterReceiver.CompleteMessageAsync(dlMessage);
                    }
                }
                finally
                {
                    await deadLetterReceiver.DisposeAsync();
                }
            }
            finally
            {
                await receiver.DisposeAsync();
            }
        }

        private static async Task MessagePropertiesExample()
        {
            Console.WriteLine("\n--- Message Properties Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(QueueName);

            try
            {
                var message = new ServiceBusMessage("Message with various properties")
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json",
                    Subject = "UserRegistration",
                    CorrelationId = Guid.NewGuid().ToString(),
                    ReplyTo = "response-queue",
                    TimeToLive = TimeSpan.FromMinutes(10)
                };

                // Add custom application properties
                message.ApplicationProperties.Add("UserId", 12345);
                message.ApplicationProperties.Add("Department", "Sales");
                message.ApplicationProperties.Add("Priority", "High");
                message.ApplicationProperties.Add("Timestamp", DateTime.UtcNow);

                await sender.SendMessageAsync(message);
                Console.WriteLine("✓ Message sent with properties:");
                Console.WriteLine($"  MessageId: {message.MessageId}");
                Console.WriteLine($"  Subject: {message.Subject}");
                Console.WriteLine($"  CorrelationId: {message.CorrelationId}");
                Console.WriteLine($"  ReplyTo: {message.ReplyTo}");
                Console.WriteLine($"  TimeToLive: {message.TimeToLive}");
                Console.WriteLine("  Application Properties:");
                foreach (var prop in message.ApplicationProperties)
                {
                    Console.WriteLine($"    {prop.Key}: {prop.Value}");
                }
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        private static async Task TransactionExample()
        {
            Console.WriteLine("\n--- Transaction Example ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(QueueName);
            ServiceBusReceiver receiver = client.CreateReceiver(QueueName);

            try
            {
                // First, send a message to receive
                var initialMessage = new ServiceBusMessage("Message for transaction test")
                {
                    MessageId = Guid.NewGuid().ToString()
                };
                await sender.SendMessageAsync(initialMessage);
                Console.WriteLine("✓ Initial message sent");

                // Wait a bit for message to be available
                await Task.Delay(1000);

                // Receive and process in a transaction
                var messages = await receiver.ReceiveMessagesAsync(maxMessages: 1, maxWaitTime: TimeSpan.FromSeconds(5));

                if (messages.Any())
                {
                    var message = messages.First();
                    Console.WriteLine($"✓ Received message: {message.MessageId}");

                    // Create a transaction scope
                    using var ts = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeAsyncFlowOption.Enabled);

                    // Complete the received message
                    await receiver.CompleteMessageAsync(message);
                    Console.WriteLine("✓ Message completed in transaction");

                    // Send a new message as part of the same transaction
                    var newMessage = new ServiceBusMessage("Transaction processed successfully")
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        CorrelationId = message.MessageId
                    };
                    await sender.SendMessageAsync(newMessage);
                    Console.WriteLine("✓ New message sent in transaction");

                    // Complete the transaction
                    ts.Complete();
                    Console.WriteLine("✓ Transaction committed");
                }
                else
                {
                    Console.WriteLine("No messages available for transaction example");
                }
            }
            finally
            {
                await receiver.DisposeAsync();
                await sender.DisposeAsync();
            }
        }

        #endregion

        #region Processor Pattern (Event-Driven)

        public static async Task ProcessorExample()
        {
            Console.WriteLine("\n--- Message Processor Example (Event-Driven) ---");

            await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusProcessor processor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
            });

            // Configure event handlers
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            try
            {
                await processor.StartProcessingAsync();
                Console.WriteLine("✓ Processor started. Press any key to stop...");
                Console.ReadKey();

                await processor.StopProcessingAsync();
                Console.WriteLine("✓ Processor stopped");
            }
            finally
            {
                await processor.DisposeAsync();
            }
        }

        private static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"✓ Processing message: {body} (MessageId: {args.Message.MessageId})");

            try
            {
                // Process the message here
                await Task.Delay(100); // Simulate processing

                // Complete the message
                await args.CompleteMessageAsync(args.Message);
                Console.WriteLine($"✓ Completed message: {args.Message.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error processing message: {ex.Message}");
                // Abandon or dead-letter the message
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"✗ Error from source {args.ErrorSource}: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        #endregion

        #region Helper Classes

        public class OrderMessage
        {
            public int OrderId { get; set; }
            public string? CustomerName { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime OrderDate { get; set; }
        }

        #endregion
    }
}
