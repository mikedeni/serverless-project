using System.Security.Claims;

namespace ConstructionSaaS.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetCompanyId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("company_id");
            if (claim != null && int.TryParse(claim.Value, out int companyId))
                return companyId;
            return 0;
        }
        
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("user_id");
            if (claim != null && int.TryParse(claim.Value, out int userId))
                return userId;
            return 0;
        }
    }
}
