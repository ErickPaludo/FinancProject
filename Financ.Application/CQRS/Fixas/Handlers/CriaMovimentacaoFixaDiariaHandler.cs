using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class CriaMovimentacaoFixaDiariaHandler : IRequestHandler<CriaMovimentacaoFixaDiariaCommand, Resultado<BasePost<string>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CriaMovimentacaoFixaDiariaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<string>>> Handle(CriaMovimentacaoFixaDiariaCommand request, CancellationToken cancellationToken)
        {
            try
            {

            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuarioPredicado(x => x.IdUsuario == request.idUsuario && x.IdConta == request.idConta);

            Movimentacao movimentacao = new Movimentacao(request.tipo, contaUsuario, request.valor, request.titulo, request.observacao, null, null, false);

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

            MovimentacaoFixa fixa = new MovimentacaoFixa(request.DataInicio, request.DataFim, request.OcorrenciasDiarias, movimentacao);
            await _unitOfWork.movimentacaoFixaRepositorio.Adicionar(fixa);

            await _unitOfWork.Commit();
            return Resultado<BasePost<string>>.GeraSucesso(new BasePost<string>("Movimentação fixa gerada com sucesso"));
            
            }catch(MovimentacaoFixaValidacao ex)
            {
                return Resultado<BasePost<string>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch (MovimentacaoValidacao ex)
            {
                return Resultado<BasePost<string>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}

