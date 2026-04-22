using Financ.Application.CQRS.Categorias.Command;
using Financ.Application.CQRS.Categorias.Query;
using Financ.Application.DTOs.Categoria.Post;
using Financ.Application.DTOs.Categorias.Patch;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/Contas/{idConta}/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriasController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CriarCategoria(int idConta, CadastraCategoriaDTO categoriaDTO)
        {
            var categoria = await _mediator.Send(new CriaCategoriaCommand(idConta, User.RetornaIdUsuario(), categoriaDTO.Nome, categoriaDTO.cor));
            return categoria.RetornoAutomatico();
        }
        [HttpGet]
        public async Task<IActionResult> RetornaCategorias(int idConta)
        {
            var categoria = await _mediator.Send(new RetornaCategoriasQuery(idConta, User.RetornaIdUsuario()));
            return categoria.RetornoAutomatico();
        }
        [HttpPatch("/api/Contas/Categorias/{idCategoria}/Alterar")]
        public async Task<IActionResult> AlterarCategoria(int idCategoria, [FromBody] AlterarCategoriaDTO categoriaDTO )
        {
            var categoria = await _mediator.Send(new AlterarCategoriaCommand(idCategoria, User.RetornaIdUsuario(),categoriaDTO.Nome,categoriaDTO.Cor));
            return categoria.RetornoAutomatico();
        }
        [HttpDelete("/api/Contas/Categorias/{idCategoria}/Remover")]
        public async Task<IActionResult> RemoverCategoria(int idCategoria)
        {
            var categoria = await _mediator.Send(new RemoverCategoriaCommand(idCategoria, User.RetornaIdUsuario()));
            return categoria.RetornoAutomatico();
        }
    }
}
