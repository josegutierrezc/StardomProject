using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        // GET: Agents
        public ActionResult Index()
        {
            return View();
        }
    }
}