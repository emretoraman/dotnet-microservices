using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static async Task PrepPopulation(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
            var platforms = await grpcClient.GetAllPlatforms();

            var commandRepository = serviceScope.ServiceProvider.GetRequiredService<ICommandRepository>();

            await SeedData(commandRepository, platforms!);
        }

        private static async Task SeedData(ICommandRepository commandRepository, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var platform in platforms)
            {
                var exists = await commandRepository.ExternalPlatformExists(platform.ExternalId);
                if (!exists)
                {
                    commandRepository.CreatePlatform(platform);
                }
            }
            await commandRepository.SaveChanges();
        }
    }
}