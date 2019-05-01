using System.Collections.Generic;
using Cycloid.Models;

namespace Cycloid.Services
{
    public interface IChannelsService
    {
        IEnumerable<Channel> GetChannels();
        IEnumerable<string> GetSubscribedChannelsByDeviceId(string deviceId);
    }
}
