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
        public IList<Program> GetByChannelId(string channelId, int skip, int take)
        {
            IList<Program> programs = _programsService.GetByChannelId(channelId);
            return programs.Skip(skip).Take(take).ToList();
        }

        public Program GetById(string id)
        {
            return _programsService.GetById(id);
        }
    }
}
