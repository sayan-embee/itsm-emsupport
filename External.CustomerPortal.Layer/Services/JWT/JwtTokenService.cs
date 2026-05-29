
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.JWT;
using External.CustomerPortal.Layer.ExceptionLog;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace External.CustomerPortal.Layer.Services.JWT
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly string _secretKey;
        //private readonly string _issuer;
        //private readonly string _audience;
        //private readonly int _expiryInMinutes;

        public JwtTokenService(IConfiguration configuration
            , IOptions<JwtSettings> jwtSettings
            , IHttpContextAccessor httpContextAccessor
            , IOptions<AppSettingsModel> appSettings
        )
        {
            _appSettings = appSettings.Value;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            //var jwtSettings = configuration.GetSection("JwtSettings");
            //_secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("Required: JwtSettings -> SecretKey");
            //_issuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("Required: JwtSettings -> Issuer");
            //_audience = jwtSettings["Audience"] ?? throw new ArgumentNullException("Required: JwtSettings -> Audience");
            //_expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? throw new ArgumentNullException("Required: JwtSettings -> ExpiryInMinutes"));
        }

        public int GetTokenExpiryInMinutes()
        {
            return _jwtSettings.ExpiryInMinutes;
        }

        public string GenerateJwtToken(JwtTokenModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException("Required: JwtTokenModel");
                }
                else if (string.IsNullOrEmpty(_jwtSettings.SecretKey)
                        || string.IsNullOrEmpty(_jwtSettings.Issuer)
                        || _jwtSettings.Audience.Length == 0
                        || _jwtSettings.ExpiryInMinutes == 0)
                {
                    throw new ArgumentNullException("Required: JwtSettings");
                }
                else if (string.IsNullOrEmpty(model.UserEmail)
                    || string.IsNullOrEmpty(model.CustomerId))
                {
                    throw new ArgumentNullException("Required: JwtTokenModel");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.UserEmail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, model.UserEmail),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim(ClaimTypes.Email, model.UserEmail),
                    new Claim(ClaimTypes.UserData, model.CustomerId),
                    new Claim(ClaimTypes.NameIdentifier, model.SessionId),
                };

                //claims.Add(new Claim("aud", _jwtSettings.Audience.First()));
                //foreach (var aud in _jwtSettings.Audience.Skip(1))
                //{
                //    claims.Add(new Claim("aud", aud));
                //}

                string clientOrigin = _httpContextAccessor.HttpContext?.Items["ClientOrigin"]?.ToString() ?? string.Empty;
                string[] validAudiences = _jwtSettings.Audience;
                string audience = validAudiences.Contains(clientOrigin) ? clientOrigin : validAudiences.First();

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: audience,
                    claims: claims,
                    expires: model.ExpiresOn, // DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw;
            }
        }
    }
}
