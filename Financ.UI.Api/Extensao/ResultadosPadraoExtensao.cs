using Financ.Application.Comun.Resultado;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Financ.UI.Api.Extensao
{
    public static class ResultadosPadraoExtensao
    {
        public static IActionResult RetornoAutomatico<T>(this Resultado<T> resultado, (string rota, string controller,object parametros)? rota = null)
        {
            if (resultado.ValidaSucesso)
                if (rota != null)
                    return new CreatedAtActionResult(rota.Value.rota,null,rota.Value.parametros, resultado.Sucesso);
                else
                    return new OkObjectResult(resultado.Sucesso);


            return resultado.Falha!.Codigo switch
            {
                400 => new BadRequestObjectResult(resultado.Falha.Mensagem),
                404 => new NotFoundObjectResult(resultado.Falha.Mensagem),
                401 => new UnauthorizedObjectResult(resultado.Falha.Mensagem),
                _ => new ObjectResult(resultado.Falha.Mensagem) { StatusCode = 500 }
            };
        }
    }
}
