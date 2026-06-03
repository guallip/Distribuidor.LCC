namespace Integrador.BancaEmpresas.LCC.Dominio.DTO.Distribuidor.Consultas
{
    /// <summary>
    /// DTO que representa el estado de cierre de la línea de crédito de un cliente.
    /// Se utiliza para transferir la información mínima necesaria sobre si
    /// una línea de crédito está cerrada y si el cliente es un distribuidor.
    /// </summary>
    public class LineaCreditoCerradaDto
    {
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Indica si el cliente es un distribuidor.
        /// true si es distribuidor; false en caso contrario.
        /// </summary>
        public bool EsDistribuidor { get; set; }
    }
}
