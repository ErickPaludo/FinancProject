using Financ.Domain.Entidades;
using Financ.Infra.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas
{
    public class ContasUsuariosConfiguracoes : IEntityTypeConfiguration<ContasUsuarios>
    {
        public void Configure(EntityTypeBuilder<ContasUsuarios> builder)
        {
            builder.ToTable("fnc_contas_usuarios");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.IdConta).IsRequired();
            builder.Property(e => e.IdUsuario).IsRequired();
            builder.Property(e => e.Acesso).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.DthrReg).IsRequired();

            builder.HasOne(e => e.Contas).WithMany(e => e.ContasUsuariosVinculados).HasForeignKey(e => e.IdConta).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<UsuarioIdentity>()
              .WithMany()
              .HasForeignKey(e => e.IdUsuario)
              .HasPrincipalKey(u => u.Id)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
