using AirsoftEvents.Persistance.Entities;
using AirsoftEvents.Persistance.Interface;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace AirsoftEvents.Persistance;

public class LogRepo : ILogRepo
{
    private readonly Container _container;

    public LogRepo(IConfiguration config)
    {
        var cs = config["Cosmos:ConnectionString"];
        var db = config["Cosmos:DatabaseName"] ?? "airsoftevents";
        var container = config["Cosmos:ContainerName"] ?? "logs";

        if (string.IsNullOrWhiteSpace(cs))
            throw new Exception("Cosmos connectionstring missing (Cosmos:ConnectionString).");

        var client = new CosmosClient(cs);
        _container = client.GetContainer(db, container);
    }

    public async Task<LogEntry> CreateAsync(LogEntry entry)
    {
        var res = await _container.CreateItemAsync(entry, new PartitionKey(entry.id));
        return res.Resource;
    }

    public async Task<LogEntry?> GetAsync(string id)
    {
        try
        {
            var res = await _container.ReadItemAsync<LogEntry>(id, new PartitionKey(id));
            return res.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
