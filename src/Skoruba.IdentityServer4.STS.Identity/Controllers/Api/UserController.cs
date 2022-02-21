using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController<TUser, TKey> : ControllerBase where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> userManager;

        public UserController(UserManager<TUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("داده های ارسال شده صحیح نیست");
            }
            var idClaim = HttpContext.User.FindFirst(s => s.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null)
                return BadRequest("Invalid Token");

            var user = await userManager.FindByIdAsync(idClaim.Value);
            if (user == null)
            {
                return BadRequest("Invalid user");
            }

            var result = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("Test")]
        public IActionResult Test([FromServices] IDNTCaptchaValidatorService validatorService)
        {
            if (!validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.SumOfTwoNumbersToWords))
            {
                return BadRequest("Invalid captcha");
            }
            return Ok();
        }
    }
}
