using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _appDbContext;

        public CommandRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> SaveChanges()
        {
            var result = await _appDbContext.SaveChangesAsync();
            return result >= 0;
        }

        // Platforms
        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            var platforms = await _appDbContext.Platforms.ToListAsync();
            return platforms;
        }

        public void CreatePlatform(Platform platform)
        { 
            _appDbContext.Platforms.Add(platform);
        }

        public async Task<bool> PlatformExists(int platformId)
        {
            var exists = await _appDbContext.Platforms.AnyAsync(p => p.Id == platformId);
            return exists;
        }

        public async Task<bool> ExternalPlatformExists(int externalPlatformId)
        {
            var exists = await _appDbContext.Platforms.AnyAsync(p => p.ExternalId == externalPlatformId);
            return exists;
        }

        // Commands
        public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId)
        { 
            var commands = await _appDbContext.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name)
                .ToListAsync();

            return commands;
        }

        public async Task<Command?> GetCommand(int platformId, int commandId)
        {
            var command = await _appDbContext.Commands
                .SingleOrDefaultAsync(c => 
                    c.PlatformId == platformId 
                    && c.Id == commandId
                );

            return command;
        }

        public void CreateCommand(int platformId, Command command)
        {
            command.PlatformId = platformId;
            _appDbContext.Commands.Add(command);
        }
    }
}
