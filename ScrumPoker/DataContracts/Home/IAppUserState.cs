using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumPoker.DataContracts.Home
{
    public interface IAppUserState
    {
        string UserName { get; set; }

        bool IsAdmin { get; set; }

        ITeamListItem[] Teams { get; set; }

        IFacilitatorListItem[] Facilitators { get; set; }
    }
}