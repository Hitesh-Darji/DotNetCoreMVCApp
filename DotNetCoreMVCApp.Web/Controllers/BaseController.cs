using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNetCoreMVCApp.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        public string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
