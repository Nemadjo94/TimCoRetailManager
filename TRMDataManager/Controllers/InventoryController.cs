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
    //[Authorize]
    [RoutePrefix("api/Inventory")]
    public class InventoryController : ApiController
    {
        // GET: api/Inventory/
        [HttpGet]
        [Route("GetInventory")]
        public List<InventoryModel> GetInventory()
        {
            InventoryData data = new InventoryData();
            return data.GetInventory();
        }

        // POST: api/Inventory/
        [HttpPost]
        [Route("PostRecord")]
        public void PostRecord(InventoryModel record)
        {
            InventoryData data = new InventoryData();
            data.SaveInventoryRecord(record);
        }
    }
}
