using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Auth
{
    public class UserToken
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires")]
        public DateTime Expires { get; set; }
    }
}
