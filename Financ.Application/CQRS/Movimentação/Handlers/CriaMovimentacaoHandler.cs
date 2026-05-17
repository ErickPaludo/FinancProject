using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
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
        public CriaMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(CriaMovimentacaoCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(c => c.Id == request.idConta);

                if (conta is null)
                    return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

                ContaUsuario? contasUsuario = conta.ContaUsuarios.FirstOrDefault(u => u.IdUsuario == request.idUsuario);

                Movimentacao movimentacao = new Movimentacao(request.tipo, contasUsuario, request.valor, request.titulo, request.observacao, request.dthrMovimentacao, request.dthrConclusao, request.concluido);

                await _unitOfWork.movimentacaoRepositorio.Adicionar(movimentacao);

                if (request.IdsCategoria is not null)
                {
                    foreach (var idCategoria in request.IdsCategoria)
                    {
                        Categoria? categoria = await _unitOfWork.categoriaRepositorio.BuscarObjetoUnico(c => c.Id == idCategoria && c.IdConta == request.idConta);
                        if (categoria is null)
                            return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado($"Categoria com id {idCategoria} não encontrada"));

                        MovimentacaoCategoria movimentacaoCategoria = new MovimentacaoCategoria(movimentacao, categoria);
                        movimentacao.AdicionarCategoria(movimentacaoCategoria);
                        await _unitOfWork.movimentacaoCategoriaRepositorio.Adicionar(movimentacaoCategoria);
                    }
                }
                conta.ProcessaMovimentacao(movimentacao);
                _unitOfWork.contasRepositorio.Atualiza(conta);
                await _unitOfWork.Commit();

                return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
            }
            catch (MovimentacaoValidacao ex)
            {
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch (ContasValidacao ex)
            {
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
