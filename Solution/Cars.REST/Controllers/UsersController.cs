using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using AppsManager.DL;
using AppsManager.DTO;
using FluentValidation.Results;

namespace Cars.REST.Controllers
{
    public class UsersController : ApiController
    {
        [Route("api/v1/users")]
        [HttpGet]
        public HttpResponseMessage Get()
        {
            //Get user from DB
            UsersManager userMan = new UsersManager();
            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<User>, List<UserDTO>>(userMan.GetAll()));
        }

        [Route("api/v1/users")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] UserDTO dto)
        {
            //Validate
            UserDTOValidator validator = new UserDTOValidator();
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

            //Add
            UsersManager userMan = new UsersManager();
            User user = Mapper.Map<UserDTO, User>(dto);
            bool added = userMan.Add(user);

            //Return
            return added ? Request.CreateResponse(HttpStatusCode.Created) : Request.CreateResponse(HttpStatusCode.Conflict, "A user with username " + dto.Username + " exists already.");
        }

        [Route("api/v1/users/{username}")]
        [HttpGet]
        public HttpResponseMessage Get(string username)
        {
            //Get user from DB
            UsersManager userMan = new UsersManager();
            User user = userMan.GetByUsername(username);

            if (user == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "User does not exist.");

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<User, UserDTO>(user));
        }

        [Route("api/v1/users/{username}")]
        [HttpPut]
        public HttpResponseMessage Put([FromBody] UserDTO dto)
        {
            //Validate
            UserDTOValidator validator = new UserDTOValidator();
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid) return Request.CreateResponse(HttpStatusCode.BadRequest, result.Errors);

            //Update
            UsersManager userMan = new UsersManager();
            bool updated = userMan.Update(Mapper.Map<UserDTO, User>(dto));

            //Return
            return updated ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to update user " + dto.Username);
        }

        [Route("api/v1/users/{username}")]
        [HttpDelete]
        public HttpResponseMessage Delete(string username)
        {
            //Delete
            UsersManager userMan = new UsersManager();
            bool deleted = userMan.Delete(username);

            //Return
            return deleted ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to delete user " + username);
        }

        [Route("api/v1/users/{username}/agencies")]
        [HttpGet]
        public HttpResponseMessage Agencies(string username)
        {
            //Get user from DB
            UsersManager userMan = new UsersManager();
            User user = userMan.GetByUsername(username);

            if (user == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "User does not exist.");

            List<UserAgencyHelper> agencies = userMan.GetAgencies(username);

            return Request.CreateResponse(HttpStatusCode.OK, Mapper.Map<List<UserAgencyHelper>, List<UserAgencyDTO>>(agencies));
        }

        [Route("api/v1/users/{username}/agencies")]
        [HttpPost]
        public HttpResponseMessage Agencies(string username, [FromBody] UserAgencyDTO dto)
        {
            //Assign Agency to User
            UsersManager userMan = new UsersManager();
            bool assigned = userMan.AssignAgency(Mapper.Map<UserAgencyDTO, UserAgencyHelper>(dto));

            return assigned ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to assign this agency to the user " + username);
        }

        [Route("api/v1/users/{username}/agencies/{agencynumber}")]
        [HttpDelete]
        public HttpResponseMessage Agencies(string username, string agencynumber)
        {
            //Desassign Agency from User
            UsersManager userMan = new UsersManager();
            bool assigned = userMan.UnassignAgency(username, agencynumber);

            return assigned ? Request.CreateResponse(HttpStatusCode.OK) : Request.CreateResponse(HttpStatusCode.BadRequest, "An error occurred trying to assign this agency to the user " + username);
        }
    }
}
