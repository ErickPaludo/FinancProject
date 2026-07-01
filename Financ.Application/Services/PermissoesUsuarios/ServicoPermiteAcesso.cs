using Financ.Application.Comun.Enums;
using Financ.Domain.Enums.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services.PermissoesUsuarios
{
    public static class ServicoPermiteAcesso
    {
        private static readonly Dictionary<TiposAcessos, PermissoesContasUsuarios[]> _permissoes = new()
        {
            [TiposAcessos.Mestre] =
            [PermissoesContasUsuarios.EditarContaBancaria,
             PermissoesContasUsuarios.CriarConvite,
             PermissoesContasUsuarios.RevogarConvite,
             PermissoesContasUsuarios.EditarContaUsuario,
             PermissoesContasUsuarios.ExpurgarContaUsuario,
             PermissoesContasUsuarios.ConsultarMovimentacao,
             PermissoesContasUsuarios.CadastrarMovimentacao,
             PermissoesContasUsuarios.EditarMovimentacao,
             PermissoesContasUsuarios.ConcluirMovimentcao,
             PermissoesContasUsuarios.ExtornarMovimentacao,
             PermissoesContasUsuarios.ExcluirMovimentacao,
             PermissoesContasUsuarios.CadastrarCategoria,
             PermissoesContasUsuarios.EditarCategoria,
             PermissoesContasUsuarios.ExluirCategoria,
             PermissoesContasUsuarios.ConsultarMovimentacaoFixa,
             PermissoesContasUsuarios.CadastrarMovimentacaoFixa,
             PermissoesContasUsuarios.EditarMovimentacaoFixa],

            [TiposAcessos.Administrador] =
            [
             PermissoesContasUsuarios.ConsultarMovimentacao,
             PermissoesContasUsuarios.CadastrarMovimentacao,
             PermissoesContasUsuarios.EditarMovimentacao,
             PermissoesContasUsuarios.ConcluirMovimentcao,
             PermissoesContasUsuarios.ExtornarMovimentacao,
             PermissoesContasUsuarios.ExcluirMovimentacao],

            [TiposAcessos.Visualizador] =
            [PermissoesContasUsuarios.ConsultarMovimentacao]
        };
        public static bool PossuiPermissao(TiposAcessos acesso, PermissoesContasUsuarios permissoes)
            => _permissoes[acesso].Contains(permissoes);
    }
}
