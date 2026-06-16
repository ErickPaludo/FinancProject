using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
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
    public class MaterializaMovimentacaoFixaHandler : IRequestHandler<MaterializaMovimentacaoFixaCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        public MaterializaMovimentacaoFixaHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(MaterializaMovimentacaoFixaCommand request, CancellationToken cancellationToken)
        {
            try
            {
             MovimentacaoFixa? movimentacoaFixa = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacaoFixaCompleta(x => x.Id == request.IdMovimentacao && x.Status == StatusMovimentacaoFixa.Ativo);

                if (movimentacoaFixa is null)
                    return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação fixa não encontrada!"));

                ContaUsuario? contaUsuario = movimentacoaFixa.Movimentacao.Conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);
                Movimentacao movimentacao = movimentacoaFixa.MaterializaMovimentacao(request.DataMovimentacao, contaUsuario);
                await _unitOfWork.movimentacaoRepositorio.Adicionar(movimentacao);

                await _unitOfWork.Commit();
                return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
            }
            catch (MovimentacaoFixaValidacao ex) {
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }

        }
    }
}
