using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/Inventory")]
    public class InventoryController : ApiController
    {
        // GET: api/Inventory/
        [HttpGet]
        [Route("GetInventory")]
        [Authorize(Roles = "Manager,Admin")]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData();
            return data.GetInventory();
        }

        // POST: api/Inventory/
        [HttpPost]
        [Route("PostRecord")]
        [Authorize(Roles = "Admin")] // Implement Inventory manager role
        public void PostRecord(InventoryModel record)
        {
            InventoryData data = new InventoryData();
            data.SaveInventoryRecord(record);
        }
    }
}
