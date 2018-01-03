using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryAgents.Web.Interfaces
{
    public interface IAgentControllerService
    {
        Task<List<StudentViewModel>> SearchStudents(IEntryAgentsUser user, string searchTerm, string applicationStatus, int studentPage);
        Task<AgentSetupAccountViewModel> GetSetupAccountViewModel(IEntryAgentsUser user);
        Task SetupAccount(IEntryAgentsUser user, AgentSetupAccountViewModel agentSetupAccountViewModel);
        Task<Lsit<AgentViewModel>> List(IEntryAgentsUser user, string searchTerm, int agentPage);
        Task<AgentSetupAccountViewModel > GetUpdateModel(string userName);
        Task Update(IEntryAgentsUser user, string userName, AgentSetupAccountViewModel agentSetupAccountViewModel);
        Task<AgentDetailsViewModel> GetDetailsModel(IEntryAgentsUser user, string userName);
        Task<Tuple<AgentViewModel, bool>> GetApproveDisapprove(IEntryAgentsUser user, string userName);
        Task<Tuple<AgentViewModel, bool>> GetBlockUnblock(IEntryAgentsUser user, string userName);
    }
}
