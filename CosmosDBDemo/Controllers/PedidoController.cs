using System;
using System.Threading.Tasks;
using CosmosDBDemo.Models;
using CosmosDBDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace CosmosDBDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        public PedidoController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _cosmosDbService.Query("SELECT * FROM Pedidos"));
        }

        [HttpPost("novoPedido")]
        public async Task<ActionResult> NovoPedido(Pedido pedido)
        {
            try
            {
                pedido.Id = Guid.NewGuid().ToString();
                var criouComSucesso = await _cosmosDbService.Novo(pedido);
                if (criouComSucesso)
                    return Ok();

                return Forbid();
            }
            catch (Exception)
            {
                return Forbid();
            }
        }
    }
}