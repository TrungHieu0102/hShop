using Core.Model;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace Core.ConfigOptions
{
    public  class PaypalConfiguration
    {
        private readonly PayPalSettings _payPalSettings;

        public PaypalConfiguration(IOptions<PayPalSettings> payPalSettings)
        {
            _payPalSettings = payPalSettings.Value;
        }

        public string ClientId => _payPalSettings.ClientId;
        public string ClientSecret => _payPalSettings.ClientSecret;

        public Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>
        {
            { "mode", _payPalSettings.Mode },
            { "clientId", ClientId },
            { "clientSecret", ClientSecret },
            { "ConnectionTimeout", _payPalSettings.ConnectionTimeout.ToString() },
            { "RequestRetries", _payPalSettings.RequestRetries.ToString() }
        };
        }

        private string GetAccessToken()
        {
            return new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
        }

        public APIContext GetAPIContext()
        {
            var apiContext = new APIContext(GetAccessToken())
            {
                Config = GetConfig()
            };
            return apiContext;
        }
    }
}
