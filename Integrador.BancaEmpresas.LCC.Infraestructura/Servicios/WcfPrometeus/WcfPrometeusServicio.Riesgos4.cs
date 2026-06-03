using Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas;
using Riesgos4;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Servicios.WcfPrometeus
{
    public partial class WcfPrometeusServicio
    {
        /// <summary>
        /// Obtiene las lineas de Credito Cerrada por distribuidor o corporativo.
        /// </summary>
        /// <param name="lineaCreditoCerradaDto">Dto para consultar las lineas de credito cerradas</param>
        /// <returns>Lineas de creditos cerradas</returns>
        public Task<BuscarLccResponse> BuscarLineaCreditoCerrada(LineaCreditoCerradaDto lineaCreditoCerradaDto)
        {
            return Riesgos4WcfAsync(client =>
                client.BuscarLccAsync(lineaCreditoCerradaDto.IdCliente, lineaCreditoCerradaDto.EsDistribuidor));
        }
    }
}
