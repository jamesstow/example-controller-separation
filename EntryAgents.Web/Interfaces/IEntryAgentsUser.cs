using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryAgents.Web.Interfaces
{
    public interface IEntryAgentsUser
    {
        string UserName { get; }
        ClaimsPrinciple Principle { get; }
    }
}
