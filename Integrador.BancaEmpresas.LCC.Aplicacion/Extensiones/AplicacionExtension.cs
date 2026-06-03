using System.Reflection;
using FluentValidation;
using Integrador.BancaEmpresas.LCC.Aplicacion.Distribuidor.Consultas;
using Microsoft.Extensions.DependencyInjection;

namespace Integrador.BancaEmpresas.LCC.Aplicacion.Extensiones
{
    /// <summary>
    /// Ejecuta la inyeccion de dependencia de librerías de terceros
    /// </summary>
    public static class AplicacionExtension
    {
        /// <summary>
        /// Carga la libreria para las diferentes aplicaciones de terceros
        /// </summary>
        /// <param name="iServicioColeccion">Interfaz de servicios registrados</param>
        /// <returns>Interfaz con servicios de terceros inyectados</returns>
        public static IServiceCollection AgregarServiciosAplicacion(this IServiceCollection iServicioColeccion)
        {
            // Inyeccion de dependencias
            iServicioColeccion.AddScoped<IBuscarLineaCreditoAplicacion, BuscarLineaCreditoAplicacion>();

            // FluentValidation
            iServicioColeccion.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Registro del servicio de validación genérico
            iServicioColeccion.AddScoped(typeof(ValidacionExtension<>));

            return iServicioColeccion;
        }
    }
}
