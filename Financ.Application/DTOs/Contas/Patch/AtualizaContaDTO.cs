using Financ.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Contas.Ptch
{
    public class AtualizaContaDTO 
    {   
        public string? Titulo { get; set; }
        public TiposStatusContas? Status { get; set; }
    }
}
