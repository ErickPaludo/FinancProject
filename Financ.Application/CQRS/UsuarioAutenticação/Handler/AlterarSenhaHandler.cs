using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.UsuarioAutenticação.Handler
{
    public class AlterarSenhaHandler : IRequestHandler<AlterarSenhaCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPassService _passService;
        public AlterarSenhaHandler(IUnitOfWork unitOfWork, IPassService passService)
        {
            _unitOfWork = unitOfWork;
            _passService = passService;
        }
        async Task<Resultado<string>> IRequestHandler<AlterarSenhaCommand, Resultado<string>>.Handle(AlterarSenhaCommand request, CancellationToken cancellationToken)
        {
            Usuario? usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Id.Equals(request.idUsuario));

            if (usuario is null)
                return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado!"));

            if (!_passService.ValidaPassArgon(usuario.HashPass, request.senhaAntiga, usuario.Salt))
                return Resultado<string>.GeraFalha(Falha.NaoAutorizado("Senha inválida."));

            var senha = _passService.CriaPassArgon(request.senhaNova);
            usuario.AtualizaSenha(senha.Salt,senha.Hash);
            _unitOfWork.usuariosRepostorio.Atualiza(usuario);
            await _unitOfWork.Commit();

            return Resultado<string>.GeraSucesso("Senha alterada com sucesso!");
        }
    }
}
