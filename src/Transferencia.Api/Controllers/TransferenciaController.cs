using Transferencia.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Application.Interfaces.Services;
using Transferencia.Application.Dtos.Transferencia;
using Asp.Versioning;
using static System.Net.Mime.MediaTypeNames;

namespace Transferencia.Api.Controllers
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TransferenciaController : ControllerBase
    {
        private readonly ITransferenciaService _transferenciaService;

        public TransferenciaController(ITransferenciaService transferenciaService)
        {
            _transferenciaService = transferenciaService;
        }

        /// <summary>
        /// Retorna todas as transferências.
        /// </summary>
        /// <returns>Lista de transferências.</returns>
        /// <response code="200">Transferências retornadas com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransferenciaDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAllTransferencias()
        {
            var transferencias = await _transferenciaService.GetAllTransferenciasAsync();
            return Ok(transferencias);
        }

        /// <summary>
        /// Retorna uma transferência específica por ID.
        /// </summary>
        /// <param name="id">ID da transferência.</param>
        /// <returns>Detalhes da transferência.</returns>
        /// <response code="200">Transferência encontrada.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Transferência não encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TransferenciaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTransferenciaById(Guid id)
        {
            var transferencia = await _transferenciaService.GetTransferenciaByIdAsync(id);
            return Ok(transferencia);
        }

        /// <summary>
        /// Realiza uma nova transferência.
        /// </summary>
        /// <param name="transferenciaDto">Dados da nova transferência.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="201">Transferência realizada com sucesso.</response>
        /// <response code="400">Dados inválidos ou saldo insuficiente.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RealizarTransferencia([FromBody] CreateTransferenciaDTO createTransferenciaDto)
        {
            var result = await _transferenciaService.RealizarTransferenciaAsync(createTransferenciaDto);
            return CreatedAtAction(nameof(GetTransferenciaById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Cancela uma transferência existente.
        /// </summary>
        /// <param name="id">ID da transferência a ser cancelada.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Transferência cancelada com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Transferência não encontrada.</response>
        [HttpPut("cancelar/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CancelarTransferencia(Guid id)
        {
            var result = await _transferenciaService.CancelarTransferenciaAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Exclui uma transferência existente.
        /// </summary>
        /// <param name="id">ID da transferência a ser excluída.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Transferência excluída com sucesso.</response>
        /// <response code="400">ID inválido.</response>
        /// <response code="404">Transferência não encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTransferencia(Guid id)
        {
            var result = await _transferenciaService.DeleteTransferenciaAsync(id);
            return Ok(result);
            
        }

        /// <summary>
        /// Retorna histórico de transferências por conta.
        /// </summary>
        /// <param name="contaId">Id da conta a ser carregado o histórico</param>
        /// <returns>Histórico de transferências</returns>
        /// <response code="200">Lista de histórico de transferências.</response>
        /// <response code="400">Conta inválida.</response>
        /// <response code="404">Histórico de transferência não encontrada.</response>
        [HttpGet("historico/{contaId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransferenciaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ApiError>), StatusCodes.Status404NotFound)]
        
        public async Task<ActionResult> GetHistoricoTransferencias(Guid contaId)
        {
            var historico = await _transferenciaService.GetHistoricoTransferenciasAsync(contaId);
            return Ok(historico);
        }
    }
}
