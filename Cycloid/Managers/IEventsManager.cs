using Cycloid.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cycloid.Managers
{
    public interface IEventsManager
    {
        /// <summary>
        /// Gets the events by channel id
        /// </summary>
        /// <param name="sessionId">The session id</param>
        /// <param name="channelId">The channel id</param>
        /// <param name="ct">(optional) The cancellation token</param>
        /// <returns>The events</returns>
        Task<List<Event>> GetEventsAsync(string sessionId, string channelId, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// Gets all events playing at the moment
        /// </summary>
        /// <param name="sessionId">The session Id</param>
        /// <param name="ct">(optional) The cancellation token</param>
        /// <returns></returns>
        Task<List<Event>> GetPlayingEventsAsync(string sessionId, CancellationToken ct = default(CancellationToken));
    }
}
