using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingManager.Web.Models
{
    public class ReportsViewModel
    {
        public SelectList Reports { get; set; }
        public SelectList Formats { get; set; }
        public SelectList TourOperators { get; set; }
        public SelectList PaymentsStatuses { get; set; }
        public SelectList CarReservationStatuses { get; set; }
        public int SelectedReportId { get; set; }
        public ReportViewModel SelectedReport { get; set; }
    }

    public class ReportViewModel {
        public int ReportId { get; set; }
        public string ReportName { get; set; }
        public bool IsFromDateEnabled { get; set; }
        public bool IsToDateEnabled { get; set; }
        public bool IsTourOperatorEnabled { get; set; }
        public bool IsPaymentStatusEnabled { get; set; }
        public bool IsAllStatusesEnabled { get; set; }

        [Display(Name = "Formato de exportación:")]
        public string Format { get; set; }
        [Display(Name = "Desde:")]
        public string FromDate { get; set; }
        [Display(Name = "Hasta:")]
        public string ToDate { get; set; }
        [Display(Name = "Tour Operador:")]
        public int TourOperatorId { get; set; }
        [Display(Name = "Estado de los pagos:")]
        public int PaymentStatusId { get; set; }
        [Display(Name = "Estado de la reserva")]
        public string CarReservationStatus { get; set; }
    }
}