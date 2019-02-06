using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace AppsManager.DTO
{
    public class AgencyDTO
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string VoucherNotes { get; set; }
        public string Disclaimer { get; set; }
        public string RentMEVersion { get; set; }
        public bool IsActive { get; set; }
        public string LogoFilename { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public bool IsAssociatedAgency { get; set; }
    }

    public class AgencyDTOValidator : AbstractValidator<AgencyDTO>
    {
        public AgencyDTOValidator()
        {
            RuleFor(e => e.Number).NotNull().Length(4);
            RuleFor(e => e.Name).NotNull().NotEmpty();
            RuleFor(e => e.IsActive).NotNull();
            RuleFor(e => e.Phone).NotNull().NotEmpty();
        }
    }
}
