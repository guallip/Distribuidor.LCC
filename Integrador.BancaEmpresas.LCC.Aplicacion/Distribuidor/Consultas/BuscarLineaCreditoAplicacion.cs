using System.Collections.ObjectModel;
using System.Xml.Linq;
using Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas;
using Integrador.BancaEmpresas.LCC.Dominio.Modelos.Distribuidor.Consultas;
using Integrador.BancaEmpresas.LCC.Infraestructura.Servicios.WcfPrometeus;
using Riesgos4;
using Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Helpers;

namespace Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Consultas
{
    /// <summary>
    /// Implementa la búsqueda de líneas de crédito cerradas consultando el servicio WCF de Prometeus.
    /// </summary>
    public class BuscarLineaCreditoAplicacion : IBuscarLineaCreditoAplicacion
    {
        private readonly IWcfPrometeusServicio wcfPrometeusServicio;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BuscarLineaCreditoAplicacion"/>.
        /// </summary>
        /// <param name="wcfPrometeusServicio">Servicio WCF para consultar líneas de crédito en Prometeus.</param>
        public BuscarLineaCreditoAplicacion(IWcfPrometeusServicio wcfPrometeusServicio)
        {
            this.wcfPrometeusServicio = wcfPrometeusServicio;
        }

        /// <summary>
        /// Busca las líneas de crédito cerradas llamando al servicio de Prometeus y asignando los resultados al modelo de dominio.
        /// </summary>
        /// <param name="lineaCreditoCerradaDto">Criterios de búsqueda para líneas de crédito cerradas.</param>
        /// <returns>Una colección de líneas de crédito cerradas mapeadas desde la respuesta del servicio WCF.</returns>
        public async Task<Collection<LineaCreditoCerradaModelo>> Handler(LineaCreditoCerradaDto lineaCreditoCerradaDto)
        {
            BuscarLccResponse response = await this.wcfPrometeusServicio.BuscarLineaCreditoCerrada(lineaCreditoCerradaDto);
            XElement diffgram = response.BuscarLccResult?.Nodes?.Skip(1).FirstOrDefault();

            return MapeoVariables.MapLineasCreditoCerradas(diffgram);
        }
    }
}
