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
    public class MovimentacaoFixaDiariaConfiguracoes : IEntityTypeConfiguration<MovimentacaoFixaDiaria>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoFixaDiaria> builder)
        {
            builder.ToTable("fnc_movimentacoes_fixas_diaria");
            builder.HasKey(mfs => mfs.Id);

            builder.HasOne(mfs => mfs.MovimentacaoFixa)
                .WithMany()
                .HasForeignKey(mf => mf.IdFixo)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
