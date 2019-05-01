using System.Collections.Generic;
using Cycloid.Models;

namespace Cycloid.Managers
{
    public interface IChannelsManager
    {
        IEnumerable<Channel> GetAllChannels();
        IEnumerable<Channel> GetSubscribedChannelsBySessionId(string sessionId);
        Channel GetById(string channelId);
    }
}
