using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

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

            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }
    }
}
