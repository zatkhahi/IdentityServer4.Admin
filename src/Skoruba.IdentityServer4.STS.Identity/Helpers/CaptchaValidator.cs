using DNTCaptcha.Core;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class CaptchaValidator : ICustomTokenRequestValidator
    {
        private readonly IDNTCaptchaValidatorService validatorService;
        private readonly IOptions<DNTCaptchaOptions> options;
        private readonly CaptchaConfiguration captchaOptions;

        public CaptchaValidator(IDNTCaptchaValidatorService validatorService, IOptions<DNTCaptchaOptions> options,
            IOptions<CaptchaConfiguration> captchaOptions)
        {
            this.validatorService = validatorService;
            this.options = options;
            this.captchaOptions = captchaOptions.Value ?? new CaptchaConfiguration();
        }
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            if (!captchaOptions.EnableLoginCaptcha)
            {
                context.Result.IsError = false;
                return Task.CompletedTask;
            }
            var properties = context.Result.ValidatedRequest.Client.Properties;
            if (properties != null && properties.ContainsKey("LoginCaptcha") && properties["LoginCaptcha"].ToLower() == "false")
            {
                context.Result.IsError = false;
                return Task.CompletedTask;
            }
            if (!validatorService.HasRequestValidCaptchaEntry(captchaOptions.Language, captchaOptions.DisplayMode))
            {
                context.Result.IsError = true;
                // this.ModelState.AddModelError(options.Value.CaptchaComponent.CaptchaInputName, "Please enter the security code as a number.");
                context.Result.Error = "INVALID_CAPTCHA";
                context.Result.ErrorDescription = "متن امنیتی (کپچا) صحیح نیست";
            }
            else
                context.Result.IsError = false;

            return Task.CompletedTask;
        }
    }
}
