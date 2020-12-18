using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DogGo.Controllers
{
    public class DoggoControllerBase : Controller
    {
        protected int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }

        protected string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role);
        }
    }
}
