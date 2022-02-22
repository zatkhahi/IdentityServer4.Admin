using DNTCaptcha.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.STS.Identity.Configuration;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers.Api
{
    public class CaptchaApiResponseDto
    {
        public bool CaptchaEnabled { get; set; }
        public string CaptchaImgUrl { get; set; }
        public string CaptchaText { get; set; }
        public string CaptchaToken { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IDNTCaptchaApiProvider _apiProvider;
        private readonly CaptchaConfiguration captchaOptions;

        public CaptchaController(IDNTCaptchaApiProvider apiProvider, IOptions<CaptchaConfiguration> captchaOptions)
        {
            _apiProvider = apiProvider;
            this.captchaOptions = captchaOptions.Value ?? new CaptchaConfiguration();
        }

        [HttpGet("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public ActionResult<CaptchaApiResponseDto> CreateCaptchaParams()
        {
            // Note: For security reasons, a JavaScript client shouldn't be able to provide these attributes directly.
            // Otherwise an attacker will be able to change them and make them easier!
            if (captchaOptions.EnableLoginCaptcha)
            {
                var p = _apiProvider.CreateDNTCaptcha(new DNTCaptchaTagHelperHtmlAttributes
                {
                    BackColor = captchaOptions.BackColor,
                    FontName = captchaOptions.FontName,
                    FontSize = captchaOptions.FontSize,
                    ForeColor = captchaOptions.ForeColor,
                    Language = captchaOptions.Language,
                    DisplayMode = captchaOptions.DisplayMode,
                    Max = captchaOptions.MaxValue,
                    Min = captchaOptions.MinValue
                });
                return new CaptchaApiResponseDto
                {
                    CaptchaEnabled = true,
                    CaptchaImgUrl = p.DntCaptchaImgUrl,
                    CaptchaText = p.DntCaptchaTextValue,
                    CaptchaToken = p.DntCaptchaTokenValue
                };
            }
            else
            {
                return new CaptchaApiResponseDto()
                {
                    CaptchaEnabled = false
                };
            }
        }
    }
}
