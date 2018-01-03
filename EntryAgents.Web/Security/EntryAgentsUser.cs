using EntryAgents.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EntryAgents.Web.Security
{
    public class EntryAgentsUser : IEntryAgentsUser
    {
        private readonly ClaimsPrincipal _claimsPrinciple;

        public EntryAgentsUser(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrinciple = claimsPrincipal;
        }

        public string UserName => _claimsPrinciple.Identity.Name; // or similar

        public ClaimsPrincipal Principle => _claimsPrinciple;
    }
}
