using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingManager.Web.Models;
using BookingManager.Web.Helpers;
using Cars.REST.Client;
using Cars.DTO;
using System.Threading.Tasks;

namespace BookingManager.Web.Controllers
{
    [Authorize]
    public class CarPriceConfigurationController : Controller
    {
        public async Task<ActionResult> Index()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            PriceConfigurationViewModel model = new PriceConfigurationViewModel();

            ViewData["TourOperators"] = new SelectList((await Client.Instance.GetTourOperators(agencyNumber, true)).Where(e => e.IsActive).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            ViewData["Seasons"] = new SelectList((await Client.Instance.GetAllSeasons(agencyNumber)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.Name)
            }).ToList(), "Value", "Text");

            model.Columns = new Dictionary<int, KeyValuePair<int, PriceColumnModel>>();
            model.Rows = new Dictionary<int, KeyValuePair<int, PriceRowModel>>();
            model.Data = new List<List<PriceDataModel>>();
            
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Get(int TourOperatorId, int SeasonId) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            PriceConfigurationViewModel model = new PriceConfigurationViewModel();
            if (TourOperatorId != -1 & SeasonId != -1)
            {
                //Load ReservationDays

                int column = 0;
                model.Columns = new Dictionary<int, KeyValuePair<int, PriceColumnModel>>();
                foreach (ReservationDayDTO r in await Client.Instance.GetAllReservationDays(agencyNumber))
                {
                    if (!r.IsActive) continue;
                    PriceColumnModel pmodel = new PriceColumnModel();
                    pmodel.ReservationDayId = r.Id;
                    pmodel.Description = "De " + r.FromDay + " a " + r.ToDay + " días";
                    model.Columns.Add(r.Id, new KeyValuePair<int, PriceColumnModel>(column, pmodel));
                    column += 1;
                }

                //Load Categories
                int row = 0;
                model.Rows = new Dictionary<int, KeyValuePair<int, PriceRowModel>>();
                foreach (CarCategoryDTO c in await Client.Instance.GetTourOperatorCarCategories(agencyNumber, TourOperatorId))
                {
                    if (model.Rows.ContainsKey(c.Id)) continue;

                    PriceRowModel rmodel = new PriceRowModel();
                    rmodel.CarCategoryId = c.Id;
                    rmodel.Description = c.Name;

                    model.Rows.Add(c.Id, new KeyValuePair<int, PriceRowModel>(row, rmodel));
                    row += 1;
                }

                //Initialize Price Configuration List
                model.Data = new List<List<PriceDataModel>>();
                for (int drow = 0; drow <= model.Rows.Count() - 1; drow++)
                {
                    model.Data.Add(new List<PriceDataModel>());
                    for (int dcolumn = 0; dcolumn <= model.Columns.Count() - 1; dcolumn++)
                        model.Data[drow].Add(new PriceDataModel() { CarCategoryId = model.Rows.ElementAt(drow).Key, ReservationDayId = model.Columns.ElementAt(dcolumn).Key, CostPrice = null, SalePrice = null });
                }

                //Load Data
                foreach (PriceConfigurationDTO pc in await Client.Instance.GetPriceConfigurationByTourOperatorAndSeason(agencyNumber, TourOperatorId, SeasonId))
                {
                    if (model.Rows.ContainsKey(pc.CarCategoryId) && model.Columns.ContainsKey(pc.ReservationDayId))
                    {
                        row = model.Rows[pc.CarCategoryId].Key;
                        column = model.Columns[pc.ReservationDayId].Key;
                        model.Data[row][column].CostPrice = pc.CostPrice;
                        model.Data[row][column].SalePrice = pc.SalePrice;
                    }
                }
            }
            else {
                model.Columns = new Dictionary<int, KeyValuePair<int, PriceColumnModel>>();
                model.Rows = new Dictionary<int, KeyValuePair<int, PriceRowModel>>();
                model.Data = new List<List<PriceDataModel>>();
            }

            return PartialView("_PriceConfiguration", model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(PriceConfigurationViewModel Model) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);
            string username = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.UsernameTagName);

            List<PriceConfigurationDTO> configuration = new List<PriceConfigurationDTO>();
            foreach (List<PriceDataModel>dataRow in Model.Data) {
                foreach (PriceDataModel data in dataRow)
                {
                    if (data.CostPrice == null | data.SalePrice == null) continue;
                    if ((double)data.CostPrice == 0 | (double)data.SalePrice == 0) continue;
                    configuration.Add(new PriceConfigurationDTO()
                    {
                        TourOperatorId = Model.TourOperatorId,
                        SeasonId = Model.SeasonId,
                        CarCategoryId = data.CarCategoryId,
                        ReservationDayId = data.ReservationDayId,
                        CostPrice = (double)data.CostPrice,
                        SalePrice = (double)data.SalePrice
                    });
                }
            }

            bool update = await Client.Instance.UpdatePriceConfiguration(agencyNumber, Model.TourOperatorId, Model.SeasonId, username, configuration);

            return Json(new { Success = update });
        }
    }
}