using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Usuarios.Commands;
using Financ.Application.Interfaces.Segurança;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Domain.Validacoes;
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
                var converteSenha = _passService.CriaPassArgon(request.Senha);

                Usuario usuario = new Usuario(request.PrimeiroNome, request.SegundoNome, request.Email, converteSenha.salt, converteSenha.hash);

                if (!await _unitOfWork.usuariosRepostorio.ExisteId(x => x.Email.Equals(request.Email)))
                {
                    await _unitOfWork.usuariosRepostorio.Adicionar(usuario);
                    await _unitOfWork.Commit();
                    return Resultado<string>.GeraSucesso("Usuário criado com sucesso!");
                }
                else
                {
                    return Resultado<string>.GeraFalha(Falha.ErroOperacional("Já existe um usuário cadastrado com esse e-mail."));
                }

                return Resultado<string>.GeraFalha(Falha.ErroOperacional());

            }
            catch (UsuariosValidacoes ex)
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
            catch
            {
                return Resultado<string>.GeraFalha(Falha.ErroOperacional());
            }
        }
    }
}
