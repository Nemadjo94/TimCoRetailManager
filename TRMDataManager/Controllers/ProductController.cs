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
    [RoutePrefix("api/Products")]
    public class ProductController : ApiController
    {
        // GET: api/Products/
        [HttpGet]
        [Route("Get")]
        public List<ProductModel> GetProducts()
        {
            ProductData data = new ProductData();

            return data.GetProducts();
        }
    }
}
