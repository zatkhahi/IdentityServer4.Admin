using DNTCaptcha.Core;
using IdentityServer4.Validation;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class CaptchaValidator : ICustomTokenRequestValidator
    {
        private readonly IDNTCaptchaValidatorService validatorService;
        private readonly IOptions<DNTCaptchaOptions> options;

        public CaptchaValidator(IDNTCaptchaValidatorService validatorService, IOptions<DNTCaptchaOptions> options)
        {
            this.validatorService = validatorService;
            this.options = options;
        }
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            if (!validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.SumOfTwoNumbersToWords))
            {
                context.Result.IsError = true;
                // this.ModelState.AddModelError(options.Value.CaptchaComponent.CaptchaInputName, "Please enter the security code as a number.");
                context.Result.Error = "Invalid Captcha";
            }
            else
                context.Result.IsError = false;
        }
    }
}
