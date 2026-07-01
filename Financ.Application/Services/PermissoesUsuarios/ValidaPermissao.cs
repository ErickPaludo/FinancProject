using Financ.Application.Comun.Enums;
using Financ.Application.Interfaces;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Validacoes.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services.PermissoesUsuarios
{
    public class ValidaPermissao : IValidaPermissao
    {
        public void Valiidar(ContaUsuario usuario, PermissoesContasUsuarios acao)
        {
            if(usuario.Status != StatusContasUsuario.Ativo)
            {
                throw new ExceptionPermissoes("O usuário não está ativo na conta.");
            }
            if(!ServicoPermiteAcesso.PossuiPermissao(usuario.Acesso, acao))
            {
                throw new ExceptionPermissoes("O usuário não possui permissão para realizar esta ação.");
            }
        }
    }
}
