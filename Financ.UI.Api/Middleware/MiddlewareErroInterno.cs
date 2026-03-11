using System.Text.Json;

namespace Financ.UI.Api.Middleware
{
    public class MiddlewareErroInterno
    {
        private readonly RequestDelegate _proximo;
        public MiddlewareErroInterno(RequestDelegate proximo) => _proximo = proximo;
        public async Task InvokeAsync(HttpContext contexto)
        {
            try
            {
                await _proximo(contexto);
            }
            catch (Exception ex)
            {
                contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await contexto.Response.WriteAsJsonAsync(new { Mensagem = "Ocorreu um erro interno no servidor.", Detalhes = ex.Message });
            }
        }

        public static Task HadlerExcessao(HttpContext contexto, Exception ex)
        {
            contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
            contexto.Response.ContentType = "application/json";
            
            var resposta = new
            {
                Mensagem = "Ocorreu um erro interno no servidor.",
                Detalhes = ex.Message
            };

            var json = JsonSerializer.Serialize(resposta);
            return contexto.Response.WriteAsync(json);
        }
    }
}
