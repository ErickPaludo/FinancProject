using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Contas.Get
{
    public sealed class CadastrarContaDTO
    {
        [Required(ErrorMessage = "O titúlo deverá ser informado!")]
        [MinLength(3, ErrorMessage = "O título deve possuir no mínimo 3 caracteres")]
        [MaxLength(100, ErrorMessage = "O título deve possuir no máximo 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;
    }
}
