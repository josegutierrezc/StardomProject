using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Cars.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedByUserId { get; set; }
        public string CreatedByUsername { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedByUserId { get; set; }
        public string ModifiedByUsername { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Notes { get; set; }
        public Nullable<int> LinkedToAgencyId { get; set; }
        public string LinkedAgencyName { get; set; }
        public bool Troubled { get; set; }
    }

    public class ClientDTOValidator : AbstractValidator<ClientDTO>
    {
        public ClientDTOValidator()
        {
            RuleFor(e => e.FirstName).NotNull().NotEmpty();
            RuleFor(e => e.LastName).NotNull().NotEmpty();
        }
    }
}
