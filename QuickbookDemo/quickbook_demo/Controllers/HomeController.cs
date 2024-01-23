using DemoQuickBooks.Repository.Interfaces;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using ModelDataLibrary.Helper;
using ModelDataLibrary.Models;
using ModelDataLibrary.ViewModels;
using quickbook_demo.Models;
using System.Diagnostics;

namespace quickbook_demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IQuickBookAuthorization _quickbook;
        private readonly IUserToken _userToken;
        private readonly IConfiguration _configuration;
        private readonly string _companyId;

        public HomeController(ILogger<HomeController> logger, IQuickBookAuthorization quickbook, IUserToken userToken, IConfiguration configuration)
        {
            _logger = logger;
            _quickbook = quickbook;
            _userToken = userToken;
            _configuration = configuration;
            _companyId = _configuration.GetValue<string>("QuickBooksCredentials:CompanyId") ?? string.Empty;
        }

        public IActionResult Index(string? realMId)
        {
            if (string.IsNullOrEmpty(realMId))
                realMId = _companyId;

            var user = _userToken.GetByRealMId(realMId);
            if (user != null && user.IsConnected && !(user.AccessTokenExpiryDate < DateTime.Now.AddMinutes(15)))
            {
                ViewBag.RealMId = realMId;
                ViewBag.IsConnected = user.IsConnected;
            }
            else
            {
                string Url = _quickbook.InitiateAuth();
                ViewBag.QbUrl = Url;
                ViewBag.IsConnected = false;
            }
            return View();

        }

        public async Task<IActionResult> callback(string code, string state, string realmId)
        {
            var userInfo = _userToken.GetByRealMId(realmId);
            TokenResponse response = await _quickbook.GetAuthTokensAsync(code, realmId);
            try
            {
                if (userInfo == null)
                {
                    UserToken tokenInfo = new UserToken();
                    tokenInfo.RealMId = realmId;
                    tokenInfo.AccessToken = response.AccessToken;
                    tokenInfo.RefreshToken = response.RefreshToken;
                    tokenInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(response.AccessTokenExpiresIn);
                    tokenInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn);
                    tokenInfo.TokenType = response.TokenType;
                    tokenInfo.CreatedDate = DateTime.Now;
                    tokenInfo.IsConnected = true;
                    _userToken.AddUserToken(tokenInfo);
                }
                else
                {
                    userInfo.AccessToken = response.AccessToken;
                    userInfo.RefreshToken = response.RefreshToken;
                    userInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(response.AccessTokenExpiresIn);
                    userInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(response.RefreshTokenExpiresIn);
                    userInfo.TokenType = response.TokenType;
                    userInfo.UpdatedDate = DateTime.Now;
                    userInfo.IsConnected = true;
                    _userToken.UpdateUserToken(userInfo);
                }
                _userToken.SaveUserTokenChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ErrorView));
            }

            return RedirectToAction(nameof(Index), new { realMId = realmId });
        }
        public async Task<IActionResult> GetCustomersList(string realmId)
        {
            var userInfo = _userToken.GetByRealMId(realmId);
            ViewBag.RealmId = realmId;
            //ViewBag.IsConnected = obj.IsConnected;
            IEnumerable<Customer> list = new List<Customer>();
            try
            {
                if (userInfo == null)
                {
                    return RedirectToAction(nameof(Index), new { realMId = realmId });
                }
                else if (userInfo.AccessTokenExpiryDate < DateTime.Now.AddMinutes(15))
                {
                    TokenResponse refreshObj = await _quickbook.GetAuthByRefreshTokenAsync(userInfo.RefreshToken, userInfo.RealMId);

                    userInfo.AccessToken = refreshObj.AccessToken;
                    userInfo.RefreshToken = refreshObj.RefreshToken;
                    userInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.AccessTokenExpiresIn);
                    userInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.RefreshTokenExpiresIn);
                    userInfo.UpdatedDate = DateTime.Now;
                    userInfo.IsConnected = true;
                    _userToken.UpdateUserToken(userInfo);

                    _userToken.SaveUserTokenChanges();
                }

                list = _quickbook.GetCustomers(userInfo.AccessToken, realmId);
                return View(list);
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(ErrorView));
            }
        }
        public async Task<IActionResult> AddEditCustomerData(string realmId, string Id = "")
        {
            ViewBag.RealMId = realmId;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var userInfo = _userToken.GetByRealMId(realmId);
                    if (userInfo == null)
                    {
                        return RedirectToAction(nameof(Index), new { realMId = realmId });
                    }
                    else if (userInfo.AccessTokenExpiryDate < DateTime.Now.AddMinutes(15))
                    {
                        TokenResponse refreshObj = await _quickbook.GetAuthByRefreshTokenAsync(userInfo.RefreshToken, userInfo.RealMId);
                        userInfo.AccessToken = refreshObj.AccessToken;
                        userInfo.RefreshToken = refreshObj.RefreshToken;
                        userInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.AccessTokenExpiresIn);
                        userInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.RefreshTokenExpiresIn);
                        userInfo.UpdatedDate = DateTime.Now;
                        userInfo.IsConnected = true;
                        _userToken.UpdateUserToken(userInfo);
                        _userToken.SaveUserTokenChanges();
                    }
                    var customer = _quickbook.GetCustomerIfAlreadyPresent(Id, realmId, userInfo.AccessToken);
                    CustomerViewModel customerViewModel = ModelMapper.Map_CustomerViewModel_to_Customer(customer);
                    return View(customerViewModel);
                }
                else
                {
                    return View(new CustomerViewModel());
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ErrorView));
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddEditCustomerData(CustomerViewModel customerViewModel)
        {
            string realMid = customerViewModel.RealMId;
            try
            {
                Customer customer = ModelMapper.Map_Customer_to_CustomerViewModel(customerViewModel);
                var userInfo = _userToken.GetByRealMId(realMid);
                if (userInfo == null)
                {
                    return RedirectToAction(nameof(Index), new { realMId = realMid });
                }
                else if (userInfo.AccessTokenExpiryDate < DateTime.Now.AddMinutes(15))
                {
                    TokenResponse refreshObj = await _quickbook.GetAuthByRefreshTokenAsync(userInfo.RefreshToken, userInfo.RealMId);

                    userInfo.AccessToken = refreshObj.AccessToken;
                    userInfo.RefreshToken = refreshObj.RefreshToken;
                    userInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.AccessTokenExpiresIn);
                    userInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.RefreshTokenExpiresIn);
                    userInfo.UpdatedDate = DateTime.Now;
                    userInfo.IsConnected = true;
                    _userToken.UpdateUserToken(userInfo);

                    _userToken.SaveUserTokenChanges();
                }
                _quickbook.AddEditCustomerToQB(customer, realMid, userInfo.AccessToken);
                return RedirectToAction(nameof(GetCustomersList), new { realmId = realMid });
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(ErrorView));
            }
        }

        public async Task<ActionResult> RevokeToken(string realmId)
        {
            var userInfo = _userToken.GetByRealMId(realmId);
            try
            {
                if (userInfo == null)
                {
                    return RedirectToAction(nameof(Index), new { realMId = realmId });
                }
                else if (userInfo.AccessTokenExpiryDate < DateTime.Now.AddMinutes(15))
                {
                    TokenResponse refreshObj = await _quickbook.GetAuthByRefreshTokenAsync(userInfo.RefreshToken, userInfo.RealMId);
                    userInfo.AccessToken = refreshObj.AccessToken;
                    userInfo.RefreshToken = refreshObj.RefreshToken;
                    userInfo.AccessTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.AccessTokenExpiresIn);
                    userInfo.RefreshTokenExpiryDate = DateTime.Now.AddSeconds(refreshObj.RefreshTokenExpiresIn);
                    userInfo.UpdatedDate = DateTime.Now;
                    userInfo.IsConnected = true;
                    _userToken.UpdateUserToken(userInfo);
                    _userToken.SaveUserTokenChanges();
                }
                var TokenReponse = _quickbook.RevokeToken(userInfo.RefreshToken);
                if (TokenReponse != null)
                {
                    userInfo.AccessToken = string.Empty;
                    userInfo.RefreshToken = string.Empty;
                    userInfo.AccessTokenExpiryDate = null;
                    userInfo.RefreshTokenExpiryDate = null;
                    userInfo.TokenType = string.Empty;
                    userInfo.UpdatedDate = DateTime.Now;
                    userInfo.IsConnected = false;
                    _userToken.UpdateUserToken(userInfo);
                    _userToken.SaveUserTokenChanges();
                }
                return RedirectToAction(nameof(Index), new { realMId = realmId });
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(ErrorView));
            }
        }

        public ActionResult ErrorView()
        {
            return View();
        }
    }
}
