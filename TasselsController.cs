using KRealm_WebApi.Models;
using Newtonsoft.Json;
using System.Web.Http;

namespace KRealm_WebApi.Controllers
{
    public class TasselsController : ApiController
    {
        [HttpPost]
        public string Add( [FromBody] string content )
        {
            Tassel t = new Tassel();
            Product newTassel = JsonConvert.DeserializeObject<Product>( content );
            return JsonConvert.SerializeObject( t.AddTassel( newTassel ) );
        }

        [HttpGet]
        public string Get()
        {
            Tassel t = new Tassel();
            return JsonConvert.SerializeObject( t.GetTassels() );
        }

        [HttpGet]
        public string Get( int number )
        {
            Tassel t = new Tassel();
            return JsonConvert.SerializeObject( t.GetTassel( number ) );
        }

        [HttpPost]
        [HttpPatch]
        [HttpPut]
        public string Update( [FromUri] int number, [FromBody] string content )
        {
            Tassel t = new Tassel();
            Product updated = JsonConvert.DeserializeObject<Product>( content );
            return JsonConvert.SerializeObject( t.UpdateTassel( number, updated ) );
        }

        // DELETE api/values/5
        public bool Delete( int number )
        {
            Tassel t = new Tassel();
            return t.DeleteTassel( number );
        }
    }
}