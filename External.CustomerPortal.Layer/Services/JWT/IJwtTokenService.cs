using Common.Layer.Models.JWT;

namespace External.CustomerPortal.Layer.Services.JWT
{
    public interface IJwtTokenService
    {
        int GetTokenExpiryInMinutes();
        string GenerateJwtToken(JwtTokenModel model);
    }
}