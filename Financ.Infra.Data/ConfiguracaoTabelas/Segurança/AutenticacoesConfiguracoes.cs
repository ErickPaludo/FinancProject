using Financ.Domain.Entidades.Segurança;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas.Segurança
{
    public class AutenticacoesConfiguracoes : IEntityTypeConfiguration<Autenticacao>
    {
        public void Configure(EntityTypeBuilder<Autenticacao> builder)
        {
            builder.ToTable("fnc_autenticacao");
            builder.HasKey(e => e.IdSession);

            builder.HasOne(a => a.Usuario)
                   .WithMany()
                   .HasForeignKey(a => a.IdUsuario)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
