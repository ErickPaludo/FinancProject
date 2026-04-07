using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Repositorios.Usuarios;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Usuarios
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
