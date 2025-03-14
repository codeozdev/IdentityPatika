using IdentityPatika.Context;
using IdentityPatika.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityPatika.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration; // config'den veri okumak icin kullaniyoruz (appsettings.json)


        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                // kullanici olustu 
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false); // cookie bilgisi tutulsun mu = false
                    return Ok(new { message = "User registered successfully" });
                }

                return BadRequest(new { errors = result.Errors.Select(item => item.Description) });
            }

            // hata yoksa yukarisi calisacak
            return BadRequest(new { errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage) });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // var user = await _userManager.FindByEmailAsync(model.Email);

                    var token = HelperGenerateJwtToken.GenerateToken(model.Email, _configuration["Jwt:Key"],
                        _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"]);
                    
                    return Ok(new
                    {
                        message = "User logged in", token = token
                    }); // -> login oldugumuzda mesaj ve  toke: "sadasdjsadqwekwq" bize gosteriliyor  
                }

                return Unauthorized(new { message = "email or password is incorrect" });
            }

            return BadRequest(new { errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage) });
        }


        // rol ekleyen method
        [HttpPost("createrole")]
        public async Task<IActionResult> CreateRole(UserRoleViewMode model)
        {
            if (!string.IsNullOrWhiteSpace(model.RoleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));

                if (result.Succeeded)
                {
                    return Ok(new { message = "Role created successfully" });
                }
                else
                {
                    return BadRequest(new { errors = result.Errors.Select(item => item.Description) });
                }
            }

            return BadRequest(new { message = "Role name is invalid" });
        }

        // tum rolleri getiren method
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        // kayitli kullanicinin idsi ile ona var olan role atamasi yaptik
        [HttpPost("addtorole")]
        public async Task<IActionResult> AddRole(AddRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                return NotFound(new { message = "Role not found" });
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role added successfully" });
            }
            else
            {
                return BadRequest(new { message = result.Errors.Select(item => item.Description) });
            }
        }

        // id ile kullanicinin rolunu gorme (burayi tum kullanicilari cekelim ve kullanicinin rolunu de gosterecek sekile cevirecegim)
        [HttpGet("userroles/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }
    }
}


// https://www.youtube.com/watch?v=4vkzW8pF85Q&t=1s
// 23:31 -> rol kismini anlatiyor

// AspNetRoles -> database tablosunda gorunuyor
// rolleri kullanicilara atamak icinde HttpPost olusturuyoruz