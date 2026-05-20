using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Usuarios.Commands;
using Financ.Application.Interfaces.Segurança;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Domain.Validacoes.Usuarios;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Usuarios.Handler
{
    public class CadastraUsuarioHandler : IRequestHandler<CadastraUsuarioCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISegurancaServico _passService;
        public CadastraUsuarioHandler(IUnitOfWork unitOfWork, ISegurancaServico passService)
        {
            _unitOfWork = unitOfWork;
            _passService = passService;
        }
        public async Task<Resultado<string>> Handle(CadastraUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Email = request.Email.Trim();

                if (await _unitOfWork.usuariosRepostorio.ExisteId(x => x.Email == request.Email))
                    return Resultado<string>.GeraFalha(Falha.ErroOperacional("Já existe um usuário cadastrado com esse e-mail."));

                if(request.Senha != request.ConfirmarSenha)
                    return Resultado<string>.GeraFalha(Falha.ErroOperacional("A senhas não são identicas!")); //Não é bom validar nessa camada, deverá alterar futuramente

                var converteSenha = _passService.CriaSenhaArgon(request.Senha);
                Usuario usuario = new Usuario(request.PrimeiroNome, request.SegundoNome, request.Email, converteSenha.salt, converteSenha.hash);
                await _unitOfWork.usuariosRepostorio.Adicionar(usuario);
                await _unitOfWork.Commit();
                return Resultado<string>.GeraSucesso("Usuário criado com sucesso!");
            }
            catch (UsuariosValidacoes ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch (Exception ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
