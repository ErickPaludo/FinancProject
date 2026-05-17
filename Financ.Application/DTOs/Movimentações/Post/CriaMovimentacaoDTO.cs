using Financ.Domain.Enums.Movimentações;


namespace Financ.Application.DTOs.Movimentações.Post
{
    public record class CriaMovimentacaoDTO(TipoMovimentacao tipo, decimal valor, bool concluido, string titulo, string? observacao, DateTime? dthrMovimentacao, DateTime? dthrConclusao, int[]? IdsCategoria);
}
