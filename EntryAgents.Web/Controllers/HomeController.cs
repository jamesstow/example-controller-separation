using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EntryAgents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using EntryAgents.Web.Security;
using EntryAgents.Web.Interfaces;

namespace EntryAgents.Web.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = nameof(Role.Admin) + "," + nameof(Role.Agent))]
    public class AgentController : EntryAgentsBaseController
    {
        private readonly IAgentControllerService _service;

        public AgentController(IAgentControllerService service)
        {
            _service = service;
        }

        // GET: Agents
        public async Task<IActionResult> Index(string searchTerm = null, string applicationStatus = null, int studentPage = 1)
        {
            ViewBag.SearchTerm = searchTerm;
            ViewBag.ApplicationStatus = applicationStatus;

            var result = _service.SearchStudents(User, searchTerm,applicationStatus,studentPage);

            return View(result);
        }

        // Get: Agent/SetupAccount
        [Route("SetupAccount")]
        public async Task<IActionResult> SetupAccount()
        {
            var result = await _service.GetSetupAccountViewModel(User);

            return View(result);
        }

        // Post: Agent/SetupAccount
        [HttpPost]
        [Route("SetupAccount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupAccount(AgentSetupAccountViewModel agentSetupAccountViewModel)
        {
            if (ModelState.IsValid)
            {
                await _service.SetupAccount(User, agentSetupAccountViewModel);

                return RedirectToAction("RequestPayment", "Payment");
            }

            return View(agentSetupAccountViewModel);
        }

        // GET: /Agent/Referral/
        [Route("/Agent/Referral/")]
        public IActionResult Referral()
        {
            return View();
        }

        // Get: Admin/Agent/List
        [Route("/Admin/Agent/List")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> List(string searchTerm = null, int agentPage = 1)
        {
            var result = await _service.List(User, searchTerm, agentPage);

            return View(result);
        }

        // Get: /Admin/Agent/Edit/first@gmail.com
        [Route("/Admin/Agent/Edit/{userName}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> Edit(string userName)
        {
            var result = await _service.GetUpdateModel(userName);

            return View("SetupAccount", result);
        }

        // Post: /Admin/Agent/Edit/first@gmail.com
        [HttpPost]
        [Route("/Admin/Agent/Edit/{userName}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> Edit(string userName, AgentSetupAccountViewModel agentSetupAccountViewModel)
        {
            IActionResult result = View("SetupAccount", agentSetupAccountViewModel);

            if (ModelState.IsValid)
            {
                await _service.Update(User, userName, agentSetupAccountViewModel);
                result = RedirectToAction("List", "Agent");
            }

            return result;
        }

        // GET: /Admin/Agent/Details/first@gmail.com
        [Route("/Admin/Agent/Details/{username}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> Details(string userName)
        {
            var result = await _service.GetDetailsModel(User, userName);
            return View(result);
        }

        // GET: /Admin/Agent/ApproveDisapprove/first@gmail.com
        [Route("/Admin/Agent/ApproveDisapprove/{username}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> ApproveDisapprove(string userName)
        {
            var model = await _service.GetApproveDisapprove(User, userName);

            var agent = model.Item1;
            var isApproved = model.Item2; // hack, obviously, this needs its own viewmodel really

            ViewResult result;

            if (isApproved)
            {
                result = View("ApproveSuccess", agent);
            }
            else
            {
                result = View("DisapproveSuccess", agent);
            }

            return result;
        }

        // GET: /Admin/Agent/BlockUnblock/first@gmail.com
        [Route("/Admin/Agent/BlockUnblock/{username}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> BlockUnblock(string userName)
        {
            var model = await _service.GetBlockUnblock(User, userName);

            var agent = model.Item1;
            var isBlocked = model.Item2; // hack, obviously, this needs its own viewmodel really

            ViewResult result;

            if (isBlocked)
            {
                result = View("BlockSuccess", agent);
            }
            else
            {
                result = View("UnblockSuccess", agent);
            }

            return result;
        }
    }
}
