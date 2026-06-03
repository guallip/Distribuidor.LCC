using Integrador.BancaEmpresas.LCC.Infraestructura.Servicios.WcfPrometeus;
using Microsoft.Extensions.DependencyInjection;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Extensiones
{
    public static class ServicioExtension
    {
        public static void AgregarServicio(this IServiceCollection iServicioColeccion)
        {
            iServicioColeccion.AddHttpClient<IWcfPrometeusServicio, WcfPrometeusServicio>();
        }
    }
}
