namespace Financ.Domain.Objetos_de_Valor.ContaUsuario
{
    public sealed record ExpiracaoConvite : ExpiracaoBase
    {
        private static readonly int _expiracaoDias = 7;
        private ExpiracaoConvite() : base(DateTime.UtcNow.AddDays(_expiracaoDias)){}
        public static ExpiracaoConvite Create() => new();
    }
}
