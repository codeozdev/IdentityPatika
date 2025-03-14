using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityPatika.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("test")]
        public IActionResult Get()
        {
            return Ok(new { message = "Bu veriye sadece yetkilendirilmiş kullanıcılar erişebilir." });
        }
    }
}

// login islemi yapilmadigi icin 401 aliyoruz, login oldugumuzda yetkimiz oluyor
// yetki atamasini yapmamiz lazim