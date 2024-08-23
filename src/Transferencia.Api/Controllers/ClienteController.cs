using Transferencia.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Application.Exceptions;
using Transferencia.Application.Dtos.Cliente;
using Transferencia.Application.Interfaces.Services;
using Asp.Versioning;

namespace Transferencia.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Retorna todos os clientes.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        /// <response code="200">Clientes retornados com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClienteDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes); 
        }

        /// <summary>
        /// Retorna um cliente específico por ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>Detalhes do cliente.</returns>
        /// <response code="200">Cliente encontrado.</response>
        /// <response code="400">Id do cliente inválido.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetClienteById(Guid id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            return Ok(cliente); 
        }

        /// <summary>
        /// Retorna um cliente específico por Número da Conta.
        /// </summary>
        /// <param name="numeroConta">Número da conta do cliente.</param>
        /// <returns>Detalhes do cliente.</returns>
        /// <response code="200">Cliente encontrado.</response>
        /// <response code="400">Id do cliente inválido.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpGet("por-conta/{numeroConta}")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetClienteByNumeroConta(string numeroConta)
        {
            var cliente = await _clienteService.GetClienteByNumeroContaAsync(numeroConta);
            return Ok(cliente); 
        }

        /// <summary>
        /// Adiciona um novo cliente.
        /// </summary>
        /// <param name="clienteDto">Dados do novo cliente.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="201">Cliente adicionado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ClienteDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddCliente([FromBody] CreateClienteDTO createClienteDto)
        {
            var cliente = await _clienteService.AddClienteAsync(createClienteDto);
            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.Id }, cliente);

        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado.</param>
        /// <param name="clienteDto">Dados atualizados do cliente.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Cliente atualizado com sucesso.</response>
        /// <response code="400">ID do cliente não corresponde ao ID no corpo da requisição.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCliente(Guid id, [FromBody] UpdateClienteDTO updateClienteDto)
        {
            if (id != updateClienteDto.Id)
            {
                throw new CustomException("ID do cliente não corresponde ao ID no corpo da requisição.", StatusCodes.Status400BadRequest) ;
            }

            var result = await _clienteService.UpdateClienteAsync(id, updateClienteDto);
            return Ok(result);
        }

        /// <summary>
        /// Remove um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente a ser removido.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Cliente removido com sucesso.</response>
        /// <response code="400">Id do cliente inválido.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCliente(Guid id)
        {
            var result = await _clienteService.DeleteClienteAsync(id);
            return Ok(result); 
        }
    }
}
