using Financ.Application.DTOs.Categorias.Get;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class MovimentacaoMapper
    {
        public static MovimentacaoDTO ParaDTO(Movimentacao movimentacao)
        {
            return new MovimentacaoDTO
            (movimentacao.Id,
            movimentacao.Tipo,
            movimentacao.IdConta,
            movimentacao.IdFixo,
            movimentacao.Status is TipoStatusMovimentacao.Concluido,
            movimentacao.Valor,
            movimentacao.Titulo,
            movimentacao.Observacao,
            movimentacao.DthrReg ?? DateTime.MinValue,
            movimentacao.DthrMovimentacao ?? DateTime.MinValue,
            movimentacao.DthrConclusao,
            ContaUsuarioMapper.ParaUsuarioDTO(movimentacao.ContaUsuarioCriador),
            (movimentacao.ContaUsuarioExecutor is null ? null : ContaUsuarioMapper.ParaUsuarioDTO(movimentacao.ContaUsuarioExecutor)),
            (movimentacao.IdCategoria is null || movimentacao.Categoria is null) ? null : CategoriaMapper.ParaDTO(movimentacao.Categoria));
        }
        public static RetornaMovimentacaoDTO ParaDTO(ResumoMovimentacoesDTO resumoDTO, IEnumerable<Movimentacao>? movimentacao)
        {
            List<MovimentacaoDTO> listaMovimentacoes = movimentacao?.Select(ParaDTO).ToList() ?? new List<MovimentacaoDTO>();
            return new RetornaMovimentacaoDTO(resumoDTO, listaMovimentacoes);
        }

    }
}
