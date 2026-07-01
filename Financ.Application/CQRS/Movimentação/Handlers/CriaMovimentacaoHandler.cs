using Financ.Application.Comun.Enums;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.Movimentações;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class CriaMovimentacaoHandler : IRequestHandler<CriaMovimentacaoCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidaPermissao _validaPermissao;
        private readonly IExisteContaUsuario _encontraContaUsuario;
        public CriaMovimentacaoHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao, IExisteContaUsuario encontraContaUsuario)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
            _encontraContaUsuario = encontraContaUsuario;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(CriaMovimentacaoCommand request, CancellationToken cancellationToken)
        {

            var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(c => c.Id == request.idConta);

            if (conta is null)
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));
            
            ContaUsuario contasUsuario = await _encontraContaUsuario.Buscar(u => u.IdUsuario == request.idUsuario);
            _validaPermissao.Valiidar(contasUsuario, PermissoesContasUsuarios.CadastrarMovimentacao);

            Movimentacao movimentacao = new Movimentacao(request.tipo, contasUsuario, request.valor, request.titulo, request.observacao, request.dthrMovimentacao, request.dthrConclusao, request.concluido);


            if (request.IdsCategoria is not null)
            {
                foreach (var idCategoria in request.IdsCategoria)
                {
                    Categoria? categoria = await _unitOfWork.categoriaRepositorio.BuscarObjetoUnico(c => c.Id == idCategoria && c.IdConta == request.idConta);
                    if (categoria is null)
                        return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado($"Categoria com id {idCategoria} não encontrada"));

                    MovimentacaoCategoria movimentacaoCategoria = new MovimentacaoCategoria(movimentacao, categoria);
                    movimentacao.AdicionarCategoria(movimentacaoCategoria);
                }
            }

            conta.ProcessaMovimentacao(movimentacao);
            await _unitOfWork.movimentacaoRepositorio.Adicionar(movimentacao);
            _unitOfWork.contasRepositorio.Atualiza(conta);
            await _unitOfWork.Commit();

            return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));

        }
    }
}
