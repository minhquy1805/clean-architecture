using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Generate JWT Access Token with custom claims.
        /// </summary>
        string GenerateToken(IEnumerable<Claim> claims);
    }
}
