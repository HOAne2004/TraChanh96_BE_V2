using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class ClaimsExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("UserId");
        if (claim == null)
            throw new UnauthorizedAccessException("Token không chứa UserId.");

        return int.Parse(claim.Value);
    }

    public static Guid GetUserPublicId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(JwtRegisteredClaimNames.Sub);
        if (claim == null)
            throw new UnauthorizedAccessException("Token không chứa PublicId.");

        return Guid.Parse(claim.Value);
    }
}
