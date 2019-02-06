using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Cars.DTO
{
    public class UserPrivilegeDTO
    {
        public string Username { get; set; }
        public string PrivilegeName { get; set; }
    }

    public class UserPrivilegeDTOValidator : AbstractValidator<UserPrivilegeDTO> {
        public UserPrivilegeDTOValidator() {
            RuleFor(e => e.Username).NotNull().NotEmpty();
            RuleFor(e => e.PrivilegeName).NotNull().NotEmpty();
        }
    }
}
