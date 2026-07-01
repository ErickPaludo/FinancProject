using Financ.Application.Exceções;
using Financ.Domain.Validacoes.Categorias;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Cor;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using Financ.Domain.Validacoes.Segurança;
using Financ.Domain.Validacoes.Usuarios;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Financ.UI.Api.Excessao
{
    public class ExcessaoGlobal : IExceptionHandler
    {
        private readonly ILogger<ExcessaoGlobal> _logger;

        public ExcessaoGlobal(ILogger<ExcessaoGlobal> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, titulo) = exception switch
            {
                AutenticacaoValidacao => (400, exception.Message),
                UsuariosValidacao => (400, exception.Message),
                ContasValidacao => (400, exception.Message),
                ContasUsuariosValidacao => (400, exception.Message),
                MovimentacaoValidacao => (400, exception.Message),
                MovimentacaoFixaValidacao => (400, exception.Message),
                CategoriaValidacao => (400, exception.Message),
                CorValidacao => (400, exception.Message),
                ConvitesValidacao => (400, exception.Message),
                ExceptionPermissoes => (401, exception.Message),
                ExceptionNaoEncontrado => (404, exception.Message),
                KeyNotFoundException =>
                    (404, "Recurso não encontrado"),

                InvalidOperationException =>
                    (409, "Conflito de operação"),

                ArgumentException =>
                    (400, "Requisição inválida"),

                _ =>
                    (500, "Erro interno do servidor")
            };

            if (status == 500)
            {
                _logger.LogError(
                    exception,
                    "Erro inesperado: {Mensagem}",
                    exception.Message);
            }
            else
            {
                _logger.LogWarning(
                    "Exceção tratada [{Status}]: {Mensagem}",
                    status,
                    exception.Message);
            }

            // Monta a resposta ProblemDetails
            var problem = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = status;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}
