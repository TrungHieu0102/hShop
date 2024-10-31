using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class PayPalSettings
    {
        public string Mode { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int ConnectionTimeout { get; set; }
        public int RequestRetries { get; set; }
    }
}
