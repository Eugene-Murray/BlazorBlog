using Microsoft.Azure.Cosmos;
using TokenAuthApi.Models;

namespace TokenAuthApi.Services;

public class CosmosUserService
{
    private readonly Container _container;

    public CosmosUserService(CosmosClient client, IConfiguration config)
    {
        _container = client.GetContainer(config["Cosmos:Database"], config["Cosmos:Container"]);
    }

    public async Task<AppUser?> GetUserAsync(string email)
    {
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Email = @email")
            .WithParameter("@email", email);

        var iterator = _container.GetItemQueryIterator<AppUser>(query);
        var results = await iterator.ReadNextAsync();
        return results.FirstOrDefault();
    }

    public async Task CreateUserAsync(AppUser user)
    {
        await _container.CreateItemAsync(user, new PartitionKey(user.Id));
    }
}