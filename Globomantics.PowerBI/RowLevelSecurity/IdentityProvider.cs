using Globomantics.PowerBI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.PowerBI.Api.Models;
using System.Collections.Generic;
using System.Linq;
namespace Globomantics.PowerBI.RowLevelSecurity
{
    public class IdentityProvider : IIdentityProvider
    {
        public enum Roles
        {
            MaerskAdmin,
            SurveyorCompanyAdmin,
            SurveyorCompanyUser,
            CommercialOwnerAdmin,
            CommercialOwnerUser
        }

        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public class UserRole
        {
            public string Name { get; set; }
            public List<string> Roles { get; set; }
        }


        public List<string> GetRoles(string username)
        {
            var users = new List<UserRole>()
            {
                 new UserRole ()
                 {
                     Name = "Anand", Roles = new List<string>
                                            {
                                                Roles.MaerskAdmin.ToString(),
                                                Roles.CommercialOwnerAdmin.ToString(),
                                            }
                 },
                 new UserRole ()
                 {
                     Name = "nscadmin", Roles = new List<string>
                                            {
                                                Roles.CommercialOwnerAdmin.ToString(),
                                            }
                 },
                 new UserRole ()
                 {
                     Name = "NSC", Roles = new List<string>
                                            {
                                                Roles.CommercialOwnerUser.ToString(),
                                            }
                 },
                 new UserRole ()
                 {
                     Name = "nsbadmin", Roles = new List<string>
                                            {
                                                Roles.SurveyorCompanyAdmin.ToString(),
                                            }
                 },
                 new UserRole ()
                 {
                     Name = "NSB", Roles = new List<string>
                                            {
                                                Roles.SurveyorCompanyUser.ToString(),
                                            }
                 }


            };

            var roles = users.Find(u => string.Equals(u.Name,username,System.StringComparison.OrdinalIgnoreCase));

            if (roles == null)            
                return null;
            
            return roles.Roles;

        }

        public EffectiveIdentity GetUserIdentity()
        {
            var user = GetCurrentUserName();
            return new EffectiveIdentity
            {
                Username = user,
                Roles = GetRoles(user)
                //Roles = new List<string> { "MaerskAdmin" }        
            };
        }

        public EffectiveIdentity GetUserIdentityWithDatasetId(string datasetId)
        {
            var user = GetCurrentUserName();
            return new EffectiveIdentity
            {
                Username = user,
                Roles = GetRoles(user),
                //Roles = new List<string> { "MaerskAdmin" },        
                Datasets = new List<string> { datasetId }
            };
        }

        // Utility function to get the current 'user' from our simple authentication setup
        // This would normally be provided by HttpContext.User.Identity (or similar)
        private string GetCurrentUserName()
        {
            var auth = _httpContextAccessor.HttpContext.Request.Cookies["demoAuth"];
            return Base64UrlEncoder.Decode(auth);
        }
    }
}
