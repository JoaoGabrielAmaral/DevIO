using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(INotifier notifier,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings
        ) : base(notifier)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomResponse(registerUser);
            }

            foreach (var error in result.Errors)
            {
                NotifyError(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("signIn")]
        public async Task<ActionResult> SignIn(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomResponse(loginUser);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);
            if (result.Succeeded)
            {
                return CustomResponse(await GenerateToken());
            }

            if (result.IsLockedOut)
            {
                NotifyError("Usuário temporariamente bloqueado por tentativas inválidas.");
                return CustomResponse(loginUser);
            }

            NotifyError("Usuário ou Senha incorretos.");
            return CustomResponse(loginUser);
        }

        private async Task<string> GenerateToken(string email = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emitter,
                Audience = _appSettings.ValidAt,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpireAtHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            });

            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }
    }
}
