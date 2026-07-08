namespace Financ.Domain.Objetos_de_Valor.ContaUsuario
{
    public sealed record PreferenciasContaUsuario
    {
        public bool ContaFavorita { get; private set; } = false;
        public bool AutoSoma { get; private set; } = true;

        private PreferenciasContaUsuario() { }
        public static PreferenciasContaUsuario Create() => new();
        public void Favoritar() => ContaFavorita = !ContaFavorita;
        public void PerimiteAutoSoma() => AutoSoma = !AutoSoma;
    }
}
