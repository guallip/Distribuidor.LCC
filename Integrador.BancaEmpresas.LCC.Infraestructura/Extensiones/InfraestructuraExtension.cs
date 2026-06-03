using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Extensiones
{
    public static class InfraestructuraExtension
    {
        /// <summary>
        /// Invocación de servicios
        /// </summary>
        /// <param name="iServiceCollection">iServiceCollection</param>
        /// <param name="iConfiguration">iConfiguration</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1163:Unused parameter.", Justification = "<Pending>")]
        public static IServiceCollection AgregarServiciosInfraestructura(this IServiceCollection iServiceCollection, IConfiguration iConfiguration)
        {
            iServiceCollection.AgregarServicio();

            return iServiceCollection;
        }
    }
}
