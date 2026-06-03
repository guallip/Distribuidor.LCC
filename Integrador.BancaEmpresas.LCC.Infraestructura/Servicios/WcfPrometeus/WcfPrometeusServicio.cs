using Integrador.BancaEmpresas.LCC.Dominio.General;
using Microsoft.Extensions.Configuration;
using Riesgos4;
using System.Security.Principal;
using System.ServiceModel;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Servicios.WcfPrometeus
{
    public partial class WcfPrometeusServicio : BaseServicio, IWcfPrometeusServicio
    {
        /// <summary>
        /// Instancia EndpointAddress
        /// </summary>
        private EndpointAddress endpoint;

        /// <summary>
        /// binding
        /// </summary>
        private BasicHttpBinding binding;

        /// <summary>
        /// Llamada al servicio de consultas iConfiguration
        /// </summary>
        private readonly IConfiguration iConfiguration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clienteWeb">clienteWeb</param>
        /// <param name="iConfiguration">iConfiguration</param>
        /// <exception cref="ArgumentNullException"></exception>
        public WcfPrometeusServicio(HttpClient clienteWeb, IConfiguration iConfiguration) : base(clienteWeb)
        {
            this.iConfiguration = iConfiguration ?? throw new ArgumentNullException(nameof(iConfiguration));
            binding = new BasicHttpBinding
            {
                Security = new BasicHttpSecurity
                {
                    Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.Windows
                    }
                }
            };
        }

        /// <summary>
        /// Método para enlace con WCF Cuentas1
        /// </summary>
        /// <typeparam name="TResult">TResult</typeparam>
        /// <param name="funcion">funcion</param>
        /// <returns>Conexion</returns>
        public async Task<TResult> Riesgos4WcfAsync<TResult>(Func<RiesgosSrv4Client, Task<TResult>> funcion)
        {
            endpoint = new EndpointAddress(iConfiguration[Constantes.URL_WCF_RIESGOS4]);
            RiesgosSrv4Client clientRiesgos = new RiesgosSrv4Client(binding, endpoint);

            clientRiesgos.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
            clientRiesgos.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;

            try
            {
                TResult result = await funcion(clientRiesgos);
                clientRiesgos.Close();
                return result;
            }
            catch
            {
                clientRiesgos.Abort();
                throw;
            }
        }
    }
}
