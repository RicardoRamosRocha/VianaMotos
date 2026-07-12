using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VianaMotos.Web.Data;

namespace VianaMotos.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdministratorRole)]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
