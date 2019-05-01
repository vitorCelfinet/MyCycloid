using System;
using System.Collections.Concurrent;
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

        public Task<List<Event>> GetEventsAsync(string sessionId, string channelId, CancellationToken ct = default(CancellationToken))
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

        public Task<List<Event>> GetPlayingEventsAsync(string sessionId, CancellationToken ct = default(CancellationToken))
        {
            var channelsSubscribed = new TaskFactory().StartNew(()=> _channelsManager.GetSubscribedChannelsBySessionId(sessionId), ct);
            return GetPreviousCurrentNextEvents(channelsSubscribed, ct);
        }

        private Task<List<Event>> GetPreviousCurrentNextEvents(Task<IEnumerable<Channel>> channelsSubscribed, CancellationToken ct)
        {
            return new TaskFactory().StartNew(() =>
            {
                var eventList = new ConcurrentBag<Event>();

                Parallel.ForEach(_programsManager.GetAllGroupByChannel(), channelGroup =>
                {
                    var channelName = _channelsManager.GetById(channelGroup.Key).Name;
                    var isSubscribed = channelsSubscribed.Result.Select(c => c.Id).Contains(channelGroup.Key);
                    using (var iter = channelGroup.OrderBy(c => c.StartTime).GetEnumerator())
                    {
                        Event previous = null;
                        while (iter.MoveNext())
                        {
                            var program = iter.Current;
                            if (DateTime.UtcNow.Between(program.StartTime, program.EndTime))
                            {
                                if (previous != null) eventList.Add(previous);
                                eventList.Add(ProgramToEvent(program, channelName, isSubscribed));
                                if (iter.MoveNext()) eventList.Add(ProgramToEvent(iter.Current, channelName, isSubscribed));
                                break;
                            }
                            previous = ProgramToEvent(iter.Current, channelName, isSubscribed);
                        }
                    }
                });
                return eventList.OrderBy(c=>c.ChannelName).ThenBy(c=> c.ProgramStartTime).ToList();
            }, ct);
        }

        private Event ProgramToEvent(Program program, string channelName, bool isSubscribed)
        {
            return new Event
            {
                ProgramTitle = program.Title,
                ProgramDescription = program.Description,
                ProgramStartTime = program.StartTime,
                ProgramEndTime = program.EndTime,
                ChannelName = channelName,
                IsSubscribed = isSubscribed
            };
        }
    }
}
