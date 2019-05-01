using System.Collections.Generic;
using System.Linq;
using Cycloid.Models;
using Cycloid.Repositories;
using Cycloid.Services;

namespace Cycloid.Managers
{
    public class ChannelsManager : IChannelsManager
    {
        private readonly IChannelsService _channelsService;
        private readonly IDevicesRepository _devicesRepository;

        public ChannelsManager(IChannelsService channelsService, IDevicesRepository devicesRepository)
        {
            _channelsService = channelsService;
            _devicesRepository = devicesRepository;
        }
        public IEnumerable<Channel> GetAllChannels()
        {
            return _channelsService.GetChannels();
        }

        public IEnumerable<Channel> GetSubscribedChannelsBySessionId(string sessionId)
        {
            var device = _devicesRepository.GetDevice(sessionId);
            
            var channelsIds = _channelsService.GetSubscribedChannelsByDeviceId(device.Id);

            return GetByIds(channelsIds);
        }

        public Channel GetById(string channelId)
        {
            return GetAllChannels().FirstOrDefault(c => c.Id.Equals(channelId));
        }

        private IEnumerable<Channel> GetByIds(IEnumerable<string> channelsIds)
        {
            return GetAllChannels().Where(c => channelsIds.Contains(c.Id));
        }
    }
}
