using Financ.Application.Comun.Enums;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
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
    public class MaterializaMovimentacaoFixaHandler : IRequestHandler<MaterializaMovimentacaoFixaCommand, Resultado<BasePost<MovimentacaoDTO>>>
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidaPermissao _validaPermissao;

        public MaterializaMovimentacaoFixaHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
        }
        public async Task<Resultado<BasePost<MovimentacaoDTO>>> Handle(MaterializaMovimentacaoFixaCommand request, CancellationToken cancellationToken)
        {
            MovimentacaoFixa? movimentacoaFixa = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacaoFixaCompleta(x => x.Id == request.IdMovimentacao && x.Status == StatusMovimentacaoFixa.Ativo);

            if (movimentacoaFixa is null)
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Movimentação fixa não encontrada!"));

            ContaUsuario? contaUsuario = movimentacoaFixa.Movimentacao.Conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

            if (contaUsuario is null)
                return Resultado<BasePost<MovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado($"Usuário não pertence a conta!"));

            _validaPermissao.Valiidar(contaUsuario, PermissoesContasUsuarios.CadastrarMovimentacaoFixa);

            Movimentacao movimentacao = movimentacoaFixa.MaterializaMovimentacao(request.DataMovimentacao, contaUsuario);
            await _unitOfWork.movimentacaoRepositorio.Adicionar(movimentacao);

            await _unitOfWork.Commit();
            return Resultado<BasePost<MovimentacaoDTO>>.GeraSucesso(new BasePost<MovimentacaoDTO>(MovimentacaoMapper.ParaDTO(movimentacao)));
        }
    }
}
