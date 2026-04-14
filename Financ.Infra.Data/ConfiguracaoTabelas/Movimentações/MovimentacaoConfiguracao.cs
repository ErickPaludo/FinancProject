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
    public class MovimentacaoConfiguracao : IEntityTypeConfiguration<Movimentacao>
    {
        public void Configure(EntityTypeBuilder<Movimentacao> builder)
        {
            builder.ToTable("fnc_movimentacoes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Tipo).IsRequired();
            builder.Property(e => e.IdConta).IsRequired();
            builder.Property(e => e.IdUsuarioCriador).IsRequired();

            builder.Property(e => e.Titulo).HasMaxLength(80);
            builder.Property(e => e.Observacao).HasMaxLength(255);

            builder.Property(e => e.Valor).HasColumnType("decimal(18,2)");
            builder.HasOne(e => e.Conta)
                .WithMany()
                .HasForeignKey(e => e.IdConta)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ContaUsuarioCriador)
            .WithMany()
            .HasForeignKey(e => e.IdUsuarioCriador)
            .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(e => e.ContaUsuarioExecutor)
            .WithMany()
            .HasForeignKey(e => e.IdUsuarioExecutor)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Categoria)
           .WithMany()
           .HasForeignKey(e => e.IdCategoria)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
