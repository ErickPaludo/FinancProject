using Financ.Domain.Entidades.Movimentações;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas.Movimentações
{
    public class CategoriaConfiguração : IEntityTypeConfiguration<CategoriaMovimentacao>
    {
        public void Configure(EntityTypeBuilder<CategoriaMovimentacao> builder)
        {
            builder.ToTable("fnc_categorias");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).HasMaxLength(50);

            builder.HasOne(e => e.Conta)
                .WithMany()
                .HasForeignKey(e => e.IdConta)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
