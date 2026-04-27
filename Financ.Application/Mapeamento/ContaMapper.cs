using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Contas.NovaPasta;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Domain.Entidades;
using Financ.Domain.Entidades.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Financ.Application.Mapeamento
{
    public static class ContaMapper
    {
        public static ContaDashDTO ParaDTO(Conta conta) => new ContaDashDTO(conta.Id, conta.Titulo, conta.Cor.Valor);
        
    }
}
