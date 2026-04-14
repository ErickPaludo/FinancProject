using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;


namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaContasUsuariosHandler : IRequestHandler<RetornaContaUsuariosQuery, Resultado<BaseGetList<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaContasUsuariosHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGetList<RetornaContasDTO>>> Handle(RetornaContaUsuariosQuery request, CancellationToken cancellationToken)
        {
            var contasUsuarios = await ContasUsuariosSelecionadas(request);
            var idsContas = contasUsuarios.Select(c => c.Conta.Id).ToList();
            var movimentacoes = await _unitOfWork.movimentacaoRepositorio.BuscarPorCondicao(m => idsContas.Contains(m.IdConta) && m.Status == TipoStatusMovimentacao.Pendente
);
            if (contasUsuarios.Count() == 0)
                return Resultado<BaseGetList<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhuma conta foi encontrada!"));

            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();

            return Resultado<BaseGetList<RetornaContasDTO>>.GeraSucesso(ContaUsuarioMapper.ParaDTO(contasUsuarios,movimentacoes, request.Filtros));
        }
        private async Task<IEnumerable<ContaUsuario>> ContasUsuariosSelecionadas(RetornaContaUsuariosQuery filtros)
        {
            var filtroId = filtros.Filtros?.Id;
            var filtroTitulo = filtros.Filtros?.Titulo;
            var filtroStatus = filtros.Filtros?.Status;
            var possuiFiltros = filtros.Filtros != null;

            var contasUsuario = await _unitOfWork.contasUsuariosRepositorio.ObterContasDoUsuario(
                x => x.IdUsuario == filtros.IdUsuario
                && (!possuiFiltros || (
                    (!filtroId.HasValue || x.IdConta == filtroId.Value) &&
                    (string.IsNullOrEmpty(filtroTitulo) || x.Conta!.Titulo!.Contains(filtroTitulo)) &&
                    (!filtroStatus.HasValue || x.Conta!.Status == filtroStatus.Value)))
            );
            return contasUsuario;
        }
    }
}
