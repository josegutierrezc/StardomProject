using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace AppsManager.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(e => e.FirstName).NotNull().NotEmpty();
            RuleFor(e => e.LastName).NotNull().NotEmpty();
            RuleFor(e => e.Username).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty();
            RuleFor(e => e.IsActive).NotNull().NotEmpty();
        }
    }
}
