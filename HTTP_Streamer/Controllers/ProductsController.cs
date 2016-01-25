using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HTTP_Streamer.Controllers
{
    public class ProductsController : ApiController
    {

        public Product GetProduct()
        {
            return new Product() { productId = 1, productName = "shit" };
        }

    }

    public class Product
    {
        public int productId { get; set; }
        public string productName { get; set; }
    }
}
