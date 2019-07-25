using CosmosDBDemo.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CosmosDBDemo.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<bool> Novo(Pedido pedido)
        {
            var response = await _container.CreateItemAsync<Pedido>(pedido, new PartitionKey(pedido.Id));

            if(response.StatusCode == HttpStatusCode.Created)
                return true;

            return false;

        }

        public async Task Apagar(string id)
        {
            await _container.DeleteItemAsync<Pedido>(id, new PartitionKey(id));
        }

        public async Task<Pedido> PorId(string id)
        {
            ItemResponse<Pedido> response = await _container.ReadItemAsync<Pedido>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.Resource;
        }

        public async Task<IEnumerable<Pedido>> Query(string queryString)
        {
            var query = _container.GetItemQueryIterator<Pedido>(new QueryDefinition(queryString));
            List<Pedido> results = new List<Pedido>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task Atualizar(string id, Pedido item)
        {
            await _container.UpsertItemAsync<Pedido>(item, new PartitionKey(id));
        }
    }
}
