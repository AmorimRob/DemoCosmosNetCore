using System;
using System.Collections.Generic;
using System.Linq;
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
                await _cosmosDbService.Novo(pedido);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}