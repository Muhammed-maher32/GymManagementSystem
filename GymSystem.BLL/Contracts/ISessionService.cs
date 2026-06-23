using GymSystem.BLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Contracts
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel?>> GetAllSessionsAsync(CancellationToken ct);
    }
}
