using CosmosDBDemo.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task Novo(Pedido pedido)
        {
            await this._container.CreateItemAsync<Pedido>(pedido, new PartitionKey(pedido.Id));
        }

        public async Task Apagar(string id)
        {
            await this._container.DeleteItemAsync<Pedido>(id, new PartitionKey(id));
        }

        public async Task<Pedido> PorId(string id)
        {
            ItemResponse<Pedido> response = await this._container.ReadItemAsync<Pedido>(id, new PartitionKey(id));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return response.Resource;
        }

        public async Task<IEnumerable<Pedido>> Query(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Pedido>(new QueryDefinition(queryString));
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
            await this._container.UpsertItemAsync<Pedido>(item, new PartitionKey(id));
        }
    }
}
