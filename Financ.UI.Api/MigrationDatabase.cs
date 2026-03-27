using Financ.Infra.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            using (var appContext = scope.ServiceProvider.GetRequiredService<AppContextoData>()) // Substitua ApplicationDbContext pelo nome do seu DbContext
            {
                try
                {
                    appContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Logar o erro, ou lidar com ele de forma apropriada
                    Console.WriteLine($"Erro ao aplicar migrations: {ex.Message}");
                    throw; // Re-lançar a exceção para falhar a inicialização do host se as migrations não puderem ser aplicadas
                }
            }
        }
        return host;
    }
}