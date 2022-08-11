using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;
using static PlatformService.GrpcPlatform;

namespace CommandService.SyncDataServices.Grpc
{
    public class GrpcPlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public GrpcPlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Platform>?> GetAllPlatforms()
        {
            string address = _configuration["GrpcPlatform"];
            Console.WriteLine($"--> Calling GRPC service: {address}");

            var channel = GrpcChannel.ForAddress(address);
            var client = new GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = await client.GetAllPlatformsAsync(request);

                return _mapper.Map<List<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC service: {ex.Message}");
                return null;
            }
        }
    }
}
