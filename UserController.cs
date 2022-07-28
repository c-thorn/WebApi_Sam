using KRealm_WebApi.Models;
using Newtonsoft.Json;
using System.Web.Http;

namespace KRealm_WebApi.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        public string Get() { return "failure"; }

        [HttpGet]
        public string Get( string value )
        {
            User u = new User();
            return JsonConvert.SerializeObject( u.GetUser( value ) );
        }

        [HttpPost]
        public string Add([FromBody] string content)
        {
            User u = new User();
            User newUser = JsonConvert.DeserializeObject<User>( content );
            return JsonConvert.SerializeObject( u.AddUser( newUser ) );
        }

        [HttpPost]
        /* loginContent format = { "username": "", "password": "" }  */
        public string Login([FromBody] string content )
        {
            User u = new User();
            LoginAttempt attempt = JsonConvert.DeserializeObject<LoginAttempt>( content );
            return JsonConvert.SerializeObject( u.CheckLogin( attempt ) );
        }

        [HttpPost]
        [HttpPatch]
        [HttpPut]
        public string Update( [FromUri] int number, [FromBody] string content )
        {
            User u = new User();
            User updated = JsonConvert.DeserializeObject<User>( content );
            return JsonConvert.SerializeObject( u.UpdateUser( number, updated ) );
        }

        [HttpDelete]
        public bool Delete(int number)
        {
            User u = new User();
            return u.DeleteUser(number);
        }
    }
}
