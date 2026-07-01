using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Entidades
{
    public abstract class EntidadeBase
    {
        public Guid Id { get; private set; } = new Guid();
        public DateTime DataHoraHoraRegistro { get; private set; } = DateTime.UtcNow;
    }
}
