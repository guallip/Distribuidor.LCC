using Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas;
using Riesgos4;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Servicios.WcfPrometeus
{
    /// <summary>
    /// Comunicacion con los WFC de Riesgos4
    /// </summary>
    public partial interface IWcfPrometeusServicio
    {
        /// <summary>
        /// Obtiene las lineas de Credito Cerrada por distribuidor o corporativo.
        /// </summary>
        /// <param name="lineaCreditoCerradaDto">Dto para consultar las lineas de credito cerradas</param>
        /// <param name="struObjeto">struObjeto</param>
        /// <returns>Lineas de creditos cerradas</returns>
        Task<BuscarLccResponse> BuscarLineaCreditoCerrada(LineaCreditoCerradaDto lineaCreditoCerradaDto);
    }
}
