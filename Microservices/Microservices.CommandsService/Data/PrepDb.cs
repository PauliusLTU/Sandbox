using Microservices.CommandsService.Models;
using Microservices.CommandsService.SyncDataServices.Grpc;

namespace Microservices.CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulution(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                IPlatformDataClient grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                IEnumerable<Platform> platforms = grpcClient.ReturnAllPlatforms();

                ICommandRepo repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();

                SeedData(repo, platforms);
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("->> Seeding new platforms...");

            foreach (Platform platform in platforms)
            {
                if (!repo.ExternalPlatformExits(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                }
            }

            repo.SaveChanges();
        }
    }
}
