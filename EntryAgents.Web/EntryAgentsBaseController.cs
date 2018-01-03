using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using EntryAgents.Web.Interfaces;

namespace EntryAgents.Web
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class EntryAgentsBaseController : Controller
    {
        public new IEntryAgentsUser User
        {
            get
            {
                var claimsPrincipal = HttpContext.User as ClaimsPrincipal;
                if (claimsPrincipal == null) return null;

                return new EntryAgentsUser(claimsPrincipal);
            }
        }
    }
}
