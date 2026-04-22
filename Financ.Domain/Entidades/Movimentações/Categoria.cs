using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.Movimentações
{
    public class Categoria
    {
        public int Id { get; private set; }
        public int IdConta { get; private set; }
        public string Nome { get; private set; }
        public Cor Cor { get; private set; }
        public Conta Conta { get; private set; }
        private Categoria() { }
        public Categoria(ContaUsuario? contaUsuario, string nome, string? cor)
        {
            CategoriaValidacao.Verifica(contaUsuario is null, "Usuário não encontrado");
            CategoriaValidacao.Verifica(contaUsuario!.Conta is null, "Conta não encontrada");
            CategoriaValidacao.Verifica(contaUsuario.Status is not TipoStatusContasUsuario.Ativo, "Usuário não está ativo!");
            CategoriaValidacao.Verifica(contaUsuario.Acesso is not TiposAcessos.Mestre, "Usuário deve possuir acesso mestre para essa implementação.");

            IdConta = contaUsuario!.Conta!.Id;
            Conta = contaUsuario!.Conta!;
            ValidaNome(nome);
            Nome = nome;
            Cor = new Cor(cor);
        }
        public void Remover(ContaUsuario? contaUsuario)
        {
            CategoriaValidacao.Verifica(contaUsuario is null, "Usuário não encontrado");
            CategoriaValidacao.Verifica(contaUsuario!.Conta is null, "Conta não encontrada");
            CategoriaValidacao.Verifica(contaUsuario.Status is not TipoStatusContasUsuario.Ativo, "Usuário não está ativo!");
            CategoriaValidacao.Verifica(contaUsuario.Acesso is not TiposAcessos.Mestre, "Usuário deve possuir acesso mestre para essa implementação.");
        }

        public void Alterar(ContaUsuario? contaUsuario, string? nome, string? cor)
        {
            CategoriaValidacao.Verifica(contaUsuario is null, "Usuário não encontrado");
            CategoriaValidacao.Verifica(contaUsuario!.Conta is null, "Conta não encontrada");
            CategoriaValidacao.Verifica(contaUsuario.Status is not TipoStatusContasUsuario.Ativo, "Usuário não está ativo!");
            CategoriaValidacao.Verifica(contaUsuario.Acesso is not TiposAcessos.Mestre, "Usuário deve possuir acesso mestre para essa implementação.");

            if (nome is not null)
            {
                ValidaNome(nome);
                Nome = nome.Trim();
            }
            if(cor is not null)
            {
                Cor = new Cor(cor);
            }
        }
        private void ValidaNome(string valor)
        {
            CategoriaValidacao.Verifica(string.IsNullOrWhiteSpace(valor), MensagemCategoria.NOME_OBRIGATORIO);
            CategoriaValidacao.Verifica(valor.Length < 2 || valor.Length > 50, MensagemCategoria.CARACTERES_INVALIDOS);
        }
    }
}
