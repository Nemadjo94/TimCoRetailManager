using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/Sale")]
    public class SaleController : ApiController
    {
        // POST: api/Sale/
        [HttpPost]
        [Route("PostSale")]
        [Authorize(Roles = "Cashier")]
        public void PostSale(SaleModel sale)
        {
            SaleData data = new SaleData();

            string userId = RequestContext.Principal.Identity.GetUserId();

            data.SaveSale(sale, userId);
        }

        // GET: api/Sale/
        [HttpGet]
        [Route("GetSalesReport")]
        [Authorize(Roles = "Admin,Manager")]
        [Authorize]
        public List<SaleReportModel> GetSalesReport()
        {
            //if (RequestContext.Principal.IsInRole("Admin"))
            //{
            //    // Do admin stuff
            //}else if (RequestContext.Principal.IsInRole("Manager"))
            //{
            //    // Do manager stuff
            //}

            SaleData data = new SaleData();

            return data.GetSalesReport();
        }
    }
}
