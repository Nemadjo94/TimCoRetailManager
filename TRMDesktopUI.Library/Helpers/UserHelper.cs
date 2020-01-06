using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Helpers
{
    public static class UserHelper
    {
        // Clear out logged in user data
        public static void LogOffUser(ILoggedInUserModel userModel)
        {
            userModel.Id = "";
            userModel.Token = "";
            userModel.FirstName = "";
            userModel.LastName = "";
            userModel.EmailAddress = "";
            userModel.CreatedDate = DateTime.MinValue;
        }
    }
}
