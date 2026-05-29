using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.JWT
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string[] Audience { get; set; }
        public int ExpiryInMinutes { get; set; }
    }

    public class JwtTokenModel
    {
        public string Role { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? CustomerId { get; set; }
        public string? SessionId { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
