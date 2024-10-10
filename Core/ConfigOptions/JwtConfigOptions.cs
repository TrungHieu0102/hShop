using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ConfigOptions
{
    public class JwtConfigOptions
    {
        public string Issuer { get; set; }  
        public string Key { get; set; }
        public string Audience { get; set; }

    }
}
