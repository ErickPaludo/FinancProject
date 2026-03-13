using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Usuarios.Commands;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Autenticação;
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
        private readonly IUsuariosIdentityServicos _usuariosServico;
        public CadastraUsuarioHandler(IUsuariosIdentityServicos usuariosServico)
        {
            _usuariosServico = usuariosServico;
        }
        public async Task<Resultado<string>> Handle(CadastraUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Usuario usuario = new Usuario(request.PrimeiroNome, request.SegundoNome, request.Email);

                if (string.IsNullOrEmpty(await _usuariosServico.ObtemIdUsuario(request.Email)))
                {
                    var usuarioCriado = await _usuariosServico.RegistrarUsuario(usuario, request.Senha);

                    return usuarioCriado.Item1 ? Resultado<string>.GeraSucesso("Usuário criado com sucesso!") : Resultado<string>.GeraFalha(Falha.ErroOperacional(usuarioCriado.Item2!));
                }
                else
                {
                    return Resultado<string>.GeraFalha(Falha.ErroOperacional("Já existe um usuário cadastrado com esse e-mail."));
                }


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
