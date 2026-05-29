using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.Site24x7
{
    public class AccessTokenDetails
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("client_id")]
        public string client_id { get; set; }

        [JsonProperty("client_secret")]
        public string client_secret { get; set; }
        [JsonProperty("access_token")]
        public string access_token { get; set; }
        [JsonProperty("refresh_token")]
        public string refresh_token { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("api_domain")]
        public string API_Domain { get; set; }
        [JsonProperty("token_type")]
        public string token_type { get; set; }

        [JsonProperty("expires_in")]
        public int expires_in { get; set; }
        public DateTime ExpiresStarts { get; set; }
        public DateTime ExpiresOn { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [JsonProperty("expiryFlag")]
        public bool ExpiryFlag { get; set; }

    }
    
}
