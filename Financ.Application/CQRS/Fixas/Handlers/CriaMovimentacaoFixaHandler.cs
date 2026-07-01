using Financ.Application.Comun.Enums;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class CriaMovimentacaoFixaHandler : IRequestHandler<CriaMovimentacaoFixaCommand, Resultado<BasePost<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidaPermissao _validaPermissao;

        public CriaMovimentacaoFixaHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
        }
        public async Task<Resultado<BasePost<string>>> Handle(CriaMovimentacaoFixaCommand request, CancellationToken cancellationToken)
        {
            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(x => x.IdUsuario == request.idUsuario && x.IdConta == request.idConta);
            if (contaUsuario is null)
                return Resultado<BasePost<string>>.GeraFalha(Falha.NaoEncontrado($"Usuário não pertence a conta!"));

            _validaPermissao.Valiidar(contaUsuario, PermissoesContasUsuarios.CadastrarMovimentacaoFixa);
            Movimentacao movimentacao = new Movimentacao(request.tipo, contaUsuario, request.valor, request.titulo, request.observacao, null, null, false);

            MovimentacaoFixa fixa = new MovimentacaoFixa(request.TipoFixo, request.DataInicio, request.DataFim, request.DataOcorrencia, movimentacao);

            if (request.IdsCategoria is not null)
            {
                foreach (var idCategoria in request.IdsCategoria)
                {
                    Categoria? categoria = await _unitOfWork.categoriaRepositorio.BuscarObjetoUnico(c => c.Id == idCategoria && c.IdConta == request.idConta);
                    if (categoria is null)
                        return Resultado<BasePost<string>>.GeraFalha(Falha.NaoEncontrado($"Categoria com id {idCategoria} não encontrada"));

                    MovimentacaoCategoria movimentacaoCategoria = new MovimentacaoCategoria(movimentacao, categoria);
                    movimentacao.AdicionarCategoria(movimentacaoCategoria);
                }
            }

            await _unitOfWork.movimentacaoFixaRepositorio.Adicionar(fixa);
            await _unitOfWork.Commit();
            return Resultado<BasePost<string>>.GeraSucesso(new BasePost<string>("Movimentação fixa gerada com sucesso"));
        }
    }
}
