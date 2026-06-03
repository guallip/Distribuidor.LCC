using Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Consultas;
using Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas;
using Microsoft.AspNetCore.Mvc;
using Produnet.BFF.Modelo.DTO;
using Produnet.BFF.Modelo.Utilitarios;

namespace Integrador.BancaEmpresas.LCC.API.Controllers.Distribuidor
{
    /// <summary>
    /// Controlador para la búsqueda de líneas de crédito cerradas
    /// </summary>
    [Route("Distribuidor/[controller]")]
    [ApiController]
    public class LineasCreditoController : ControllerBase
    {
        /// <summary>
        /// Objeto de proceso para registro de logs de auditoria
        /// </summary>
        private readonly ILogger<LineasCreditoController> auditoria;

        /// <summary>
        /// Interfaz de aplicacion para la busqueda de lineas de credito cerradas, implementada en la capa de aplicacion
        /// </summary>
        private readonly IBuscarLineaCreditoAplicacion buscarLineaCreditoAplicacion;

        /// <summary>
        /// Modelo de respuesta del proceso
        /// </summary>
        protected RespuestaDto respuestaDto;

        /// <summary>
        /// Constructor del controlador, inyecta las dependencias necesarias para la ejecución de los procesos
        /// </summary>
        /// <param name="auditoria">Objeto de proceso para registro de logs de auditoria</param>
        /// <param name="buscarLineaCreditoAplicacion">Objeto que implementa la aplicacion de lineas de credito</param>
        /// <exception cref="ArgumentNullException">Excepción para la inicialización de objetos</exception>
        public LineasCreditoController(
            ILogger<LineasCreditoController> auditoria,
            IBuscarLineaCreditoAplicacion buscarLineaCreditoAplicacion)
        {
            this.auditoria = auditoria ?? throw new ArgumentNullException(nameof(auditoria));
            respuestaDto = new RespuestaDto();
            this.buscarLineaCreditoAplicacion = buscarLineaCreditoAplicacion ?? throw new ArgumentNullException(nameof(buscarLineaCreditoAplicacion));
        }

        /// <summary>
        /// Busca una línea de crédito cerrada por los criterios del cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="esDistribuidor"></param>
        [HttpGet("BuscarLineasCreditoCerrada/IdCliente/{idCliente}/EsDistribuidor/{esDistribuidor}")]
        public async Task<IActionResult> BuscarLineaCreditoCerrada(int idCliente, bool esDistribuidor)
        {
            LineaCreditoCerradaDto lineaCreditoCerradaDto = new LineaCreditoCerradaDto();
            lineaCreditoCerradaDto.IdCliente = idCliente;
            lineaCreditoCerradaDto.EsDistribuidor = esDistribuidor;

            respuestaDto = await RespuestaUtilitario.GenerarRespuestaDto(auditoria, () =>
                buscarLineaCreditoAplicacion.Handler(lineaCreditoCerradaDto)
            );

            return Ok(respuestaDto);
        }
    }
}
