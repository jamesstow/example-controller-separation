using EntryAgents.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryAgents.Web.Services
{
    public class AgentControllerService : IAgentControllerService
    {
        private readonly IEntryAgentRepository _repository;
        private readonly IAgentStudentService _agentStudentService;
        private readonly IAgentManager _agentManager;

        private readonly UserManager<Agent> _userManager;
        private readonly SignInManager<Agent> _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHostingEnvironment _environment;

        public AgentControllerService(IEntryAgentRepository repository,
                                IAgentStudentService agentStudentService,
                                IAgentManager agentManager,
                                UserManager<Agent> userManager,
                                SignInManager<Agent> signInManager,
                                IAuthorizationService authorizationService,
                                IHostingEnvironment environment)
        {
            _repository = repository;
            _agentStudentService = agentStudentService;
            _agentManager = agentManager;

            _userManager = userManager;
            _authorizationService = authorizationService;
            _environment = environment;
            _signInManager = signInManager;
        }

        public async Task<Tuple<AgentViewModel, bool>> GetApproveDisapprove(IEntryAgentsUser user, string userName)
        {
            var agent = await _repository.AgentData.GetByUserName(userName);
            var isApproved = await _repository.AgentIdentity.ApproveDisapprove(agent);

            return new Tuple<AgentViewModel, bool>(agent, isApproved);
        }

        public async Task<Tuple<AgentViewModel, bool>> GetBlockUnblock(IEntryAgentsUser user, string userName)
        {
            var agent = await _repository.AgentData.GetByUserName(userName);
			var isBlocked = await _repository.AgentIdentity.BlockUnblock(agent);

            return new Tuple<AgentViewModel, bool>(agent, isBlocked);
        }

        public async Task<AgentDetailsViewModel> GetDetailsModel(IEntryAgentsUser user, string userName)
        {
            var agent = await _repository.AgentData.GetByUserName(userName);
            agent.Applications = await _repository.ApplicationData.GetAgentApplications(agent).ToListAsync();

            var result = await agent.ToDetailsViewModel(_userManager);

            return result;
        }

        public async Task<AgentSetupAccountViewModel> GetSetupAccountViewModel(IEntryAgentsUser user)
        {
            var result = await _agentManager.GetSetupAccountViewModel(user.UserName);

            return result;
        }

        public async Task<AgentSetupAccountViewModel> GetUpdateModel(string userName)
        {
            var result = await _agentManager.GetSetupAccountViewModel(userName);

            return result;
        }

        public async Task<List<AgentViewModel>> List(IEntryAgentsUser user, string searchTerm, int agentPage)
        {
            var pagedResult = await _repository.AgentData
                                               .GetAll()
                                               .FilterByNameOrEmail(searchTerm)
                                               .TakePageAsync(agentPage, EntryAgentsConfig.DefaultPageSize);

            var result = (await pagedResult.items.ToDetailsViewModel(_userManager))
                                .ToPagedListViewModel(agentPage, EntryAgentsConfig.DefaultPageSize, pagedResult.count);

        }

        public async Task<List<StudentViewModel>> SearchStudents(IEntryAgentsUser user, string searchTerm, string applicationStatus, int studentPage)
        {
            var agent = await _userManager.GetUserAsync(user.Principle);

            var agentStudents = await _agentStudentService.GetAgentStudents(agent);
            var filteredStudents =
                await agentStudents.FilterByApplicationStatus(applicationStatus)
                                   .FilterByNameOrEmail(searchTerm)
                                   .GetLastTen()
                                   .ToListAsync();

            var result = filteredStudents.ToStudentDetailsViewModels(_environment);
        }

        public async Task SetupAccount(IEntryAgentsUser user, AgentSetupAccountViewModel agentSetupAccountViewModel)
        {
            var agent = await _agentManager.Edit(user.UserName, agentSetupAccountViewModel);

            // TODO: update email after verification
            // TODO: update username
            // https://stackoverflow.com/questions/36367140/aspnet-core-generate-and-change-email-address
            if (agent.Email != agentSetupAccountViewModel.Email)
            {
                agent.Email = agentSetupAccountViewModel.Email;
                agent.UserName = agentSetupAccountViewModel.Email;
                await _userManager.UpdateAsync(agent);

                await _signInManager.RefreshSignInAsync(agent);
                //await _signInManager.SignInAsync(agent, true);
                //var token = await _userManager.GenerateChangeEmailTokenAsync(agent, agentSetupAccountViewModel.Email);
                //await _userManager.ChangeEmailAsync(agent, agentSetupAccountViewModel.Email, token);
            }
        }

        public async Task Update(IEntryAgentsUser user, string userName, AgentSetupAccountViewModel agentSetupAccountViewModel)
        {
            await _agentManager.Edit(userName, agentSetupAccountViewModel);
        }
    }
}
