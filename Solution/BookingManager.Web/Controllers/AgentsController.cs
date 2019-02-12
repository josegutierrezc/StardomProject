using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AppsManager.DTO;
using Cars.DTO;
using BookingManager.Web.Helpers;
using BookingManager.Web.Models;
using Cars.REST.Client;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        // GET: Agents
        public async Task<ActionResult> Index()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            AgentsViewModel model = new AgentsViewModel();
            foreach (UserDTO user in await Client.Instance.GetAgencyUsers(agencyNumber)) {
                if (user.Username.ToUpper() == "SYSADMIN") continue;
                AgentModel agent = new AgentModel();
                agent.IsActive = user.IsActive;
                agent.Firstname = user.FirstName;
                agent.Lastname = user.LastName;
                agent.Username = user.Username;
                agent.Password = string.Empty;
                model.Agents.Add(agent);
            }

            return View(model);
        }

        public async Task<ActionResult> GetDetails(string Username)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            UserDTO agent = await Client.Instance.GetUser(Username);

            AgentsViewModel model = new AgentsViewModel();
            model.SelectedAgent = new AgentModel() {
                IsActive = agent.IsActive,
                Firstname = agent.FirstName,
                Lastname = agent.LastName,
                Username = agent.Username
            };

            List<string> userPrivilegeName = new List<string>();
            foreach (PrivilegeDTO priv in await Client.Instance.GetUserPrivileges(agencyNumber, Username))
                userPrivilegeName.Add(priv.Name);

            model.SelectedAgent.Privileges = new List<PrivilegeModel>();
            foreach (PrivilegeDTO p in await Client.Instance.GetAllPrivileges(agencyNumber)) {
                if (p.Name.ToUpper() == "SYSADMIN") continue;
                PrivilegeModel m = new PrivilegeModel();
                m.Name = p.Name;
                m.Description = p.Description;
                m.IsSelected = userPrivilegeName.Contains(p.Name);
                model.SelectedAgent.Privileges.Add(m);
            }
            
            return PartialView("_AgentDetails", model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePrivilege(string Username, string PrivilegeName, bool IsSelected) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            bool setted = await Client.Instance.SetUserPrivilege(agencyNumber, Username, PrivilegeName, !IsSelected);
            return Json(new { Success = setted });
        }
    }
}