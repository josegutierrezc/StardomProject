using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Cars.DTO;

namespace BookingManager.Web.Models
{
    public class ClientViewModel
    {
        public string ReturnUrl { get; set; }
        public int Id { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }

        [Required(ErrorMessage = "El Nombre del cliente es requerido")]
        [Display(Name="Nombre:")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El Apellido del cliente es requerido")]
        [Display(Name = "Apellidos:")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El Teléfono del cliente es requerido")]
        [Display(Name = "Teléfono:")]
        public string Phone { get; set; }

        [Display(Name = "Correo electrónico:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La Fecha de Nacimiento del cliente es requerida")]
        [Display(Name = "Fecha de nacimiento:")]
        public string Birthday { get; set; }

        [Display(Name = "Notas:")]
        public string Notes { get; set; }

        [Display(Name = "Asociado a agencia:")]
        public Nullable<int> LinkedToAgencyId { get; set; }

        public string LinkedAgencyName { get; set; }
        public bool Troubled { get; set; }
        public string ClientSince { get; set; }
        public int TotalCarReservations { get; set; }
        public int TotalCarReservationDays { get; set; }
        public int AverageCarReservationDays { get; set; }
        public int AverageCarReservationsPerYear { get; set; }
        public string PreferedCarCategory { get; set; }
        public List<ClientActivityModel> Activities { get; set; }
    }
}