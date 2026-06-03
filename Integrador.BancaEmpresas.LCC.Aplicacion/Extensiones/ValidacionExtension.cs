using FluentValidation;

namespace Integrador.BancaEmpresas.LCC.Aplicacion.Extensiones
{
    /// <summary>
    /// Servicio de validación de modelos de entrada.
    /// Use esta clase para validar manualmente un request antes de ejecutar la lógica de negocio.
    /// </summary>
    /// <typeparam name="TRequest">Tipo del modelo de invocación al servicio</typeparam>
    public class ValidacionExtension<TRequest>
    {
        /// <summary>
        /// Colección de validadores registrados para el tipo TRequest.
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> validador;

        /// <summary>
        /// Inicializa la instancia con los validadores inyectados por DI.
        /// </summary>
        /// <param name="validator">Inyección de dependencia para la lista de validadores</param>
        public ValidacionExtension(IEnumerable<IValidator<TRequest>> validator)
        {
            validador = validator;
        }

        /// <summary>
        /// Valida el request y lanza <see cref="ValidationException"/> si se encuentran errores.
        /// </summary>
        /// <param name="request">Modelo a validar</param>
        /// <param name="cancellationToken">Token de cancelación opcional</param>
        /// <returns>Task completada cuando la validación finaliza sin errores</returns>
        /// <exception cref="ValidationException">Excepción con el detalle de la validación de los campos</exception>
        public async Task ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            if (validador != null && validador.Any())
            {
                ValidationContext<TRequest> contexto = new ValidationContext<TRequest>(request);
                FluentValidation.Results.ValidationResult[]? resultadosValidacion = await Task.WhenAll(validador.Select(v => v.ValidateAsync(contexto, cancellationToken)));
                List<FluentValidation.Results.ValidationFailure>? errores = resultadosValidacion.SelectMany(r => r.Errors).Where(f => f != null).ToList();
                if (errores.Count != 0)
                {
                    throw new ValidationException(errores);
                }
            }
        }
    }
}
