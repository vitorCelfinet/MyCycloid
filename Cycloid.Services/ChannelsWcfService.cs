using System.Collections.Generic;
using System.Linq;
using Cycloid.Models;

namespace Cycloid.Services
{
    public class ChannelsWcfService : IChannelsService
    {
        public IEnumerable<Channel> GetChannels()
        {
            var client = new ChannelsService.ServiceClient();
           
            var channelsWcf = client.GetChannels();

            return channelsWcf.Select(c => new Channel
            {
                Id = c.Id,
                Name = c.Name,
                Position = c.Position
            });
        }

        public IEnumerable<string> GetSubscribedChannelsByDeviceId(string deviceId)
        {
            var client = new ChannelsService.ServiceClient();
            var channels = client.GetSubscribedChannelIds(deviceId);
            return channels.ToList();
        }
    }
}
