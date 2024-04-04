using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourNamespace
{
    public class CosmosService
    {
        private readonly string _endpointUri;
        private readonly string _primaryKey;
        private readonly string _databaseName;
        private readonly string _containerName;

        private CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        public CosmosService(string endpointUri, string primaryKey, string databaseName, string containerName)
        {
            _endpointUri = endpointUri;
            _primaryKey = primaryKey;
            _databaseName = databaseName;
            _containerName = containerName;
        }

        public async Task InitializeAsync()
        {
            _cosmosClient = new CosmosClient(_endpointUri, _primaryKey);
            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);
            _container = await _database.CreateContainerIfNotExistsAsync(_containerName, "/id");
        }

        public async Task<T> GetItemAsync<T>(string id)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>()
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<T> CreateItemAsync<T>(T item)
        {
            var response = await _container.CreateItemAsync(item);
            return response.Resource;
        }

        public async Task<T> UpdateItemAsync<T>(string id, T item)
        {
            var response = await _container.ReplaceItemAsync(item, id, new PartitionKey(id));
            return response.Resource;
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<object>(id, new PartitionKey(id));
        }
    }
}