using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AppsManager.DTO;

namespace BookingManager.Web.Models
{
    public class AgentsViewModel
    {
        public List<AgentModel> Agents { get; set; }
        public AgentModel NewUser { get; set; }
        public AgentModel SelectedAgent { get; set; }
        public AgentsViewModel() {
            Agents = new List<AgentModel>();
        }
    }

    public class AgentModel {
        [Required]
        [Display(Name = "Puede este agente utilizar el sistema?")]
        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre")]
        public string Firstname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Apellidos")]
        public string Lastname { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Repita la contraseña")]
        public string RetypedPassword { get; set; }

        public List<PrivilegeModel> Privileges { get; set; }
    }

    public class PrivilegeModel {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }
}