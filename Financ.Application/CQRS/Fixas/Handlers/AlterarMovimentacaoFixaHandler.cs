using Financ.Application.Comun.Enums;
using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Fixas.Get;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class AlterarMovimentacaoFixaHandler : IRequestHandler<AlterarMovimentacaoFixaCommand, Resultado<GetMovimentacaoFixaDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidaPermissao _validaPermissao;

        public AlterarMovimentacaoFixaHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
        }

        public async Task<Resultado<GetMovimentacaoFixaDTO>> Handle(AlterarMovimentacaoFixaCommand request, CancellationToken cancellationToken)
        {
            MovimentacaoFixa? movimentacoaFixa = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacaoFixaCompleta(x => x.Id == request.IdFixa && x.Status == StatusMovimentacaoFixa.Ativo && x.Tipo != TipoMovimentacaoFixa.Diaria);

            if (movimentacoaFixa is null)
                return Resultado<GetMovimentacaoFixaDTO>.GeraFalha(Falha.NaoEncontrado("Movimentação fixa não encontrada!"));

            ContaUsuario? contaUsuario = movimentacoaFixa.Movimentacao.Conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

            if (contaUsuario is null)
                return Resultado<GetMovimentacaoFixaDTO>.GeraFalha(Falha.NaoEncontrado($"Usuário não pertence a conta!"));

            _validaPermissao.Valiidar(contaUsuario, PermissoesContasUsuarios.EditarMovimentacaoFixa);

            movimentacoaFixa.AlteraMovimentacaoFixa(contaUsuario, request.Tipo, request.Status, request.DataInicio, request.DataFim, request.DataOcorrencia);

            _unitOfWork.movimentacaoFixaRepositorio.Atualiza(movimentacoaFixa);
            await _unitOfWork.Commit();
            return Resultado<GetMovimentacaoFixaDTO>.GeraSucesso(MovimentacoesFixasMapper.ParaDTO(movimentacoaFixa));
        }
    }
}
