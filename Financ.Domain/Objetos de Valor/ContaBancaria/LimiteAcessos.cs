using Financ.Domain.Validacoes.ContasBancarias;

namespace Financ.Domain.Objetos_de_Valor.ContaBancaria
{
    public sealed record LimiteAcessos
    {
        private static readonly int _limiteMestres = 10;
        private static readonly int _limiteAdministradores = 15;
        private static readonly int _limiteVisualizadores = 15;
        public int LimiteMestres { get; }
        public int LimiteAdministradores { get; }
        public int LimiteVisualizadores { get; }

        public int MaxUsuario { get => LimiteMestres + LimiteAdministradores + LimiteVisualizadores; }

        private LimiteAcessos(int maxMestres, int maxAdministradores, int maxVisualizadores)
        {
            LimiteMestres = maxMestres;
            LimiteAdministradores = maxAdministradores;
            LimiteVisualizadores = maxVisualizadores;
        }
        public static LimiteAcessos Create() => new(2,5,5);
        public bool DisposicaoAcessos(int quantidadeUsuarios) => quantidadeUsuarios < MaxUsuario;
        public LimiteAcessos Alterar( int maxMestres, int maxAdministradores, int maxVisualizadores)
        {
            Validar(maxMestres, maxAdministradores, maxVisualizadores);
            return new(maxMestres,maxAdministradores,maxVisualizadores);
        }

        private static void Validar(int maxMestres,int maxAdministradores,int maxVisualizadores)
        {
            ContasUsuariosValidacao.Verifica(maxMestres < 1,"O número máximo de mestres deve ser maior que zero.");
            ContasUsuariosValidacao.Verifica(maxMestres > _limiteMestres,$"O número máximo de mestres é {_limiteMestres}.");
            ContasUsuariosValidacao.Verifica(maxAdministradores < 0,"O número máximo de administradores não pode ser negativo.");
            ContasUsuariosValidacao.Verifica(maxAdministradores > _limiteAdministradores,$"O número máximo de administradores é {_limiteAdministradores}.");
            ContasUsuariosValidacao.Verifica(maxVisualizadores < 0,"O número máximo de visualizadores não pode ser negativo.");
            ContasUsuariosValidacao.Verifica(maxVisualizadores > _limiteVisualizadores,$"O número máximo de visualizadores é {_limiteVisualizadores}.");
        }


    }
}
