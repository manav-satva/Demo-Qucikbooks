using Intuit.Ipp.OAuth2PlatformClient;
using DemoQuickBooks.Repository.Interfaces;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Intuit.Ipp.DataService;
using Microsoft.Extensions.Configuration;

namespace DemoQuickBooks.Repository.Services
{
    public class QuickBookAuthorizationService : IQuickBookAuthorization
    {
        private readonly IConfiguration _configuration;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string redirectUrl;
        private readonly string appEnvironment;
        public QuickBookAuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
            clientId = _configuration.GetValue<string>("QuickBooksCredentials:ClientId") ?? string.Empty;
            clientSecret = _configuration.GetValue<string>("QuickBooksCredentials:ClientSecret") ?? string.Empty;
            redirectUrl = _configuration.GetValue<string>("QuickBooksCredentials:RedirectUrl") ?? string.Empty;
            appEnvironment = _configuration.GetValue<string>("QuickBooksCredentials:AppEnvironment") ?? string.Empty;
        }

        public OAuth2Client Auth2Client => new OAuth2Client(clientId, clientSecret, redirectUrl, appEnvironment);

        public string InitiateAuth()
        {
            string url = string.Empty;
            List<OidcScopes> scopes = new List<OidcScopes> { OidcScopes.Accounting };
            url = Auth2Client.GetAuthorizationURL(scopes);
            return url;
        }

        public async Task<TokenResponse> GetAuthTokensAsync(string code, string realmId)
        {
            TokenResponse tokenResponse = await Auth2Client.GetBearerTokenAsync(code);
            return tokenResponse;
        }

        public async Task<TokenResponse> GetAuthByRefreshTokenAsync(string refreshToken, string realmId)
        {
            TokenResponse tokenResponse = await Auth2Client.RefreshTokenAsync(refreshToken);
            return tokenResponse;
        }

        public IEnumerable<Customer> GetCustomers(string accessToken, string realMId)
        {
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(accessToken);
            ServiceContext serviceContext = new ServiceContext(realMId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            IEnumerable<Customer> list = querySvc.ExecuteIdsQuery("SELECT * FROM Customer");
            return list;
        }

        public Customer AddEditCustomerToQB(Customer customer, string realMId, string accessToken)
        {

            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(accessToken);
            ServiceContext serviceContext = new ServiceContext(realMId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            var context = new DataService(serviceContext);
            Customer obj = querySvc.ExecuteIdsQuery("SELECT * FROM Customer Where DisplayName = '" + customer.DisplayName + "' ").FirstOrDefault();
            if (obj == null)
            {
                if (!string.IsNullOrEmpty(customer.Id))
                    context.Update(customer);
                else
                    context.Add(customer);
            }
            else
            {
                customer.Id = obj.Id;
                customer.SyncToken = obj.SyncToken;
                context.Update(customer);
            }
            return customer;
        }

        public Customer GetCustomerIfAlreadyPresent(string Id, string realMId, string accessToken)
        {
            Customer obj = new Customer();
            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(accessToken);
            ServiceContext serviceContext = new ServiceContext(realMId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            QueryService<Customer> querySvc = new QueryService<Customer>(serviceContext);
            obj = querySvc.ExecuteIdsQuery("SELECT * FROM Customer Where Id = '" + Id + "' ").First();
            return obj;
        }

        public async Task<TokenRevocationResponse> RevokeToken(string refreshToken)
        {
            var tokenResp = await Auth2Client.RevokeTokenAsync(refreshToken);
            return tokenResp;
        }
    }

}
