using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios
{
    public class UsuariosRepositorio : BaseRepositorio<Usuario>, IUsuariosRepositorio
    {
        private AppContextoData _contexto;

        public UsuariosRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }
    }
}
