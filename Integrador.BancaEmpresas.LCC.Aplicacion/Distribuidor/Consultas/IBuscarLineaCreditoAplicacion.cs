using System.Collections.ObjectModel;
using Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas;
using Integrador.BancaEmpresas.LCC.Dominio.Modelos.Distribuidor.Consultas;

namespace Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Consultas
{
    /// <summary>
    /// Define el contrato para consultar líneas de crédito cerradas en el sistema de distribuidores.
    /// </summary>
    public interface IBuscarLineaCreditoAplicacion
    {
        /// <summary>
        /// Busca las líneas de crédito cerradas basadas en los criterios especificados.
        /// </summary>
        /// <param name="lineaCreditoCerradaDto">Objeto que contiene los criterios de búsqueda para líneas de crédito cerradas.</param>
        /// <returns>Una tarea que devuelve una colección de <see cref="LineaCreditoCerradaModelo"/> con los resultados de la búsqueda.</returns>
        public Task<Collection<LineaCreditoCerradaModelo>> Handler(LineaCreditoCerradaDto lineaCreditoCerradaDto);
    }
}
