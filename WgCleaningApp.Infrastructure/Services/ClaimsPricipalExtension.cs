using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
namespace WgCleaningApp.Infrastructure.Services


{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                throw new Exception("User ID claim not found in JWT.");

            return Guid.Parse(idClaim.Value);
        }
    }
}

