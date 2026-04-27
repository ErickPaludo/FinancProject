using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;

namespace Financ.Application.CQRS.Contas_Usuarios.Handler
{
    public class RetornaUsuariosAssociadosHandler : IRequestHandler<RetornaUsuariosAssociadosQuery, Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaUsuariosAssociadosHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>> Handle(RetornaUsuariosAssociadosQuery request, CancellationToken cancellationToken)
        {
            Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);

            if (conta is null)
                return Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada!"));

            if(!conta.ContaUsuarios.Any(x => x.IdUsuario == request.IdUsuario))
                return Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>.GeraFalha(Falha.ErroOperacional("Usuário não pertence a está conta."));
            var filtro = request.filtroConta;

            IQueryable<ContaUsuario> contaUsuarioQuery = _unitOfWork.contasUsuariosRepositorio.ObterContaUsuarioComUsuario();

            contaUsuarioQuery = contaUsuarioQuery.Where(x => x.IdConta == request.IdConta);

            if(filtro.IdUsuario is not null)
                contaUsuarioQuery = contaUsuarioQuery.Where(x => x.IdUsuario == filtro.IdUsuario);

            if(filtro.Status.HasValue)
                contaUsuarioQuery = contaUsuarioQuery.Where(x => x.Status.Equals(filtro.Status.Value));

            if(filtro.Acesso.HasValue)
                contaUsuarioQuery = contaUsuarioQuery.Where(x => x.Acesso.Equals(filtro.Acesso.Value));

            if (filtro.NomeUsuario is not null)
                contaUsuarioQuery =  contaUsuarioQuery.Where(x => x.Usuario.NomeCompleto.Contains(filtro.NomeUsuario));

            var contaUsuarios = await contaUsuarioQuery.ToListAsync();

            return Resultado<BaseGetList<RetornaUsuariosAssociadosDTO>>.GeraSucesso(new BaseGetList<RetornaUsuariosAssociadosDTO>(ContaUsuarioMapper.ParaUsuarioDTO(contaUsuarios)));
        }
    }
}
