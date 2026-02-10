using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace ConsoleExperimentsApp.Experiments.Azure
{
    public static class AzureCosmosDBExperiments
    {
        // Configuration - Replace with your actual values or use configuration
        private const string EndpointUri = "https://your-account.documents.azure.com:443/";
        private const string PrimaryKey = "your-primary-key";
        private const string DatabaseId = "ExperimentsDB";
        private const string ContainerId = "ProductsContainer";

        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Azure Cosmos DB Comprehensive Examples ===");
            Console.WriteLine("Description: Demonstrates CRUD operations, queries, batch operations, change feed, and more");
            Console.ResetColor();

            try
            {
                // Initialize Cosmos Client
                using CosmosClient cosmosClient = new(EndpointUri, PrimaryKey, new CosmosClientOptions
                {
                    ApplicationName = "CosmosDBExperiments",
                    ConnectionMode = ConnectionMode.Direct,
                    MaxRetryAttemptsOnRateLimitedRequests = 9,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30)
                });

                Console.WriteLine("\n1. Testing Connection...");
                await TestConnection(cosmosClient);

                Console.WriteLine("\n2. Creating Database and Container...");
                var (database, container) = await CreateDatabaseAndContainer(cosmosClient);

                Console.WriteLine("\n3. CRUD Operations...");
                await DemonstrateCRUDOperations(container);

                Console.WriteLine("\n4. Query Examples...");
                await DemonstrateQueryOperations(container);

                Console.WriteLine("\n5. Batch Operations...");
                await DemonstrateBatchOperations(container);

                Console.WriteLine("\n6. Transaction Operations...");
                await DemonstrateTransactions(container);

                Console.WriteLine("\n7. Bulk Operations...");
                await DemonstrateBulkOperations(container);

                Console.WriteLine("\n8. Change Feed Example...");
                await DemonstrateChangeFeed(container);

                Console.WriteLine("\n9. Indexing Policy Example...");
                await DemonstrateIndexingPolicy(database);

                Console.WriteLine("\n10. Stored Procedures...");
                await DemonstrateStoredProcedures(container);

                Console.WriteLine("\n11. Cleanup (optional - commented out)...");
                // await CleanupResources(database);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n✓ All examples completed successfully!");
                Console.ResetColor();
            }
            catch (CosmosException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nCosmos DB Error: {ex.StatusCode} - {ex.Message}");
                Console.ResetColor();
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

        private static async Task TestConnection(CosmosClient client)
        {
            var accountProperties = await client.ReadAccountAsync();
            Console.WriteLine($"Connected to: {accountProperties.Id}");
            Console.WriteLine($"Regions: {string.Join(", ", accountProperties.ReadableRegions.Select(r => r.Name))}");
        }

        private static async Task<(Database database, Container container)> CreateDatabaseAndContainer(CosmosClient client)
        {
            // Create database with autoscale throughput
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(
                DatabaseId,
                ThroughputProperties.CreateAutoscaleThroughput(4000));

            Console.WriteLine($"Database created or exists: {databaseResponse.Database.Id}");

            // Create container with partition key
            var containerProperties = new ContainerProperties
            {
                Id = ContainerId,
                PartitionKeyPath = "/category",
                DefaultTimeToLive = -1 // Enable TTL but items live forever by default
            };

            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
                containerProperties,
                ThroughputProperties.CreateManualThroughput(400));

            Console.WriteLine($"Container created or exists: {containerResponse.Container.Id}");

            return (databaseResponse.Database, containerResponse.Container);
        }

        private static async Task DemonstrateCRUDOperations(Container container)
        {
            // CREATE
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Laptop",
                Category = "Electronics",
                Price = 999.99m,
                Description = "High-performance laptop",
                Tags = ["computing", "portable", "business"],
                InStock = true,
                Metadata = new Dictionary<string, string>
                {
                    ["manufacturer"] = "TechCorp",
                    ["warranty"] = "2 years"
                }
            };

            var createResponse = await container.CreateItemAsync(product, new PartitionKey(product.Category));
            Console.WriteLine($"Created item: {createResponse.Resource.Name} (RU: {createResponse.RequestCharge})");

            // READ
            var readResponse = await container.ReadItemAsync<Product>(product.Id, new PartitionKey(product.Category));
            Console.WriteLine($"Read item: {readResponse.Resource.Name} (RU: {readResponse.RequestCharge})");

            // UPDATE
            product.Price = 899.99m;
            product.InStock = false;
            var replaceResponse = await container.ReplaceItemAsync(product, product.Id, new PartitionKey(product.Category));
            Console.WriteLine($"Updated item: {replaceResponse.Resource.Name} - New price: ${replaceResponse.Resource.Price} (RU: {replaceResponse.RequestCharge})");

            // PATCH (partial update)
            var patchOperations = new List<PatchOperation>
            {
                PatchOperation.Set("/price", 849.99m),
                PatchOperation.Add("/tags/-", "sale")
            };
            var patchResponse = await container.PatchItemAsync<Product>(product.Id, new PartitionKey(product.Category), patchOperations);
            Console.WriteLine($"Patched item: New price: ${patchResponse.Resource.Price} (RU: {patchResponse.RequestCharge})");

            // UPSERT
            product.Description = "Updated description via upsert";
            var upsertResponse = await container.UpsertItemAsync(product, new PartitionKey(product.Category));
            Console.WriteLine($"Upserted item: {upsertResponse.Resource.Name} (RU: {upsertResponse.RequestCharge})");

            // DELETE
            var deleteResponse = await container.DeleteItemAsync<Product>(product.Id, new PartitionKey(product.Category));
            Console.WriteLine($"Deleted item (RU: {deleteResponse.RequestCharge})");
        }

        private static async Task DemonstrateQueryOperations(Container container)
        {
            // Insert sample data
            var products = new List<Product>
            {
                new() { Id = Guid.NewGuid().ToString(), Name = "Phone", Category = "Electronics", Price = 699.99m, InStock = true },
                new() { Id = Guid.NewGuid().ToString(), Name = "Tablet", Category = "Electronics", Price = 499.99m, InStock = true },
                new() { Id = Guid.NewGuid().ToString(), Name = "Desk", Category = "Furniture", Price = 299.99m, InStock = false },
                new() { Id = Guid.NewGuid().ToString(), Name = "Chair", Category = "Furniture", Price = 199.99m, InStock = true }
            };

            foreach (var product in products)
            {
                await container.UpsertItemAsync(product, new PartitionKey(product.Category));
            }

            // Query 1: Simple SQL query
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.category = @category AND c.price < @maxPrice")
                .WithParameter("@category", "Electronics")
                .WithParameter("@maxPrice", 700);

            var iterator = container.GetItemQueryIterator<Product>(queryDefinition);
            Console.WriteLine("Query 1 - Electronics under $700:");
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                foreach (var item in response)
                {
                    Console.WriteLine($"  - {item.Name}: ${item.Price}");
                }
            }

            // Query 2: Cross-partition query
            var crossPartitionQuery = new QueryDefinition("SELECT * FROM c WHERE c.inStock = true ORDER BY c.price DESC");
            var crossPartitionIterator = container.GetItemQueryIterator<Product>(crossPartitionQuery);
            Console.WriteLine("\nQuery 2 - All in-stock items (cross-partition):");
            while (crossPartitionIterator.HasMoreResults)
            {
                var response = await crossPartitionIterator.ReadNextAsync();
                foreach (var item in response)
                {
                    Console.WriteLine($"  - {item.Name} ({item.Category}): ${item.Price}");
                }
            }

            // Query 3: Aggregate query
            var aggregateQuery = new QueryDefinition(
                "SELECT c.category, COUNT(1) as count, AVG(c.price) as avgPrice FROM c GROUP BY c.category");
            var aggregateIterator = container.GetItemQueryIterator<dynamic>(aggregateQuery);
            Console.WriteLine("\nQuery 3 - Category statistics:");
            while (aggregateIterator.HasMoreResults)
            {
                var response = await aggregateIterator.ReadNextAsync();
                foreach (var item in response)
                {
                    Console.WriteLine($"  - {item.category}: {item.count} items, Avg: ${item.avgPrice:F2}");
                }
            }

            // Query 4: LINQ query
            var linqQuery = container.GetItemLinqQueryable<Product>(allowSynchronousQueryExecution: true)
                .Where(p => p.Category == "Electronics" && p.InStock)
                .OrderBy(p => p.Price);

            Console.WriteLine("\nQuery 4 - LINQ query for Electronics:");
            foreach (var item in linqQuery)
            {
                Console.WriteLine($"  - {item.Name}: ${item.Price}");
            }
        }

        private static async Task DemonstrateBatchOperations(Container container)
        {
            var category = "BatchTest";
            var partitionKey = new PartitionKey(category);

            var batch = container.CreateTransactionalBatch(partitionKey);

            var item1 = new Product { Id = Guid.NewGuid().ToString(), Name = "Batch Item 1", Category = category, Price = 10.00m };
            var item2 = new Product { Id = Guid.NewGuid().ToString(), Name = "Batch Item 2", Category = category, Price = 20.00m };
            var item3 = new Product { Id = Guid.NewGuid().ToString(), Name = "Batch Item 3", Category = category, Price = 30.00m };

            batch.CreateItem(item1);
            batch.CreateItem(item2);
            batch.CreateItem(item3);

            var batchResponse = await batch.ExecuteAsync();

            if (batchResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Batch operation succeeded - Created 3 items (RU: {batchResponse.RequestCharge})");
            }
            else
            {
                Console.WriteLine($"Batch operation failed: {batchResponse.StatusCode}");
            }

            // Read and delete batch
            var deleteBatch = container.CreateTransactionalBatch(partitionKey);
            deleteBatch.DeleteItem(item1.Id);
            deleteBatch.DeleteItem(item2.Id);
            deleteBatch.DeleteItem(item3.Id);

            var deleteResponse = await deleteBatch.ExecuteAsync();
            Console.WriteLine($"Batch delete completed (RU: {deleteResponse.RequestCharge})");
        }

        private static async Task DemonstrateTransactions(Container container)
        {
            var category = "TransactionTest";
            var partitionKey = new PartitionKey(category);

            var item1 = new Product { Id = Guid.NewGuid().ToString(), Name = "Transaction Item 1", Category = category, Price = 100m };
            var item2 = new Product { Id = Guid.NewGuid().ToString(), Name = "Transaction Item 2", Category = category, Price = 200m };

            await container.CreateItemAsync(item1, partitionKey);

            var transactionalBatch = container.CreateTransactionalBatch(partitionKey)
                .CreateItem(item2)
                .ReplaceItem(item1.Id, new Product { Id = item1.Id, Name = "Updated in Transaction", Category = category, Price = 150m });

            var response = await transactionalBatch.ExecuteAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Transaction succeeded - All operations committed (RU: {response.RequestCharge})");
            }
            else
            {
                Console.WriteLine($"Transaction failed - All operations rolled back: {response.StatusCode}");
            }

            // Cleanup
            await container.DeleteItemAsync<Product>(item1.Id, partitionKey);
            await container.DeleteItemAsync<Product>(item2.Id, partitionKey);
        }

        private static async Task DemonstrateBulkOperations(Container container)
        {
            var cosmosClientOptions = new CosmosClientOptions
            {
                AllowBulkExecution = true,
                MaxRetryAttemptsOnRateLimitedRequests = 9
            };

            // Note: In production, create a new client with bulk enabled
            Console.WriteLine("Creating 100 items using concurrent tasks...");

            var tasks = new List<Task<ItemResponse<Product>>>();
            for (int i = 0; i < 100; i++)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"Bulk Product {i}",
                    Category = "BulkTest",
                    Price = 10m + i,
                    InStock = i % 2 == 0
                };

                tasks.Add(container.CreateItemAsync(product, new PartitionKey(product.Category)));
            }

            var responses = await Task.WhenAll(tasks);
            var totalRU = responses.Sum(r => r.RequestCharge);

            Console.WriteLine($"Created {responses.Length} items (Total RU: {totalRU:F2})");

            // Cleanup - delete all bulk test items
            var deleteQuery = new QueryDefinition("SELECT c.id FROM c WHERE c.category = 'BulkTest'");
            var deleteIterator = container.GetItemQueryIterator<dynamic>(deleteQuery);
            var deleteTasks = new List<Task>();

            while (deleteIterator.HasMoreResults)
            {
                var deleteResponse = await deleteIterator.ReadNextAsync();
                foreach (var item in deleteResponse)
                {
                    deleteTasks.Add(container.DeleteItemAsync<Product>(item.id.ToString(), new PartitionKey("BulkTest")));
                }
            }

            await Task.WhenAll(deleteTasks);
            Console.WriteLine($"Deleted all bulk test items");
        }

        private static async Task DemonstrateChangeFeed(Container container)
        {
            Console.WriteLine("Setting up Change Feed processor...");

            // Insert some test data
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Change Feed Test",
                Category = "Test",
                Price = 50m
            };
            await container.CreateItemAsync(product, new PartitionKey(product.Category));

            // Change Feed example with iterator
            var changeFeedIterator = container.GetChangeFeedIterator<Product>(
                ChangeFeedStartFrom.Beginning(),
                ChangeFeedMode.Incremental);

            Console.WriteLine("Reading Change Feed:");
            int changeCount = 0;

            while (changeFeedIterator.HasMoreResults)
            {
                var response = await changeFeedIterator.ReadNextAsync();

                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    Console.WriteLine("No more changes at this time");
                    break;
                }

                foreach (var item in response)
                {
                    changeCount++;
                    Console.WriteLine($"  Change #{changeCount}: {item.Name} - ${item.Price}");
                }
            }

            // Cleanup
            await container.DeleteItemAsync<Product>(product.Id, new PartitionKey(product.Category));
        }

        private static async Task DemonstrateIndexingPolicy(Database database)
        {
            var containerProperties = new ContainerProperties
            {
                Id = "CustomIndexContainer",
                PartitionKeyPath = "/category",
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent,
                    IncludedPaths =
                    {
                        new IncludedPath { Path = "/name/?" },
                        new IncludedPath { Path = "/price/?" }
                    },
                    ExcludedPaths =
                    {
                        new ExcludedPath { Path = "/description/*" },
                        new ExcludedPath { Path = "/_etag/?" }
                    },
                    CompositeIndexes =
                    {
                        new Collection<CompositePath>
                        {
                            new() { Path = "/category", Order = CompositePathSortOrder.Ascending },
                            new() { Path = "/price", Order = CompositePathSortOrder.Descending }
                        }
                    }
                }
            };

            var containerResponse = await database.CreateContainerIfNotExistsAsync(containerProperties);
            Console.WriteLine($"Container with custom indexing policy: {containerResponse.Container.Id}");

            // Cleanup
            await containerResponse.Container.DeleteContainerAsync();
            Console.WriteLine("Custom index container deleted");
        }

        private static async Task DemonstrateStoredProcedures(Container container)
        {
            var sprocId = "bulkInsert";
            var sprocBody = @"
                function bulkInsert(docs) {
                    var collection = getContext().getCollection();
                    var collectionLink = collection.getSelfLink();
                    var count = 0;

                    if (!docs) throw new Error('The docs parameter is required.');

                    var docsLength = docs.length;
                    if (docsLength == 0) {
                        getContext().getResponse().setBody(0);
                        return;
                    }

                    tryCreate(docs[count], callback);

                    function tryCreate(doc, callback) {
                        var isAccepted = collection.createDocument(collectionLink, doc, callback);
                        if (!isAccepted) getContext().getResponse().setBody(count);
                    }

                    function callback(err, doc, options) {
                        if (err) throw err;
                        count++;
                        if (count >= docsLength) {
                            getContext().getResponse().setBody(count);
                        } else {
                            tryCreate(docs[count], callback);
                        }
                    }
                }";

            try
            {
                var sprocResponse = await container.Scripts.CreateStoredProcedureAsync(
                    new Microsoft.Azure.Cosmos.Scripts.StoredProcedureProperties(sprocId, sprocBody));
                Console.WriteLine($"Stored procedure created: {sprocResponse.Resource.Id}");

                // Execute stored procedure
                var testDocs = new[]
                {
                    new Product { Id = Guid.NewGuid().ToString(), Name = "SP Product 1", Category = "SPTest", Price = 100m },
                    new Product { Id = Guid.NewGuid().ToString(), Name = "SP Product 2", Category = "SPTest", Price = 200m }
                };

                var executeResponse = await container.Scripts.ExecuteStoredProcedureAsync<int>(
                    sprocId,
                    new PartitionKey("SPTest"),
                    new[] { testDocs });

                Console.WriteLine($"Stored procedure executed - Inserted {executeResponse.Resource} documents (RU: {executeResponse.RequestCharge})");

                // Cleanup
                await container.Scripts.DeleteStoredProcedureAsync(sprocId);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                Console.WriteLine("Stored procedure already exists");
            }
        }

        private static async Task CleanupResources(Database database)
        {
            Console.WriteLine("Deleting database...");
            await database.DeleteAsync();
            Console.WriteLine("Database deleted");
        }
    }

    // Model class for Cosmos DB documents
    public class Product
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string? Id { get; set; }

        [Newtonsoft.Json.JsonProperty("name")]
        public string? Name { get; set; }

        [Newtonsoft.Json.JsonProperty("category")]
        public string? Category { get; set; }

        [Newtonsoft.Json.JsonProperty("price")]
        public decimal Price { get; set; }

        [Newtonsoft.Json.JsonProperty("description")]
        public string? Description { get; set; }

        [Newtonsoft.Json.JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        [Newtonsoft.Json.JsonProperty("inStock")]
        public bool InStock { get; set; }

        [Newtonsoft.Json.JsonProperty("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [Newtonsoft.Json.JsonProperty("_etag")]
        public string? ETag { get; set; }
    }
}
