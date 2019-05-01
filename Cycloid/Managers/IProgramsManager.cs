using System.Collections.Generic;
using Cycloid.Models;

namespace Cycloid.Managers
{
    public interface IProgramsManager
    {
        IList<Program> GetByChannelId(string channelId, int skip, int take);
        Program GetById(string id);
    }
}
