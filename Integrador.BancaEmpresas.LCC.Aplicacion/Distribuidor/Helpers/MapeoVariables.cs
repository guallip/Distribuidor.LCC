using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;
using Integrador.BancaEmpresas.LCC.Dominio.Modelos.Distribuidor.Consultas;

namespace Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Helpers
{
    /// <summary>
    /// Clase auxiliar que proporciona métodos para mapear datos XML a modelos de dominio y realizar conversiones de tipos de datos.
    /// </summary>
    public static class MapeoVariables
    {
        /// <summary>
        /// Mapea un elemento XML (diffgram) a una colección de modelos de líneas de crédito cerradas.
        /// </summary>
        /// <param name="diffgram">Elemento XML que contiene la estructura de datos de tablas con información de líneas de crédito.</param>
        /// <returns>Una colección de <see cref="LineaCreditoCerradaModelo"/> con los datos extraídos del XML. Retorna una colección vacía si el diffgram es nulo.</returns>
        public static Collection<LineaCreditoCerradaModelo> MapLineasCreditoCerradas(XElement diffgram)
        {
            Collection<LineaCreditoCerradaModelo> lineas = new Collection<LineaCreditoCerradaModelo>();
            if (diffgram == null)
            {
                return lineas;
            }

            foreach (XElement tabla in diffgram.Descendants().Where(x => x.Name.LocalName == "tabla"))
            {
                LineaCreditoCerradaModelo lineaCredito = new LineaCreditoCerradaModelo();

                lineaCredito.Nro = ParseInt(tabla.Element("Nro."));
                lineaCredito.Operacion = ParseString(tabla.Element("OPERACION"));
                lineaCredito.Tramite = ParseInt(tabla.Element("TRAMITE"));
                lineaCredito.Cliente = ParseString(tabla.Element("CLIENTE"));
                lineaCredito.Beneficiario = ParseString(tabla.Element("BENEFICIARIO"));
                lineaCredito.Monto = ParseDecimal(tabla.Element("MONTO"));
                lineaCredito.MontoTotal = ParseDecimal(tabla.Element("MONTO_x0020_TOTAL"));
                lineaCredito.MontoLimite = ParseDecimal(tabla.Element("MONTO_x0020_LIMITE"));
                lineaCredito.Usado = ParseDecimal(tabla.Element("USADO"));
                lineaCredito.FechaEmision = ParseNullableDateTime(tabla.Element("FECHA_x0020_EMISION"));
                lineaCredito.FechaExpiracion = ParseNullableDateTime(tabla.Element("FECHA_x0020_EXPIRACION"));
                lineaCredito.FechaLiquidacion = ParseNullableDateTime(tabla.Element("FECHA_x0020_LIQUIDACION"));
                lineaCredito.FechaCancelacion = ParseNullableDateTime(tabla.Element("FECHA_x0020_CANCELACION"));
                lineaCredito.Bloqueo = ParseBool(tabla.Element("BLOQUEO"));
                lineaCredito.ComentarioBlq = ParseString(tabla.Element("COMENTARIO_x0020_BLQ"));
                lineaCredito.Estado = ParseString(tabla.Element("ESTADO"));

                lineas.Add(lineaCredito);
            }

            return lineas;
        }

        /// <summary>
        /// Parsea un elemento XML a una cadena de texto, eliminando espacios en blanco al inicio y final.
        /// </summary>
        /// <param name="elemento">Elemento XML a parsear. Puede ser nulo.</param>
        /// <returns>Cadena de texto trimada o una cadena vacía si el elemento es nulo.</returns>
        private static string ParseString(XElement? elemento)
        {
            string valor = elemento?.Value;
            return valor?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Parsea un elemento XML a un valor entero.
        /// </summary>
        /// <param name="elemento">Elemento XML a parsear. Puede ser nulo.</param>
        /// <returns>Valor entero parseado o 0 si el parseo falla o el elemento es nulo.</returns>
        private static int ParseInt(XElement? elemento)
        {
            string valor = elemento?.Value;
            return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : 0;
        }

        /// <summary>
        /// Parsea un elemento XML a un valor decimal.
        /// </summary>
        /// <param name="elemento">Elemento XML a parsear. Puede ser nulo.</param>
        /// <returns>Valor decimal parseado o 0m si el parseo falla o el elemento es nulo.</returns>
        private static decimal ParseDecimal(XElement? elemento)
        {
            string valor = elemento?.Value;
            return decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : 0m;
        }

        /// <summary>
        /// Parsea un elemento XML a un valor booleano.
        /// </summary>
        /// <param name="elemento">Elemento XML a parsear. Puede ser nulo.</param>
        /// <returns>Valor booleano parseado o false si el parseo falla o el elemento es nulo.</returns>
        private static bool ParseBool(XElement? elemento)
        {
            string valor = elemento?.Value;
            return bool.TryParse(valor, out bool result) ? result : false;
        }

        /// <summary>
        /// Parsea un elemento XML a un valor de fecha y hora anulable en formato UTC universal.
        /// </summary>
        /// <param name="elemento">Elemento XML a parsear. Puede ser nulo.</param>
        /// <returns>Valor <see cref="DateTime"/> parseado en UTC o null si el parseo falla o el elemento es nulo.</returns>
        private static DateTime? ParseNullableDateTime(XElement? elemento)
        {
            string valor = elemento?.Value;
            return DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out DateTime result)
                ? result
                : null;
        }
    }
}
