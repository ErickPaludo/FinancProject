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
    public class ContasConfiguracao : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.ToTable("fnc_contas");
            builder.Property(e => e.Titulo).IsRequired().HasMaxLength(100);
            builder.Property(e => e.TipoConta).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.DthrReg).IsRequired();
            builder.HasKey(e => e.Id);

            builder.HasMany(c => c.ContaUsuarios)
                   .WithOne(u => u.Conta)
                   .HasForeignKey(u => u.IdConta)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Convites)
                  .WithOne(u => u.Conta)
                  .HasForeignKey(u => u.IdConta)
                  .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
#region Linhas de Credito
//builder.Property(e => e.DiaFechamento).HasMaxLength(16).HasPrecision(2,0);
//builder.Property(e => e.DiaVencimento).HasMaxLength(12).HasPrecision(2, 0);
#endregion
