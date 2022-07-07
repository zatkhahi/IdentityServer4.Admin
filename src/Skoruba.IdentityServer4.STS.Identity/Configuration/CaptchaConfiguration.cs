using DNTCaptcha.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class CaptchaConfiguration
    {
        public bool EnableLoginCaptcha { get; set; } = false;
        public DisplayMode DisplayMode { get; set; } = DisplayMode.ShowDigits;
        public Language Language { get; set; } = Language.Persian;
        public string FontName { get; set; } = "Tahoma";
        public float FontSize { get; set; } = 18;
        public string BackColor { get; set; } = "#f7f3f3";
        public string ForeColor { get; set; } = "#111111";
        public int MaxValue { get; set; } = 9000;
        public int MinValue { get; set; } = 1;
    }
}
