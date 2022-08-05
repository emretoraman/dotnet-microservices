using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _appDbContext;

        public PlatformRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> SaveChanges()
        {
            var result = await _appDbContext.SaveChangesAsync();
            return result >= 0;
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            var platforms = await _appDbContext.Platforms.ToListAsync();
            return platforms;
        }

        public async Task<Platform?> GetPlatformById(int id)
        {
            var platform = await _appDbContext.Platforms.SingleOrDefaultAsync(p => p.Id == id);
            return platform;
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _appDbContext.Platforms.Add(platform);
        }
    }
}
