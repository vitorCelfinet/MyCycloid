using System;
using Cycloid.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cycloid.Extensions;

namespace Cycloid.Managers
{
    public class EventsManager : IEventsManager
    {
        private readonly IProgramsManager _programsManager;
        private readonly IChannelsManager _channelsManager;

        public EventsManager(IProgramsManager programsManager, IChannelsManager channelsManager)
        {
            _programsManager = programsManager;
            _channelsManager = channelsManager;
        }

        public Task<List<Event>> GetEventsAsync(string sessionId, string channelId,
            CancellationToken ct = default(CancellationToken))
        {
            return new TaskFactory().StartNew(() =>
            {
                var programsTask = new TaskFactory().StartNew(() =>
                             {
                                 var startTime = DateTime.UtcNow.Date.AddDays(-1);
                                 var endTime = DateTime.UtcNow.Date.AddDays(2).AddMilliseconds(-1);

                                 return _programsManager.GetByChannelId(channelId).Where(c =>
                                     c.StartTime.Between(startTime, endTime)
                                     || (c.StartTime < startTime && c.EndTime.Between(startTime, endTime)));
                             }, ct);

                var channelTask = new TaskFactory().StartNew(() => _channelsManager.GetById(channelId), ct);

                var subscribedTask = new TaskFactory().StartNew(() => _channelsManager.GetSubscribedChannelsBySessionId(sessionId), ct);
                
                var isSubscribed = subscribedTask.Result.Select(c=> c.Id).Contains(channelId);

                return programsTask.Result.Select(c => new Event
                {
                    ChannelName = channelTask.Result.Name,
                    IsSubscribed = isSubscribed,
                    ProgramTitle = c.Title,
                    ProgramDescription = c.Description,
                    ProgramStartTime = c.StartTime,
                    ProgramEndTime = c.EndTime
                }).ToList();
            }, ct);
        }

        public Task<List<Event>> GetPlayingEventsAsync(string deviceId, CancellationToken ct = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }
    }
}
