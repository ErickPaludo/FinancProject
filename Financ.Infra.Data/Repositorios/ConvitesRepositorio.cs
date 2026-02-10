using Dapper;
using Financ.Application.Leitura.Convite;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data.Contexto;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios
{
    public class ConvitesRepositorio : BaseRepositorio<Convites>, IConvitesRepostorio
    {
        private AppContextoData _contexto;

        public ConvitesRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }
    }
}
