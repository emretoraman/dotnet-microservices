using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using static PlatformService.GrpcPlatform;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
        }

        public override async Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var platforms = await _platformRepository.GetAllPlatforms();

            var response = new PlatformResponse();
            foreach (var platform in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

            return response;
        }
    }
}
