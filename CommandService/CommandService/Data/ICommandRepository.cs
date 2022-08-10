using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepository
    {
        Task<bool> SaveChanges();

        // Platforms
        Task<IEnumerable<Platform>> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        Task<bool> PlatformExists(int platformId);
        Task<bool> ExternalPlatformExists(int externalPlatformId);

        // Commands
        Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId);
        Task<Command?> GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}
