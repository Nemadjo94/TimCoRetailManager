﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
       // GET: api/User/
       [HttpGet]
       [Route("GetById")]
       public UserModel GetById()
        {
            // Get current logged in user id
            string userId = RequestContext.Principal.Identity.GetUserId();

            UserData data = new UserData(); // replace with DI

            return data.GetUserById(userId).First();
        }

        // GET: api/User/
        [HttpGet]
        [Route("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();
                var roles = context.Roles.ToList();

                foreach(var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email,

                    };

                    foreach(var r in user.Roles)
                    {
                        u.Roles.Add(r.RoleId, roles.Where(x => x.Id == r.RoleId).First().Name);
                    }

                    output.Add(u);
                }
            }

            return output;
        }

        // GET: api/User/
        [HttpGet]
        [Route("GetAllRoles")]
        [Authorize(Roles = "Admin")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }
        }

        // POST: api/User/
        [HttpPost]
        [Route("AddRole")]
        [Authorize(Roles = "Admin")]
        public void AddRole(UserRolePairModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(model.UserId, model.RoleName);
            }
        }

        // POST: api/User/
        [HttpPost]
        [Route("RemoveRole")]
        [Authorize(Roles = "Admin")]
        public void RemoveRole(UserRolePairModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(model.UserId, model.RoleName);
            }
        }
    }

}
