using System.Collections.Generic;
using System.Linq;
using Cycloid.Models;

namespace Cycloid.Managers
{
    public interface IProgramsManager
    {
        IList<Program> GetByChannelId(string channelId);
        IList<Program> GetByChannelId(string channelId, int skip, int take);
        Program GetById(string id);
        IEnumerable<IGrouping<string, Program>> GetAllGroupByChannel();
    }
}
