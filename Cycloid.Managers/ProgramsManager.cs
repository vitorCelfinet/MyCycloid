using System.Collections.Generic;
using System.Linq;
using Cycloid.Models;
using Cycloid.Services;

namespace Cycloid.Managers
{
    public class ProgramsManager : IProgramsManager
    {
        private readonly IProgramsService _programsService;

        public ProgramsManager(IProgramsService programsService)
        {
            _programsService = programsService;
        }

        public IList<Program> GetByChannelId(string channelId)
        {
            return _programsService.GetAll().Where(c=> c.ChannelId == channelId).ToList();
        }

        public IList<Program> GetByChannelId(string channelId, int skip, int take)
        {
            IList<Program> programs = _programsService.GetAll().Where(c => c.ChannelId == channelId).ToList();
            return programs.Skip(skip).Take(take).ToList();
        }

        public Program GetById(string id)
        {
            return _programsService.GetAll().FirstOrDefault(c=> c.Id.Equals(id));
        }

        public IEnumerable<IGrouping<string, Program>> GetAllGroupByChannel()
        {
            return _programsService.GetAll().GroupBy(c=> c.ChannelId);
        }
    }
}
