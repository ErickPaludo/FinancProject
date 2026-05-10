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
    public class MovimentacaoCategoriaConfiguracao : IEntityTypeConfiguration<MovimentacaoCategoria>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoCategoria> builder)
        {
            builder.ToTable("fnc_movimentacao_categorias");
            builder.HasKey(mc => mc.Id);

            builder.HasOne(mc => mc.Categoria)
                .WithMany()
                .HasForeignKey(mc => mc.IdCategoria)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
