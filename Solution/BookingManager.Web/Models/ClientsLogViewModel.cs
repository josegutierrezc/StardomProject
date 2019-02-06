using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingManager.Web.Models;
using Cars.DTO;

namespace BookingManager.Web.Models
{
    public class ClientsLogViewModel
    {
        public string AgencyNumber { get; set; }
        public long AssignToReservationId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public string SearchFor { get; set; }
        public int TotalResults { get; set; }
        public List<ClientDTO> Clients { get; set; }
        public ClientViewModel SelectedClient { get; set; }
    }

}