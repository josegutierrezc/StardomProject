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
    public class AgenciesController : Controller
    {
        // GET: Agencies
        public async Task<ActionResult> Index()
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            AgenciesViewModel model = new AgenciesViewModel();
            model.ErrorDescription = string.Empty;
            model.LinkedAgencies = await Client.Instance.GetAllLinkedAgencies(agencyNumber);
            
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Add(string Name, string ContactName, string Phone, string Email) {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            AgenciesViewModel model = new AgenciesViewModel();
            model.NewName = Name;
            model.NewContact = ContactName;
            model.NewPhone = Phone;
            model.NewEmail = Email;

            if (Name == null || Name.Trim() == string.Empty)
                model.ErrorDescription = "El nombre de la agencia no puede ser nulo o vacío.";
            else if (ContactName == null || ContactName.Trim() == string.Empty)
                model.ErrorDescription = "El nombre del contacto no puede ser nulo o vacío.";
            else if (Phone == null || Phone.Trim() == string.Empty)
                model.ErrorDescription = "El teléfono no puede ser nulo o vacío.";
            else {
                LinkedAgencyDTO entity = new LinkedAgencyDTO();
                entity.Name = Name;
                entity.ContactName = ContactName;
                entity.Phone = Phone;
                entity.Email = Email;
                int id = await Client.Instance.AddLinkedAgency(agencyNumber, entity);

                if (id == -1) model.ErrorDescription = "No se pudo adicionar esta agencia. Por favor desconecte su sesión y conéctese nuevamente.";
                else
                {
                    model.NewName = string.Empty;
                    model.NewContact = string.Empty;
                    model.NewPhone = string.Empty;
                    model.NewEmail = string.Empty;
                }
            }

            model.LinkedAgencies = await Client.Instance.GetAllLinkedAgencies(agencyNumber);
            return PartialView("_Agencies", model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(int Id, string Name, string ContactName, string Phone, string Email)
        {
            string agencyNumber = ApplicationHelper.Instance.GetTagValueFromIdentity(User.Identity, ApplicationHelper.AgencyNumberTagName);

            AgenciesViewModel model = new AgenciesViewModel();
            model.NewName = Name;
            model.NewContact = ContactName;
            model.NewPhone = Phone;
            model.NewEmail = Email;

            if (Name == null || Name.Trim() == string.Empty)
                model.ErrorDescription = "El nombre de la agencia no puede ser nulo o vacío.";
            else if (ContactName == null || ContactName.Trim() == string.Empty)
                model.ErrorDescription = "El nombre del contacto no puede ser nulo o vacío.";
            else if (Phone == null || Phone.Trim() == string.Empty)
                model.ErrorDescription = "El teléfono no puede ser nulo o vacío.";
            else
            {
                LinkedAgencyDTO entity = new LinkedAgencyDTO();
                entity.Id = Id;
                entity.Name = Name;
                entity.ContactName = ContactName;
                entity.Phone = Phone;
                entity.Email = Email;
                bool updated = await Client.Instance.UpdateLinkedAgency(agencyNumber, entity);

                if (!updated) model.ErrorDescription = "No se pudo modificar esta agencia. Por favor desconecte su sesión y conéctese nuevamente.";
                else
                {
                    model.NewName = string.Empty;
                    model.NewContact = string.Empty;
                    model.NewPhone = string.Empty;
                    model.NewEmail = string.Empty;
                }
            }

            model.LinkedAgencies = await Client.Instance.GetAllLinkedAgencies(agencyNumber);
            return PartialView("_Agencies", model);
        }
    }
}