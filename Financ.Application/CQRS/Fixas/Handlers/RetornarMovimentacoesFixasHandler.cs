using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Fixas.Get;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Handlers
{
    public class RetornarMovimentacoesFixasHandler : IRequestHandler<RetornarMovimentacoesFixasCommand, Resultado<BaseGetList<GetMovimentacaoFixaDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidaPermissao _validaPermissao;

        public RetornarMovimentacoesFixasHandler(IUnitOfWork unitOfWork, IValidaPermissao validaPermissao)
        {
            _unitOfWork = unitOfWork;
            _validaPermissao = validaPermissao;
        }
        public async Task<Resultado<BaseGetList<GetMovimentacaoFixaDTO>>> Handle(RetornarMovimentacoesFixasCommand request, CancellationToken cancellationToken)
        {
            Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);
            if (conta is null)
                return Resultado<BaseGetList<GetMovimentacaoFixaDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

            ContaUsuario? contaUsuario = conta!.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

            if (contaUsuario is null) 
                return Resultado<BaseGetList<GetMovimentacaoFixaDTO>>.GeraFalha(Falha.NaoEncontrado("Usuário não pertence a conta!"));

            IQueryable<MovimentacaoFixa> fixas = _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacoesFixaCompleta(f => f.IdConta == request.IdConta && f.Status == StatusMovimentacaoFixa.Ativo);

            if(request.Tipo.HasValue)
                fixas = fixas.Where(x => x.Tipo == request.Tipo.Value);

            var fixasCarregadas = await fixas.ToListAsync();

            return Resultado<BaseGetList<GetMovimentacaoFixaDTO>>.GeraSucesso(new BaseGetList<GetMovimentacaoFixaDTO>(MovimentacoesFixasMapper.ParaDTO(fixasCarregadas)));
        }
    }
}
