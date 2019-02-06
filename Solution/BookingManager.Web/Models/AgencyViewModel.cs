using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cars.DTO;

namespace BookingManager.Web.Models
{
    public class AgenciesViewModel {
        public List<LinkedAgencyDTO> LinkedAgencies { get; set; }
        public string NewName { get; set; }
        public string NewContact { get; set; }
        public string NewPhone { get; set; }
        public string NewEmail { get; set; }
        public string ErrorDescription { get; set; }
    }
}