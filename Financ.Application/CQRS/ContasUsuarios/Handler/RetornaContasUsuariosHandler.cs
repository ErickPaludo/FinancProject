using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;


namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaContasUsuariosHandler : IRequestHandler<RetornaContaUsuariosQuery, Resultado<BaseGet<RetornarContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaContasUsuariosHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGet<RetornarContasDTO>>> Handle(RetornaContaUsuariosQuery request, CancellationToken cancellationToken)
        {
            var contasUsuarios = await ContasUsuariosSelecionadas(request);
            var idsContas = contasUsuarios.Select(c => c.Conta.Id).ToList();

            var fixos = await _unitOfWork.movimentacaoFixaRepositorio.BuscaMovimentacoesFixaCompleta(x => idsContas.Contains(x.IdConta) && x.Status == StatusMovimentacaoFixa.Ativo).ToListAsync();

            var movimentacoes = await _unitOfWork.movimentacaoRepositorio.BuscarPorCondicao(m => idsContas.Contains(m.IdConta) && m.Status != StatusMovimentacao.Oculta);

            var movimentacoesPendentes = movimentacoes.Where(x => x.Status == StatusMovimentacao.Pendente).ToList();
            var movimentacoesConcluidas = movimentacoes.Where(x => x.Status == StatusMovimentacao.Concluido).ToList();

            foreach (var contaUsuario in contasUsuarios)
            {
                var fixoConta = fixos.Where(x => x.IdConta == contaUsuario.IdConta).ToList();
                if (fixoConta.Count > 0)
                {
                    VirtualizaMovimentacoesFixasService virtualizaMovimentacao =
                     new VirtualizaMovimentacoesFixasService(movimentacoes.Where(m => m.Conta == contaUsuario.Conta), fixoConta, fixoConta.Min(x => x.DataInicio), fixoConta.Max(x => x.DataFim), contaUsuario);

                    var mensal = virtualizaMovimentacao.Mensal();
                    var anual = virtualizaMovimentacao.Anual();
                    var diario = virtualizaMovimentacao.Diario();

                    movimentacoesPendentes.AddRange(mensal);
                    movimentacoesPendentes.AddRange(anual);
                    movimentacoesPendentes.AddRange(diario);
                }
            }

            List<RetornarContasDTO> listaContas = new List<RetornarContasDTO>();


            return Resultado<BaseGet<RetornarContasDTO>>.GeraSucesso(ContaUsuarioMapper.ParaDTO(contasUsuarios, movimentacoesPendentes, request.Filtros));
        }
        private async Task<IEnumerable<ContaUsuario>> ContasUsuariosSelecionadas(RetornaContaUsuariosQuery filtros)
        {
            var filtroId = filtros.Filtros?.Id;
            var filtroTitulo = filtros.Filtros?.Titulo;
            var filtroStatus = filtros.Filtros?.Status;
            var possuiFiltros = filtros.Filtros != null;

            var contasUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(
                x => x.IdUsuario == filtros.IdUsuario && x.Status == StatusContasUsuario.Ativo
                && (!possuiFiltros || (
                    (!filtroId.HasValue || x.IdConta == filtroId.Value) &&
                    (string.IsNullOrEmpty(filtroTitulo) || x.Conta!.Titulo!.Contains(filtroTitulo)) &&
                    (!filtroStatus.HasValue || x.Conta!.Status == filtroStatus.Value)))
            );



            return contasUsuario.OrderByDescending(x => x.ContaFavorita).ThenByDescending(x => x.DthrReg).ToList();
        }
        //private IEnumerable<Movimentacao> VirtualizaMovimentacaoFixa(IEnumerable<Movimentacao> movimentacoes, IEnumerable<MovimentacaoFixa> fixos)
        //{
        //    VirtualizaMovimentacoesFixasService virtualizaMovimentacao =
        //        new VirtualizaMovimentacoesFixasService(movimentacoes, fixos, dataInicio, dataFim);
        //    return virtualizaMovimentacao.Mensal();
        //}
    }
}
