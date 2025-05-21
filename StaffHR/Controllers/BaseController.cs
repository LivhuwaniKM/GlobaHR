using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StaffHR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}
