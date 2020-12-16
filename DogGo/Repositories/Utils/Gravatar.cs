using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories.Utils
{
    public class Gravatar
    {
        public static string GetUrl(string email)
        {
            string emailHash = HashUtils.MD5(email.ToLower().Trim());

            return $"https://www.gravatar.com/avatar/{emailHash}?d=https://nss-day-cohort-32.github.io/images/nsslogo.png".ToLower();
        }
    }
}
