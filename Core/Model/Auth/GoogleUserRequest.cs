using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model.Auth
{
    public class GoogleUserRequest
    {
        public const string PROVIDER = "google";

        [JsonProperty("idToken")]
        [Required]
        public string idToken { get; set; } = string.Empty;
    }
}
