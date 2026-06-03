using Financ.Domain.Entidades.Movimentações.Fixas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas.Movimentações.Fixas
{
    public class MovimentacaoFixaConfiguracao : IEntityTypeConfiguration<MovimentacaoFixa>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoFixa> builder)
        {
            builder.ToTable("fnc_movimentacoes_fixas");
            builder.HasKey(mf => mf.Id);
            builder.Property(mf => mf.Tipo).IsRequired();
            builder.Property(mf => mf.Status).IsRequired();
            builder.Property(mf => mf.DataInicio).IsRequired();
            builder.Property(mf => mf.DataFim).IsRequired();
            builder.Property(mf => mf.Dthr).IsRequired();

            builder.HasOne(mf => mf.Movimentacao)
            .WithMany()
            .HasForeignKey(mf => mf.IdMovimentacao)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
