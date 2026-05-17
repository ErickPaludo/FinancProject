using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Entidades.Segurança;
using Financ.Domain.Entidades.Usuarios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Contexto
{
    public class AppContextoData : DbContext
    {
        public AppContextoData(DbContextOptions<AppContextoData> options) : base(options) { }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<ContaUsuario> ContasUsuarios { get; set; }
        public DbSet<Convite> Convites { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Autenticacao> Autenticacao { get; set; }
        public DbSet<Movimentacao> Movimentacao { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<MovimentacaoCategoria> MovimentacaoCategoria { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContextoData).Assembly);
        }
    }
}
