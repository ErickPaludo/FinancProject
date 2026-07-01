using Financ.Application.Exceções;
using Financ.Application.Services;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application
{
    public class ExisteContaUsuario : IExisteContaUsuario
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExisteContaUsuario(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ContaUsuario> Buscar(Expression<Func<ContaUsuario, bool>> predicate)
        {
            ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(predicate);
            if (contaUsuario is null)
                throw new ExceptionNaoEncontrado("Usuario não encontrado");
            return contaUsuario;
        }
    }
}
