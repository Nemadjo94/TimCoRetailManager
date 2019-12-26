using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient ApiClient { get; set; }
        private ILoggedInUserModel _loggedInUserModel { get; set; }

        public APIHelper(ILoggedInUserModel loggedInUserModel)
        {
            _loggedInUserModel = loggedInUserModel;
            // This makes sure we only have one client open during the app time so we don't clog the network
            InitializeClient();
        }

        private void InitializeClient()
        {
            // Gets the api url from app.config file 
            string api = ConfigurationManager.AppSettings["api"];

            ApiClient = new HttpClient(); // we dont want multiple http clients open so we must configure singleton
            ApiClient.BaseAddress = new Uri(api);
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            // this is the data we are sending to the API endpoint
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            });

            using (HttpResponseMessage response = await ApiClient.PostAsync("/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<AuthenticatedUser>();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            ApiClient.DefaultRequestHeaders.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Clear();//?
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await ApiClient.GetAsync("/api/User/GetById"))
            {
                if (response.IsSuccessStatusCode)
                {
                     var result = await response.Content.ReadAsAsync<LoggedInUserModel>();

                    _loggedInUserModel.Token = result.Token;
                    _loggedInUserModel.Id = result.Id;
                    _loggedInUserModel.CreatedDate = result.CreatedDate;
                    _loggedInUserModel.EmailAddress = result.EmailAddress;
                    _loggedInUserModel.FirstName = result.FirstName;
                    _loggedInUserModel.LastName = result.LastName;

                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
