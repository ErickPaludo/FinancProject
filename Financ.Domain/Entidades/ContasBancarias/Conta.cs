using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Objetos_de_Valor;
using Financ.Domain.Validacoes.Base.Mensagens;
using Financ.Domain.Validacoes.ContasBancarias;
using Financ.Domain.Validacoes.ContasBancarias.Mensagens;
using Financ.Domain.Validacoes.Movimentações;
using Financ.Domain.Validacoes.Movimentações.Mensagens;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades.ContasBancarias
{
    public sealed class Conta : BaseConta
    {
        public string Titulo { get; private set; }
        public StatusContas Status { get; private set; }
        public TipoConta TipoConta { get; private set; }
        public decimal Saldo { get; private set; }
        public Cor Cor { get; private set; }
        public byte[] RowVersion { get; private set; }
        private readonly List<ContaUsuario> _contasUsuarios = new();
        public IReadOnlyCollection<ContaUsuario> ContaUsuarios => _contasUsuarios;
        private readonly List<Convite> _convites = new();
        public IReadOnlyCollection<Convite> Convites => _convites;
        private Conta() { }

        public Conta(string titulo, string? cor)
        {   
            titulo = titulo.Trim();
            ValidaTitulo(titulo);
            Titulo = titulo;
            Status = StatusContas.Ativo;
            TipoConta = TipoConta.Corrente;
            Cor = new Cor(cor);
            DthrReg = DateTime.UtcNow;

        }
        public void AtualizaConta(string? titulo, StatusContas? status, string? cor = null)
        {
           
            if (cor != null)
                Cor = new Cor(cor);

            if (titulo is not null)
            {
                titulo = titulo.Trim();
                ValidaTitulo(titulo);
            }

            if (status.HasValue)
                ValidaStatusConta(status.Value);
        }
        public bool ConviteEmAndamento(string idUsuario)
        {
            return Convites.Any(x => x.IdUsuarioDestinatario == idUsuario
            && DateTime.UtcNow <= x.Expiracao
            && x.Aceito == null);
        }
        public bool UsuarioPertenceConta(string idUsuario)
        {
            return ContaUsuarios.Any(x => x.IdUsuario == idUsuario && (x.Expiracao is null || x.Expiracao >= DateTime.UtcNow));
        }
        public void ProcessaMovimentacao(Movimentacao movimentacao)
        {
            if (movimentacao.Status is StatusMovimentacao.Concluido)
            {
                ContasValidacao.Verifica(movimentacao.Extorno, MensagensContas.NAO_PODE_PROCESSAR_MOVIMENTACAO_COM_EXTORNO);
                ContasValidacao.Verifica(movimentacao.Tipo.Equals(TipoMovimentacao.Saida) && movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
                Saldo = movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) ? Saldo + movimentacao.Valor : Saldo - movimentacao.Valor;
            }
        }
        public void ProcessaExtornoMovimentacao(Movimentacao movimentacao)
        {
            ContasValidacao.Verifica(!movimentacao.Extorno, MensagensContas.NAO_PODE_PROCESSAR_MOVIMENTACAO_SEM_EXTORNO);
            ContasValidacao.Verifica(movimentacao.Status is not StatusMovimentacao.Pendente, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
            ContasValidacao.Verifica(movimentacao.DthrConclusao is not null, MensagensContas.EXTORNO_DE_MOVIMENTACAO_COM_DATA_DE_CONCLUSAO);
            ContasValidacao.Verifica(movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) && movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
            Saldo = movimentacao.Tipo.Equals(TipoMovimentacao.Entrada) ? Saldo - movimentacao.Valor : Saldo + movimentacao.Valor;
        }

        public void RemoverMovimentacao(Movimentacao movimentacao)
        {
            if (movimentacao.Extorno)
            {
                if (movimentacao.Tipo.Equals(TipoMovimentacao.Entrada))
                {
                    ContasValidacao.Verifica(movimentacao.Valor > Saldo, MensagensContas.SALDO_INSUFICIENTE);
                    Saldo -= movimentacao.Valor;
                }
                else
                    Saldo += movimentacao.Valor;
            }
        }

        private void ValidaTitulo(string titulo)
        {
            ContasValidacao.Verifica(string.IsNullOrWhiteSpace(titulo), MensagensContas.TITULO_OBRIGATORIO);
            ContasValidacao.Verifica(titulo.Length < 2 || titulo.Length > 30, MensagensContas.TITULO_TAMANHO_INVALIDO);
        }
        private void ValidaStatusConta(StatusContas status)
        {
            ContasValidacao.Verifica(!Enum.IsDefined(typeof(StatusContas), status), MensagensBase.STATUS_INVALIDO);
            Status = status;
        }
    
    }
}
