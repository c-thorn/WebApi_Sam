using KRealm_WebApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KRealm_WebApi.Controllers
{
    public class FlagsController : ApiController
    {
        [HttpPost]
        public string Add( [FromBody] string content)
        {
            Flag f = new Flag();
            Product newFlag = JsonConvert.DeserializeObject<Product>( content );
            return JsonConvert.SerializeObject( f.AddFlag( newFlag ) );
        }

        [HttpGet]
        public string Get()
        {
            Flag f = new Flag();
            return JsonConvert.SerializeObject( f.GetFlags() );
        }

        [HttpGet]
        public string Get( int number )
        {
            Flag f = new Flag();
            return JsonConvert.SerializeObject( f.GetFlag( number ) );
        }

        [HttpPost]
        [HttpPatch]
        [HttpPut]
        public string Update( [FromUri] int number, [FromBody]string content )
        {
            Flag f = new Flag();
            Product updated = JsonConvert.DeserializeObject<Product>( content );
            return JsonConvert.SerializeObject( f.UpdateFlag( number, updated ) );
        }

        [HttpDelete]
        public bool Delete( [FromUri] int number )
        {
            Flag f = new Flag();
            return f.DeleteFlag( number );
        }
    }
}
