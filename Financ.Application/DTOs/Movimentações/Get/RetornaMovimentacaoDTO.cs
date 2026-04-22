using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.Convites.Get.MicroDto;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get
{
    public record RetornaMovimentacaoDTO(ResumoMovimentacoesDTO Resumo, List<MovimentacaoDTO> Movimentacaos);
}
