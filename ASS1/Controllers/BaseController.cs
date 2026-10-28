using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASS1.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected string CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value;
        protected string CurrentUserEmail => User.FindFirst(ClaimTypes.Email)?.Value;
        protected int CurrentAccountId => int.Parse(User.FindFirst("AccountID")?.Value ?? "0");
    }
}