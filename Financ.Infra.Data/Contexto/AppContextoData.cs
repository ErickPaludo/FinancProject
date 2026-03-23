using Financ.Domain.Entidades;
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
        public AppContextoData(DbContextOptions<AppContextoData> options) : base(options){}
        public DbSet<Conta> Contas { get; set; }
        public DbSet<ContasUsuarios> ContasUsuarios { get; set; }
        public DbSet<Convites> Convites { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Autenticacao> Autenticacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContextoData).Assembly);
        }
    }
}
