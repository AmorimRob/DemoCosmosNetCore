using CosmosDBDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBDemo.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Pedido>> Query(string query);
        Task<Pedido> PorId(string id);
        Task Novo(Pedido item);
        Task Atualizar(string id, Pedido item);
        Task Apagar(string id);
    }
}
