using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_.Querys;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.Contas.Get.Filtros;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_.Handler
{
    public class RetornaContasHandler : IRequestHandler<RetornaContaQuery, Resultado<List<RetornaContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaContasHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<List<RetornaContasDTO>>> Handle(RetornaContaQuery request, CancellationToken cancellationToken)
        {
           var contasUsuarios = await ContasUsuariosSelecionadas(request);

            if (contasUsuarios.Count() == 0)
                return Resultado<List<RetornaContasDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhuma conta foi encontrada!"));

            List<RetornaContasDTO> listaContas = new List<RetornaContasDTO>();

            return Resultado<List<RetornaContasDTO>>.GeraSucesso(ContasUsuariosMapper.ParaDTO(contasUsuarios));
        }
        private async Task<IEnumerable<ContasUsuarios>> ContasUsuariosSelecionadas(RetornaContaQuery filtros)
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
