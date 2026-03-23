using Dapper;
using Financ.Application.Interfaces.Convites;
using Financ.Application.Leitura.Convite;
using Financ.Domain.Entidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Leitura
{
    public class ConvitesLeituraRepositorio : IConvitesLeituraRepositorio
    {
        private readonly string _connectionString;

        public ConvitesLeituraRepositorio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServer");
        }

        public async Task<IEnumerable<LeituraRetornoConvites>> RetornoConvites(string idUsuario, bool retornaConvitesRemetente)
        {
            using IDbConnection db = new SqlConnection(_connectionString);

            string sql = $@"select cv.id,
                           cv.Acesso,
                           cv.Aceito,
                           cv.DataEnvio, 
                           cv.Expiracao, 
                           c.Id as IdConta,
                           c.Titulo, 
                           c.TipoConta,
                           r.Id as IdUsuarioRemetente,
                           r.PrimeiroNome as PrimeiroNomeRemetente,
                           r.SegundoNome  as SegundoNomeRemetente,
                           r.PrimeiroNome + ' ' + r.SegundoNome as NomeCompletoRemetente,
                           d.Id as IdUsuarioDestinatario,
                           d.PrimeiroNome as PrimeiroNomeDestinatario,
                           d.SegundoNome  as SegundoNomeDestinatario,
                           d.PrimeiroNome + ' ' + d.SegundoNome as NomeCompletoDestinatario
                           from fnc_convites cv 
                           inner join aspnetusers r ON 
                           r.Id = cv.IdUsuarioRemetente 
                           inner join aspnetusers d ON 
                           d.Id = cv.IdUsuarioDestinatario
                           inner join fnc_contas c
                           on c.id = cv.IdConta 
                           where 
                           {(retornaConvitesRemetente ? "cv.IdUsuarioRemetente" : "cv.IdUsuarioDestinatario")} = @idUsuario
                           and cv.aceito is null
                           and sysdatetime() <= cv.Expiracao";

            return await db.QueryAsync<LeituraRetornoConvites>(sql, new { idUsuario });

        }
    }
}
