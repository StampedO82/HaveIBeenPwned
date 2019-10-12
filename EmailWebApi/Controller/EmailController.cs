using EmailWebApi.Configuration;
using System;
using System.Web.Http;
using Share;

namespace EmailWebApi.Controller
{
    public class EmailController : ApiController
    {
        IUserEmailGrain _userEmailGrain = null;
        IUserEmailGrain UserEmailGrain
        {
            get
            {
                if (_userEmailGrain == null)
                    _userEmailGrain = EmailWebAPIConfig.ClientToSilo.GetGrain<IUserEmailGrain>("nomnio.com");
                return _userEmailGrain;
            }
        }

        [HttpGet]
        [Route("{email}")]
        public string Get(string email)
        {
            try
            {
                var result = UserEmailGrain.IsEmailAddressPwned(email);
                return result.Result.Message.Status == Status.PWNED ? "Found" : "NotFound";
            }
            catch (Exception exc)
            {
                return $"ERROR {exc.Message}";
            }
        }

        [HttpPost]
        [Route("{email}")]
        public string Add(string email)
        {
            try
            {
                var result = UserEmailGrain.AddEmailAddress(email, string.Empty);
                return result.Result.Message.Status == Status.OK ? "Created" : "Conflict";
            }
            catch (Exception exc)
            {
                return $"ERROR: {exc.Message}";
            }
        }
    }
}