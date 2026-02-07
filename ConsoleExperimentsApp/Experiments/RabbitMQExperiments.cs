using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleExperimentsApp.Experiments
{
    public static class RabbitMQExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== RabbitMQ Experiments ===");
            Console.ResetColor();

            try
            {
                // Experiment 1: Simple Queue (Producer/Consumer)
                await SimpleQueueExample();

                // Experiment 2: Work Queues (Round-robin distribution)
                await WorkQueueExample();

                // Experiment 3: Publish/Subscribe (Fanout Exchange)
                await PublishSubscribeExample();

                // Experiment 4: Routing (Direct Exchange)
                await RoutingExample();

                // Experiment 5: Topics (Topic Exchange)
                await TopicExample();

                // Experiment 6: RPC Pattern
                await RpcExample();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        #region Experiment 1: Simple Queue
        private static async Task SimpleQueueExample()
        {
            Console.WriteLine("\n--- Experiment 1: Simple Queue (Producer/Consumer) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string queueName = "hello_queue";
            await channel.QueueDeclareAsync(queue: queueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            // Producer: Send a message
            string message = "Hello RabbitMQ!";
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty,
                               routingKey: queueName,
                               mandatory: false,
                               body: body);
            Console.WriteLine($"[Producer] Sent: {message}");

            // Consumer: Receive a message
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var receivedBody = ea.Body.ToArray();
                var receivedMessage = Encoding.UTF8.GetString(receivedBody);
                Console.WriteLine($"[Consumer] Received: {receivedMessage}");
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: queueName,
                               autoAck: true,
                               consumer: consumer);

            await Task.Delay(1000); // Wait for message processing
        }
        #endregion

        #region Experiment 2: Work Queue
        private static async Task WorkQueueExample()
        {
            Console.WriteLine("\n--- Experiment 2: Work Queues (Task Distribution) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string queueName = "task_queue";
            await channel.QueueDeclareAsync(queue: queueName,
                                durable: true, // Queue survives broker restart
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            // Set prefetch count to distribute work fairly
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            // Producer: Send multiple tasks
            var tasks = new[] { "Task 1...", "Task 2....", "Task 3...", "Task 4.....", "Task 5." };
            var properties = new BasicProperties { Persistent = true }; // Make messages persistent

            foreach (var task in tasks)
            {
                var body = Encoding.UTF8.GetBytes(task);
                await channel.BasicPublishAsync(exchange: string.Empty,
                                   routingKey: queueName,
                                   mandatory: false,
                                   basicProperties: properties,
                                   body: body);
                Console.WriteLine($"[Producer] Sent task: {task}");
            }

            // Consumer: Process tasks with manual acknowledgment
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Worker] Processing: {message}");

                // Simulate work
                int dots = message.Count(c => c == '.');
                await Task.Delay(dots * 100);

                Console.WriteLine($"[Worker] Done: {message}");
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: queueName,
                               autoAck: false, // Manual acknowledgment
                               consumer: consumer);

            await Task.Delay(3000); // Wait for task processing
        }
        #endregion

        #region Experiment 3: Publish/Subscribe (Fanout)
        private static async Task PublishSubscribeExample()
        {
            Console.WriteLine("\n--- Experiment 3: Publish/Subscribe (Fanout Exchange) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string exchangeName = "logs";
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Fanout);

            // Create two subscribers with temporary queues
            var queueResult1 = await channel.QueueDeclareAsync();
            var queueResult2 = await channel.QueueDeclareAsync();
            var queue1 = queueResult1.QueueName;
            var queue2 = queueResult2.QueueName;

            await channel.QueueBindAsync(queue: queue1, exchange: exchangeName, routingKey: string.Empty);
            await channel.QueueBindAsync(queue: queue2, exchange: exchangeName, routingKey: string.Empty);

            // Subscriber 1
            var consumer1 = new AsyncEventingBasicConsumer(channel);
            consumer1.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Subscriber 1] Received: {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: queue1, autoAck: true, consumer: consumer1);

            // Subscriber 2
            var consumer2 = new AsyncEventingBasicConsumer(channel);
            consumer2.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Subscriber 2] Received: {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: queue2, autoAck: true, consumer: consumer2);

            // Publisher: Broadcast messages
            var messages = new[] { "Info: Application started", "Warning: Low memory", "Error: Connection failed" };
            foreach (var msg in messages)
            {
                var body = Encoding.UTF8.GetBytes(msg);
                await channel.BasicPublishAsync(exchange: exchangeName,
                                   routingKey: string.Empty,
                                   mandatory: false,
                                   body: body);
                Console.WriteLine($"[Publisher] Broadcast: {msg}");
            }

            await Task.Delay(1000);
        }
        #endregion

        #region Experiment 4: Routing (Direct Exchange)
        private static async Task RoutingExample()
        {
            Console.WriteLine("\n--- Experiment 4: Routing (Direct Exchange) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string exchangeName = "direct_logs";
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Direct);

            // Create subscribers for specific severities
            var errorQueue = (await channel.QueueDeclareAsync()).QueueName;
            var infoQueue = (await channel.QueueDeclareAsync()).QueueName;
            var warningQueue = (await channel.QueueDeclareAsync()).QueueName;

            await channel.QueueBindAsync(queue: errorQueue, exchange: exchangeName, routingKey: "error");
            await channel.QueueBindAsync(queue: infoQueue, exchange: exchangeName, routingKey: "info");
            await channel.QueueBindAsync(queue: warningQueue, exchange: exchangeName, routingKey: "warning");

            // Error subscriber
            var errorConsumer = new AsyncEventingBasicConsumer(channel);
            errorConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[ERROR Handler] {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: errorQueue, autoAck: true, consumer: errorConsumer);

            // Info subscriber
            var infoConsumer = new AsyncEventingBasicConsumer(channel);
            infoConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[INFO Handler] {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: infoQueue, autoAck: true, consumer: infoConsumer);

            // Warning subscriber
            var warningConsumer = new AsyncEventingBasicConsumer(channel);
            warningConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[WARNING Handler] {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: warningQueue, autoAck: true, consumer: warningConsumer);

            // Publisher: Send messages with routing keys
            var logMessages = new[]
            {
                ("info", "User logged in"),
                ("warning", "Disk space low"),
                ("error", "Database connection lost"),
                ("info", "Request processed successfully")
            };

            foreach (var (severity, message) in logMessages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync(exchange: exchangeName,
                                   routingKey: severity,
                                   mandatory: false,
                                   body: body);
                Console.WriteLine($"[Publisher] Sent [{severity}]: {message}");
            }

            await Task.Delay(1000);
        }
        #endregion

        #region Experiment 5: Topics (Topic Exchange)
        private static async Task TopicExample()
        {
            Console.WriteLine("\n--- Experiment 5: Topics (Topic Exchange) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string exchangeName = "topic_logs";
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

            // Create subscribers with topic patterns
            var allErrorsQueue = (await channel.QueueDeclareAsync()).QueueName;
            var kernelQueue = (await channel.QueueDeclareAsync()).QueueName;
            var criticalQueue = (await channel.QueueDeclareAsync()).QueueName;

            // Bind to different patterns
            await channel.QueueBindAsync(queue: allErrorsQueue, exchange: exchangeName, routingKey: "*.error");
            await channel.QueueBindAsync(queue: kernelQueue, exchange: exchangeName, routingKey: "kern.*");
            await channel.QueueBindAsync(queue: criticalQueue, exchange: exchangeName, routingKey: "*.critical.*");

            // All errors subscriber
            var allErrorsConsumer = new AsyncEventingBasicConsumer(channel);
            allErrorsConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[All Errors] {ea.RoutingKey}: {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: allErrorsQueue, autoAck: true, consumer: allErrorsConsumer);

            // Kernel subscriber
            var kernelConsumer = new AsyncEventingBasicConsumer(channel);
            kernelConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Kernel] {ea.RoutingKey}: {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: kernelQueue, autoAck: true, consumer: kernelConsumer);

            // Critical subscriber
            var criticalConsumer = new AsyncEventingBasicConsumer(channel);
            criticalConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[Critical] {ea.RoutingKey}: {message}");
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: criticalQueue, autoAck: true, consumer: criticalConsumer);

            // Publisher: Send messages with topic routing keys
            var topicMessages = new[]
            {
                ("kern.info", "Kernel started"),
                ("kern.error", "Kernel panic!"),
                ("app.error", "Application crashed"),
                ("app.critical.alert", "System overload"),
                ("db.critical.failure", "Database unavailable")
            };

            foreach (var (routingKey, message) in topicMessages)
            {
                var body = Encoding.UTF8.GetBytes(message);
                await channel.BasicPublishAsync(exchange: exchangeName,
                                   routingKey: routingKey,
                                   mandatory: false,
                                   body: body);
                Console.WriteLine($"[Publisher] Sent [{routingKey}]: {message}");
            }

            await Task.Delay(1000);
        }
        #endregion

        #region Experiment 6: RPC Pattern
        private static async Task RpcExample()
        {
            Console.WriteLine("\n--- Experiment 6: RPC Pattern (Request/Reply) ---");

            var factory = new ConnectionFactory { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Setup RPC server queue
            string rpcQueueName = "rpc_queue";
            await channel.QueueDeclareAsync(queue: rpcQueueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            // RPC Server: Process requests and send replies
            var serverConsumer = new AsyncEventingBasicConsumer(channel);
            serverConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = new BasicProperties { CorrelationId = props.CorrelationId };

                string response = string.Empty;
                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    int number = int.Parse(message);
                    Console.WriteLine($"[RPC Server] Computing Fibonacci({number})");
                    response = Fibonacci(number).ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[RPC Server] Error: {e.Message}");
                    response = string.Empty;
                }

                var responseBytes = Encoding.UTF8.GetBytes(response);
                await channel.BasicPublishAsync(exchange: string.Empty,
                                   routingKey: props.ReplyTo!,
                                   mandatory: false,
                                   basicProperties: replyProps,
                                   body: responseBytes);
                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                Console.WriteLine($"[RPC Server] Sent response: {response}");
            };

            await channel.BasicConsumeAsync(queue: rpcQueueName, autoAck: false, consumer: serverConsumer);

            // RPC Client: Send request and wait for reply
            var replyQueue = (await channel.QueueDeclareAsync()).QueueName;
            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>();

            var clientConsumer = new AsyncEventingBasicConsumer(channel);
            clientConsumer.ReceivedAsync += async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    tcs.TrySetResult(response);
                }
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(queue: replyQueue, autoAck: true, consumer: clientConsumer);

            // Send RPC request
            int requestNumber = 10;
            var requestProps = new BasicProperties 
            { 
                CorrelationId = correlationId,
                ReplyTo = replyQueue
            };

            var requestBody = Encoding.UTF8.GetBytes(requestNumber.ToString());
            await channel.BasicPublishAsync(exchange: string.Empty,
                               routingKey: rpcQueueName,
                               mandatory: false,
                               basicProperties: requestProps,
                               body: requestBody);
            Console.WriteLine($"[RPC Client] Requesting Fibonacci({requestNumber})");

            // Wait for response with timeout
            var responseTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
            if (responseTask == tcs.Task)
            {
                Console.WriteLine($"[RPC Client] Received result: {await tcs.Task}");
            }
            else
            {
                Console.WriteLine("[RPC Client] Timeout waiting for response");
            }
        }

        private static int Fibonacci(int n)
        {
            if (n == 0 || n == 1) return n;
            return Fibonacci(n - 1) + Fibonacci(n - 2);
        }
        #endregion
    }
}
