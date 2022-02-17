using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class ProfileService : IProfileService // where TUserIdentity : class
    {
        protected UserManager<UserIdentity> _userManager;
        private readonly RoleManager<UserIdentityRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserClaimsPrincipalFactory<UserIdentity> claimsFactory;

        public ProfileService(UserManager<UserIdentity> userManager,
                RoleManager<UserIdentityRole> roleManager,
                IHttpContextAccessor httpContextAccessor,
                IUserClaimsPrincipalFactory<UserIdentity> claimsFactory)
        {
            _userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            var principal = await claimsFactory.CreateAsync(user);
            if (principal == null) throw new Exception("ClaimsFactory failed to create a principal");

            context.AddRequestedClaims(principal.Claims);

            if (_userManager.SupportsUserRole && context.RequestedResources.ParsedScopes.Any(s => s.ParsedName == "roles"))
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    context.IssuedClaims.Add(new Claim(IdentityModel.JwtClaimTypes.Role, roleName));
                    if (roleManager.SupportsRoleClaims)
                    {
                        var role = await roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            context.IssuedClaims.AddRange(await roleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }

            if (context.Client.Properties?.ContainsKey("CookieFingerprint") == true
                && context.Client.Properties["CookieFingerprint"].ToLower() == "false")
            {
                context.IssuedClaims.Add(new Claim("NoCookieFingerprint", "True"));
                return;
            }


            var randomBytes = new byte[50];
            RandomNumberGenerator.Create().GetBytes(randomBytes);
            var fingerPrint = string.Concat(randomBytes.Select(b => b.ToString("X2")).ToArray());
            httpContextAccessor.HttpContext.Items.Add("CookieFingerprint", fingerPrint);

            string fingerPrintHash;
            using (var mySHA256 = SHA256.Create())
            {
                byte[] hashValue = mySHA256.ComputeHash(Encoding.UTF8.GetBytes(fingerPrint));
                fingerPrintHash = string.Concat(hashValue.Select(b => b.ToString("X2")).ToArray());
            }
            context.IssuedClaims.Add(new Claim("CookieFingerprintHash", fingerPrintHash));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = user != null && !await _userManager.IsLockedOutAsync(user);
        }
    }
}
