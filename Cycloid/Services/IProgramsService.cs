using System.Collections.Generic;
using Cycloid.Models;

namespace Cycloid.Services
{
    public interface IProgramsService
    {
        IList<Program> GetByChannelId(string channelId);
        Program GetById(string id);
    }
}
