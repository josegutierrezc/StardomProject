using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cars.DTO;
using BookingManager.Web.Models;
using BookingManager.Web.Helpers;
using Cars.REST.Client;
using System.Threading.Tasks;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            KeyValuePair<int, List<CarReservationDTO>> result = await Client.Instance.GetCarReservations(agencyNumber, "active", 1000, 1, string.Empty, "UNCONFIRMED");

            HomeViewModel model = new HomeViewModel();
            model.CanSeePaymentsChart = User.IsInRole("sysadmin") | User.IsInRole("accountant");
            model.TotalUnconfirmedCarReservations = result.Key;

            var ci = new System.Globalization.CultureInfo("es-ES");
            model.CarReservationsPendingForConfirmation = new List<UnconfirmedCarReservation>();
            foreach (CarReservationDTO res in result.Value) {
                string fromMonth = res.FromDate.ToString("MMM dd", ci);
                string toMonth = res.ToDate.ToString("MMM dd", ci);

                fromMonth = fromMonth.First().ToString().ToUpper() + fromMonth.Substring(1);
                toMonth = toMonth.First().ToString().ToUpper() + toMonth.Substring(1);
                model.CarReservationsPendingForConfirmation.Add(new UnconfirmedCarReservation()
                {
                    CarReservationId = res.Id,
                    ClientFullname = res.ClientFirstname + " " + res.ClientLastname,
                    ClientPhone = ApplicationHelper.Instance.FormatPhoneNumber(res.ClientPhone, false).Value,
                    CarCategoryName = res.CarCategoryName,
                    FromDate = fromMonth,
                    ToDate = toMonth,
                    Days = res.Days
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GetStatistics(string DataName) {
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            if (DataName.ToLower() == "carreservationsperagent") {
                //Get Month name in spanish
                var ci = new System.Globalization.CultureInfo("es-ES");
                string mn = DateTime.Now.ToString("MMM", ci).ToUpper();

                //Get Statistics
                CarReservationsPerAgentDTO data = await Client.Instance.GetCarReservationsPerAgentStatistics(agencyNumber, username, DateTime.Now.Year - 1, DateTime.Now.Year, DateTime.Now.Month);
                string[] labels = new string[10] { mn + " 1", mn + " 3", mn + " 6", mn + " 9", mn + " 12", mn + " 15", mn + " 18", mn + " 21", mn + " 24", mn + " 27" };
                return Json(new { Success = true, Labels = labels, FirstYear = data.FirstYear, SecondYear = data.SecondYear, FirstYearData = data.FirstYearReservations, SecondYearData = data.SecondYearReservations });
            }
            if (DataName.ToLower() == "paymentsbydate") {
                //Get Month name in spanish
                var ci = new System.Globalization.CultureInfo("es-ES");
                string mn = DateTime.Now.ToString("MMM", ci).ToUpper();
                DateTime fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day - 4, 0, 0, 0);
                int day = fromDate.Day;

                PaymentsPerPeriodDTO data = await Client.Instance.GetPaymentsPerPeriodStatistics(agencyNumber, fromDate, 7, DateTime.Today.Year - 1);
                string[] labels = new string[7] { mn + " " + day, mn + " " + (day + 1), mn + " " + (day + 2) , mn + " " + (day + 3), mn + " " + (day + 4), mn + " " + (day + 5), mn + " " + (day + 6)};
                return Json(new { Success = true, Labels = labels, FromYear = data.FromDate.Year, TotalDays = data.TotalDays, ComparedWithYear = data.ComparedWithYear, FirstYearData = data.TotalPaidFirstYear, SecondYearData = data.TotalPaidSecondYear });
            }
            return Json(new { Success = false });
        }
    }
}