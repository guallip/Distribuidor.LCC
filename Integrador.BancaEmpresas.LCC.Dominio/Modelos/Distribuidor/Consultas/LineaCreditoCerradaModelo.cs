namespace Integrador.BancaEmpresas.LCC.Dominio.Modelos.Distribuidor.Consultas
{
    /// <summary>
    /// Modelo que representa los datos de una línea de crédito cerrada asociados a un cliente.
    /// Contiene información de identificación, montos relevantes, fechas del ciclo de vida
    /// y estado de bloqueo o liquidación.
    /// </summary>
    public class LineaCreditoCerradaModelo
    {
        /// <summary>
        /// Número secuencial o índice de la línea de crédito en la consulta.
        /// </summary>
        public int Nro { get; set; }

        /// <summary>
        /// Código u descripción de la operación asociada a la línea de crédito.
        /// </summary>
        public string Operacion { get; set; }

        /// <summary>
        /// Identificador del trámite relacionado con la línea de crédito.
        /// </summary>
        public int Tramite { get; set; }

        /// <summary>
        /// Nombre o identificador del cliente titular de la línea de crédito.
        /// </summary>
        public string Cliente { get; set; }

        /// <summary>
        /// Nombre del beneficiario asociado a la línea de crédito, si aplica.
        /// </summary>
        public string Beneficiario { get; set; }

        /// <summary>
        /// Monto original de la operación o partida de la línea de crédito.
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Monto total considerado para la línea de crédito (puede incluir intereses u otros cargos).
        /// </summary>
        public decimal MontoTotal { get; set; }

        /// <summary>
        /// Límite máximo autorizado para la línea de crédito.
        /// </summary>
        public decimal MontoLimite { get; set; }

        /// <summary>
        /// Fecha de emisión de la línea de crédito o de la operación.
        /// </summary>
        public DateTime? FechaEmision { get; set; }

        /// <summary>
        /// Fecha de expiración o vencimiento de la línea de crédito.
        /// </summary>
        public DateTime? FechaExpiracion { get; set; }

        /// <summary>
        /// Fecha en que la línea de crédito fue liquidada, si procede.
        /// </summary>
        public DateTime? FechaLiquidacion { get; set; }

        /// <summary>
        /// Fecha en que la línea de crédito fue cancelada, si procede.
        /// </summary>
        public DateTime? FechaCancelacion { get; set; }

        /// <summary>
        /// Indica si la línea de crédito está en estado de bloqueo.
        /// true si está bloqueada; false en caso contrario.
        /// </summary>
        public bool Bloqueo { get; set; }

        /// <summary>
        /// Comentario asociado al bloqueo, que explica la causa o detalles relevantes.
        /// </summary>
        public string ComentarioBlq { get; set; }

        /// <summary>
        /// Monto actualmente utilizado de la línea de crédito.
        /// </summary>
        public decimal Usado { get; set; }

        /// <summary>
        /// Estado textual de la línea de crédito (por ejemplo: "Cerrada", "Liquidada", "Cancelada").
        /// </summary>
        public string Estado { get; set; }
    }
}
