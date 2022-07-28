using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KRealm_WebApi.Models
{
    public class LoginAttempt
    {
        public string username { get; set; }
        public string password { get; set; }
        public string attemptResult { get; set; }
    }
}