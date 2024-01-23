using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;

namespace DemoQuickBooks.Repository.Interfaces
{
    public interface IQuickBookAuthorization
    {
        string InitiateAuth();
        Task<TokenResponse> GetAuthTokensAsync(string code, string realmId);
        IEnumerable<Customer> GetCustomers(string accessToken, string realMId);
        Task<TokenResponse> GetAuthByRefreshTokenAsync(string refreshToken, string realmId);
        Customer AddEditCustomerToQB(Customer customer, string realMId, string accessToken);
        Customer GetCustomerIfAlreadyPresent(string Id, string realMId, string accessToken);
        Task<TokenRevocationResponse> RevokeToken(string refreshToken);
    }
}
