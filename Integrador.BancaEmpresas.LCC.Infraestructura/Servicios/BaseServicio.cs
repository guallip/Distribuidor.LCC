using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Produnet.BFF.Modelo.DTO;
using Produnet.BFF.Utilitario.Implementaciones;

namespace Integrador.BancaEmpresas.LCC.Infraestructura.Servicios
{
    /// <summary>
    /// Servicio base
    /// </summary>
    public partial class BaseServicio
    {
        readonly HttpClient clienteHttp;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clienteWeb">Objeto usado para las peticiones http</param>
        /// <exception cref="ArgumentNullException">Excepciones no controladas del cliente http</exception>
        public BaseServicio(HttpClient clienteWeb)
        {
            this.clienteHttp = clienteWeb ?? throw new ArgumentNullException(nameof(clienteWeb));
        }

        /// <summary>
        ///   Obtener objeto de la respuesta http con un nombre de contenedor
        /// </summary>
        /// <typeparam name="Salida"></typeparam>
        /// <typeparam name="Consulta"></typeparam>
        /// <param name="mensajeRespuestaHttp"></param>
        /// <param name="contenedor"></param>
        /// <returns>Objeto de consulta</returns>
        /// <exception cref="InvalidDataException"></exception>
        static async Task<Tuple<Salida, Consulta>> ObtenerObjetoApi<Salida, Consulta>(HttpResponseMessage mensajeRespuestaHttp, string contenedor)
        {
            string respuesta = await mensajeRespuestaHttp.Content.ReadAsStringAsync();
            JsonNode? consulta = respuesta.First().ToString().Equals("<", StringComparison.CurrentCultureIgnoreCase) ? null : JsonNode.Parse(respuesta);
            if (mensajeRespuestaHttp.IsSuccessStatusCode && consulta != null)
            {
                if (!string.IsNullOrEmpty(contenedor))
                {
                    return Tuple.Create(JsonSerializer.Deserialize<Salida>(consulta.ToJsonString()), JsonSerializer.Deserialize<Consulta>(consulta[contenedor]));
                }
                else
                {
                    return Tuple.Create(JsonSerializer.Deserialize<Salida>(consulta.ToJsonString()), JsonSerializer.Deserialize<Consulta>(consulta.ToJsonString()));
                }
            }
            else
            {
                string mensaje = string.IsNullOrEmpty(consulta?.ToString()) ? mensajeRespuestaHttp.ReasonPhrase : consulta.ToString();
                throw new Exception(mensaje) { HResult = ObtenerValor(mensajeRespuestaHttp.StatusCode) };
            }
        }

        /// <summary>
        /// Obtener objeto de la respuesta http
        /// </summary>
        /// <typeparam name="Salida">Tipo de datos para los parametros de entrada</typeparam>
        /// <param name="mensajeRespuestaHttp">Mensaje Respuesta Http</param>
        /// <returns>Objeto de consulta</returns>
        /// <exception cref="NotImplementedException">Excepciones no controladas</exception>
        static async Task<Salida> ObtenerObjetoApi<Salida>(HttpResponseMessage mensajeRespuestaHttp)
        {
            string respuesta = await mensajeRespuestaHttp.Content.ReadAsStringAsync();
            JsonNode? consulta = respuesta.First().ToString().Equals("<", StringComparison.CurrentCultureIgnoreCase) ? null : JsonNode.Parse(respuesta);
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            if (mensajeRespuestaHttp.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<Salida>(consulta.ToJsonString(options));
            }
            else
            {
                string mensaje = string.IsNullOrEmpty(consulta?.ToString()) ? mensajeRespuestaHttp.ReasonPhrase : consulta.ToString();
                throw new Exception(mensaje) { HResult = ObtenerValor(mensajeRespuestaHttp.StatusCode) };
            }
        }

        /// <summary>
        /// Obtener objeto de la respuesta http con un nombre de contenedor
        /// </summary>
        /// <typeparam name="Salida"></typeparam>
        /// <param name="mensajeRespuestaHttp"></param>
        /// <param name="contenedor"></param>
        /// <returns>Objeto de consulta</returns>
        /// <exception cref="InvalidDataException"></exception>
        static async Task<Salida> ObtenerObjetoApi<Salida>(HttpResponseMessage mensajeRespuestaHttp, string contenedor)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
            if (mensajeRespuestaHttp.IsSuccessStatusCode)
            {
                JsonNode consulta = JsonNode.Parse(await mensajeRespuestaHttp.Content.ReadAsStringAsync());
                if (!string.IsNullOrEmpty(contenedor))
                {
                    return JsonSerializer.Deserialize<Salida>(consulta[contenedor], options);
                }
                else
                {
                    return JsonSerializer.Deserialize<Salida>(consulta.ToJsonString(), options);
                }
            }
            else
            {
                throw new Exception(mensajeRespuestaHttp.ReasonPhrase) { HResult = ObtenerValor(mensajeRespuestaHttp.StatusCode) };
            }
        }

        /// <summary>
        /// Obtener el valor int de Status Code
        /// </summary>
        /// <param name="codigo">Estado de ejecucion</param>
        /// <returns>Estado valor int</returns>
        public static int ObtenerValor(HttpStatusCode codigo)
        {
            return (int)codigo;
        }

        /// <summary>
        /// Obtener objeto
        /// </summary>
        /// <param name="httpResponseMessage">Respuesta HTTP mensaje</param>
        /// <exception cref="InvalidDataException">Control de excepciones</exception>
        static async Task ObtenerObjeto(HttpResponseMessage httpResponseMessage)
        {
            RespuestaDto respuestaDto = await ConversionRespuesta.ConvertirAProdunet<RespuestaDto>(httpResponseMessage);

            if (!respuestaDto.EsExitoso)
            {
                throw new InvalidDataException(respuestaDto.Mensaje) { HResult = respuestaDto.Codigo };
            }
        }

        /// <summary>
        /// Obtener contenedor de objetos para consulta
        /// </summary>
        /// <typeparam name="Salida">Tipo de datos para los parametros de entrada</typeparam>
        /// <param name="mensajeRespuestaHttp">objeto Respuesta Http</param>
        /// <returns>Contenedor de objetos</returns>
        /// <exception cref="NotImplementedException">Excepciones no controladas</exception>
        static async Task<Salida> ObtenerContenedor<Salida>(HttpResponseMessage mensajeRespuestaHttp)
        {
            RespuestaDto respuestaDto = await ConversionRespuesta.ConvertirAProdunet<RespuestaDto>(mensajeRespuestaHttp);

            if (respuestaDto.EsExitoso)
            {
                return await ConversionRespuesta.ConvertirAContenedor<Salida>(respuestaDto.Datos.ToString());
            }
            else
            {
                throw new InvalidDataException(respuestaDto.Mensaje) { HResult = respuestaDto.Codigo };
            }
        }

        /// <summary>
        /// Preparar peticion http
        /// </summary>
        /// <param name="metodoHttp">Metodo Http</param>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Mensaje peticion Http</returns>
        HttpRequestMessage PrepararPeticion(HttpMethod metodoHttp, string recurso, object objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = new HttpRequestMessage(metodoHttp, UriUtilitario.ObtenerUri(clienteHttp.BaseAddress, recurso));

            if (!string.IsNullOrEmpty(canal))
            {
                clienteHttp.DefaultRequestHeaders.Clear();
                mensajePeticionHttp.Headers.Add("Accept", MediaTypeNames.Application.Json);
                mensajePeticionHttp.Headers.Add("Canal", canal);
            }

            if (cabeceras != null)
            {
                foreach (KeyValuePair<string, string> item in cabeceras)
                {
                    mensajePeticionHttp.Headers.Add(item.Key, item.Value);
                }
            }

            if (objeto != null)
            {
                mensajePeticionHttp.Content = new StringContent(JsonSerializer.Serialize(objeto), Encoding.UTF8, MediaTypeNames.Application.Json);
            }

            return mensajePeticionHttp;
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="contendor">Canal en el que se peticiona</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Tuple<Salida, Consulta>> GetServicioCoreObjeto<Salida, Consulta>(string recurso, string contendor)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, null, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida, Consulta>(mensajeRespuestaHttp, contendor);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioCoreObjeto<Salida>(string recurso)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, null, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Post para un contenedor de objetos
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioCoreContenedor<Salida>(string recurso, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerContenedor<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="contenedor">Nombre del contenedor</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioCoreObjeto<Salida>(string recurso, string contenedor)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, null, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp, contenedor);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="contenedor">Nombre del contenedor</param>
        /// <param name="canal">canal</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioCoreObjeto<Salida>(string recurso, string contenedor, string canal)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, canal, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp, contenedor);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <typeparam name="Salida">Tipo de datos para los parametros de salida</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> PostServicioCoreObjeto<Entrada, Salida>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task PostServicioCoreObjeto<Entrada>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            await ObtenerObjeto(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Post para un contenedor de objetos
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <typeparam name="Salida">Tipo de datos para los parametros de salida</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> PostServicioCoreContenedor<Entrada, Salida>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerContenedor<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Put servicio core objeto
        /// </summary>
        /// <typeparam name="Entrada">Tipo de objeto entrada</typeparam>
        /// <param name="recurso">Recurso</param>
        /// <param name="objeto">Objeto que conformara el cuerpo de la peticion</param>
        /// <param name="canal">Canal</param>
        /// <param name="cabeceras">Cabeceras</param>
        public async Task PutServicioCoreObjeto<Entrada>(string recurso, Entrada objeto, string canal = null, Dictionary<string, string> cabeceras = null)
        {
            HttpRequestMessage httpRequestMessage = PrepararPeticion(HttpMethod.Put, recurso, objeto, canal, cabeceras);
            HttpResponseMessage httpResponseMessage = await clienteHttp.SendAsync(httpRequestMessage);
            await ObtenerObjeto(httpResponseMessage);
        }

        /// <summary>
        /// Peticion Put para un contenedor de objetos
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <typeparam name="Salida">Tipo de datos para los parametros de salida</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> PutServicioCoreContenedor<Entrada, Salida>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Put, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Get servicio core objeto
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<string> GetServicioCoreObjeto(string recurso)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, null, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerBase64(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Delete para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> DeleteServicioCoreObjeto<Salida>(string recurso)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Delete, recurso, null, null, null);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Obtener respuesta en base64
        /// </summary>
        /// <param name="mensajeRespuestaHttp">Mensaje Respuesta Http</param>
        /// <returns>Base64</returns>
        /// <exception cref="NotImplementedException">Excepciones no controladas</exception>
        public static async Task<string> ObtenerBase64(HttpResponseMessage mensajeRespuestaHttp)
        {
            if (mensajeRespuestaHttp.IsSuccessStatusCode)
            {
                byte[] fileBytes = await mensajeRespuestaHttp.Content.ReadAsByteArrayAsync();
                return Convert.ToBase64String(fileBytes);
            }
            else
            {
                throw new Exception(mensajeRespuestaHttp.ReasonPhrase) { HResult = ObtenerValor(mensajeRespuestaHttp.StatusCode) };
            }
        }

        /// <summary>
        /// Get servicio core objeto response
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public Task<HttpResponseMessage> GetServicioCoreObjetoFormatoSalida(string recurso)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, null, null);
            return clienteHttp.SendAsync(mensajePeticionHttp);
        }

        /// <summary>
        /// Post servicio core objeto response
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>       
        /// <returns>Objeto respuesta de la peticion http</returns>
        public Task<HttpResponseMessage> PostServicioCoreObjeto<Entrada>(string recurso, Entrada objeto)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, null, null);
            return clienteHttp.SendAsync(mensajePeticionHttp);
        }

        /// <summary>
        /// Put servicio core objeto response
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>       
        /// <returns>Objeto respuesta de la peticion http</returns>
        public Task<HttpResponseMessage> PutServicioCoreObjeto<Entrada>(string recurso, Entrada objeto)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Put, recurso, objeto, null, null);
            return clienteHttp.SendAsync(mensajePeticionHttp);
        }

        /// <summary>
        /// Delete servicio core objeto response
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public Task<HttpResponseMessage> DeleteServicioCoreObjeto(string recurso)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Delete, recurso, null, null, null);
            return clienteHttp.SendAsync(mensajePeticionHttp);
        }

        /// <summary>
        /// Peticion Post para un contenedor de objetos
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <typeparam name="Salida">Tipo de datos para los parametros de salida</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="contenedor"></param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> PostServicioCoreObjeto<Entrada, Salida>(string recurso, Entrada objeto, string contenedor, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp, contenedor);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="canal">Direccion del servicio a consumir</param>
        /// <param name="cabeceras">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioBffObjeto<Salida>(string recurso, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApiCore<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <typeparam name="Entrada">Tipo de datos para los parametros de entrada</typeparam>
        /// <typeparam name="Salida">Tipo de datos para los parametros de salida</typeparam>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="objeto">Contenido de la peticion</param>
        /// <param name="canal">Canal en el que se peticiona</param>
        /// <param name="cabeceras">Cabeceras de la peticion</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> PostServicioBffObjeto<Entrada, Salida>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Post, recurso, objeto, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApiCore<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Put servicio core objeto
        /// </summary>
        /// <typeparam name="Entrada">Tipo de objeto entrada</typeparam>
        /// <param name="recurso">Recurso</param>
        /// <param name="objeto">Objeto que conformara el cuerpo de la peticion</param>
        /// <param name="canal">Canal</param>
        /// <param name="cabeceras">Cabeceras</param>
        public async Task<Salida> PutServicioBffObjeto<Entrada, Salida>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage httpRequestMessage = PrepararPeticion(HttpMethod.Put, recurso, objeto, canal, cabeceras);
            HttpResponseMessage httpResponseMessage = await clienteHttp.SendAsync(httpRequestMessage);
            return await ObtenerObjetoApiCore<Salida>(httpResponseMessage);
        }

        /// <summary>
        /// Put servicio core objeto
        /// </summary>
        /// <typeparam name="Entrada">Tipo de objeto entrada</typeparam>
        /// <param name="recurso">Recurso</param>
        /// <param name="objeto">Objeto que conformara el cuerpo de la peticion</param>
        /// <param name="canal">Canal</param>
        /// <param name="cabeceras">Cabeceras</param>
        public async Task PutServicioBffObjeto<Entrada>(string recurso, Entrada objeto, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage httpRequestMessage = PrepararPeticion(HttpMethod.Put, recurso, objeto, canal, cabeceras);
            HttpResponseMessage httpResponseMessage = await clienteHttp.SendAsync(httpRequestMessage);
            await ObtenerObjetoApi(httpResponseMessage);
        }

        /// <summary>
        /// Peticion Post para un objeto unico
        /// </summary>
        /// <param name="recurso">Direccion del servicio a consumir</param>
        /// <param name="canal">Direccion del servicio a consumir</param>
        /// <param name="cabeceras">Direccion del servicio a consumir</param>
        /// <returns>Objeto respuesta de la peticion http</returns>
        public async Task<Salida> GetServicioObjetoCore<Salida>(string recurso, string canal, Dictionary<string, string> cabeceras)
        {
            HttpRequestMessage mensajePeticionHttp = PrepararPeticion(HttpMethod.Get, recurso, null, canal, cabeceras);
            HttpResponseMessage mensajeRespuestaHttp = await clienteHttp.SendAsync(mensajePeticionHttp);
            return await ObtenerObjetoApi<Salida>(mensajeRespuestaHttp);
        }

        /// <summary>
        /// Procesa una respuesta HTTP y deserializa el nodo "datos" como objeto del tipo
        /// Si la respuesta no es exitosa o el contenido no es válido, lanza una excepción con detalles del error.
        /// </summary>
        /// <typeparam name="Salida">Tipo del objeto a retornar desde el JSON.</typeparam>
        /// <param name="mensajeRespuestaHttp">Objeto HttpResponseMessage recibido del servicio.</param>
        /// <returns>Objeto deserializado del tipo <Salida>.</returns>
        static async Task<Salida> ObtenerObjetoApiCore<Salida>(HttpResponseMessage mensajeRespuestaHttp)
        {
            // Leer el cuerpo de la respuesta como texto
            string respuesta = await mensajeRespuestaHttp.Content.ReadAsStringAsync();

            // Verificar si el contenido parece ser HTML (error en servidor, por ejemplo)
            bool esHtml = respuesta.TrimStart().StartsWith("<", StringComparison.OrdinalIgnoreCase);

            // Si es HTML, no se puede procesar como JSON → consulta nula
            JsonNode? consulta = esHtml ? null : JsonNode.Parse(respuesta);

            JsonNode? esExitosoNodo = consulta?["EsExitoso"];

            if (JsonSerializer.Deserialize<bool>(esExitosoNodo)!)
            {
                // Extraer solo el nodo "datos"
                JsonNode? datosNodo = consulta?["Datos"];

                // Verificar que el nodo exista antes de deserializar
                if (datosNodo is not null)
                {
                    return JsonSerializer.Deserialize<Salida>(datosNodo.ToJsonString(), options: new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
                }

                // Si no existe el nodo "datos", lanzar excepción informativa
                throw new Exception("La respuesta fue exitosa, pero no se encontró el nodo 'datos'.");
            }
            else
            {
                JsonNode? mensajeNodo = consulta?["Mensaje"];
                JsonNode? codigoNodo = consulta?["Codigo"];
                // Lanzar excepción con mensaje y código HResult personalizado
                throw new Exception(JsonSerializer.Deserialize<string>(mensajeNodo.ToJsonString()))
                {
                    HResult = JsonSerializer.Deserialize<int>(codigoNodo.ToJsonString())
                };
            }
        }

        /// <summary>
        /// Procesa una respuesta HTTP y deserializa el nodo "datos" como objeto del tipo
        /// Si la respuesta no es exitosa o el contenido no es válido, lanza una excepción con detalles del error.
        /// </summary>
        /// <param name="mensajeRespuestaHttp">Objeto HttpResponseMessage recibido del servicio.</param>
        /// <returns>Objeto deserializado del tipo <Salida>.</returns>
        static async Task ObtenerObjetoApi(HttpResponseMessage mensajeRespuestaHttp)
        {
            // Leer el cuerpo de la respuesta como texto
            string respuesta = await mensajeRespuestaHttp.Content.ReadAsStringAsync();

            // Verificar si el contenido parece ser HTML (error en servidor, por ejemplo)
            bool esHtml = respuesta.TrimStart().StartsWith("<", StringComparison.OrdinalIgnoreCase);

            // Si es HTML, no se puede procesar como JSON → consulta nula
            JsonNode? consulta = esHtml ? null : JsonNode.Parse(respuesta);

            JsonNode? esExitosoNodo = consulta?["EsExitoso"];

            if (!JsonSerializer.Deserialize<bool>(esExitosoNodo)!)
            {
                JsonNode? mensajeNodo = consulta?["Mensaje"];
                JsonNode? codigoNodo = consulta?["Codigo"];
                // Lanzar excepción con mensaje y código HResult personalizado
                throw new Exception(JsonSerializer.Deserialize<string>(mensajeNodo.ToJsonString()))
                {
                    HResult = JsonSerializer.Deserialize<int>(codigoNodo.ToJsonString())
                };
            }
        }
    }
}
