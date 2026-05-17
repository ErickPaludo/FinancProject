using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Entidades.ContasBancarias;
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
            DateTime.SpecifyKind(movimentacao.DthrReg, DateTimeKind.Utc),
            DateTime.SpecifyKind(movimentacao.DthrMovimentacao, DateTimeKind.Utc),
            movimentacao.DthrConclusao.HasValue ? DateTime.SpecifyKind(movimentacao.DthrConclusao.Value, DateTimeKind.Utc) : null,
ContaUsuarioMapper.ParaUsuarioDTO(movimentacao.ContaUsuarioCriador),
            (movimentacao.ContaUsuarioExecutor is null ? null : ContaUsuarioMapper.ParaUsuarioDTO(movimentacao.ContaUsuarioExecutor)),
            movimentacao.CategoriasMovimentacao is not null ? movimentacao.CategoriasMovimentacao.Select(mc => CategoriaMapper.ParaDTO(mc.Categoria)).ToList() : null
            );
        }
        public static RetornaMovimentacaoDTO ParaDTO(ResumoMovimentacoesDTO resumoDTO, IEnumerable<Movimentacao>? movimentacao)
        {
            List<MovimentacaoDTO> listaMovimentacoes = movimentacao?.Select(m => ParaDTO(m)).ToList() ?? new List<MovimentacaoDTO>();
            return new RetornaMovimentacaoDTO(resumoDTO, listaMovimentacoes);
        }
    }

}
