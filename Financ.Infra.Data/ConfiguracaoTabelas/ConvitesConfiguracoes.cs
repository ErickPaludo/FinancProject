using Financ.Domain.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Infra.Data.Identity;

namespace Financ.Infra.Data.ConfiguracaoTabelas
{
    public class ConvitesConfiguracoes : IEntityTypeConfiguration<Convites>
    {
        public void Configure(EntityTypeBuilder<Convites> builder)
        {
            builder.ToTable("fnc_convites");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.IdConta).IsRequired();
            builder.Property(e => e.IdUsuarioRemetente).IsRequired();
            builder.Property(e => e.IdUsuarioDestinatario).IsRequired();
            builder.Property(e => e.Expiracao).IsRequired();

            // Índices para performance
            builder.HasIndex(e => e.IdConta);
            builder.HasIndex(e => e.IdUsuarioRemetente);
            builder.HasIndex(e => e.IdUsuarioDestinatario);

            // Mapeia a string para a tabela do Identity sem precisar do objeto na classe Convites
            builder.HasOne<UsuarioIdentity>()
                .WithMany()
                .HasForeignKey(e => e.IdUsuarioRemetente)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Conta)
                 .WithMany(u => u.Convites)
                 .HasForeignKey(u => u.IdConta)
                 .OnDelete(DeleteBehavior.Cascade);


            //// Relação com remetente
            //builder.HasOne(e => e.Remetente)
            //    .WithMany()
            //    .HasForeignKey(e => e.IdUsuarioRemetente)
            //    .OnDelete(DeleteBehavior.Restrict);

            //// Relação com destinatário
            //builder.HasOne(e => e.Destinatario)
            //    .WithMany()
            //    .HasForeignKey(e => e.IdUsuarioDestinatario)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
