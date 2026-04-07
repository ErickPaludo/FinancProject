using Financ.Domain.Entidades.ContasBancarias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas.ContasBancarias
{
    public class ContasUsuariosConfiguracoes : IEntityTypeConfiguration<ContaUsuario>
    {
        public void Configure(EntityTypeBuilder<ContaUsuario> builder)
        {
            builder.ToTable("fnc_contas_usuarios");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.IdConta).IsRequired();
            builder.Property(e => e.IdUsuario).IsRequired();
            builder.Property(e => e.Acesso).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.DthrReg).IsRequired();

            builder.HasOne(a => a.Usuario)
                   .WithMany()
                   .HasForeignKey(a => a.IdUsuario)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
