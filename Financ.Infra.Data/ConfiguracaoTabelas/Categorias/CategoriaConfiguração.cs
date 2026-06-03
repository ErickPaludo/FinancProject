using Financ.Domain.Entidades.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.ConfiguracaoTabelas.Categorias
{
    public class CategoriaConfiguração : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("fnc_categorias");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Nome).HasMaxLength(50);

            builder.HasOne(e => e.Conta)
                .WithMany()
                .HasForeignKey(e => e.IdConta)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(c => c.Cor, cor =>
            {
                cor.Property(p => p.Valor)
                   .HasColumnName("Cor")
                   .IsRequired();
            });
        }
    }
}
