using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using QuickBooksASPNetCore2OpenIDConnect.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using OpenId;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace QuickBooksASPNetCore2OpenIDConnect.Pages.QuickBooks.Customer
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly OpenIdConnectConfiguration _openIdConnectionConfiguration;

        public IList<Models.QuickBooksOnline.Customer> Customers { get; private set; }

        public IndexModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IOptions<OpenIdConnectConfiguration> openIdConnectionConfiguration)
        {
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _openIdConnectionConfiguration = openIdConnectionConfiguration.Value;
        }

        public async Task OnGetAsync(string realmId)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var accessToken = await _userManager.GetAuthenticationTokenAsync(user, "QuickBooks", "access_token");
            var baseUrl = $"https://sandbox-quickbooks.api.intuit.com/v3/company/{realmId}/query?query=select * from customer";
            HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(baseUrl);
            qboApiRequest.Method = "GET";
            qboApiRequest.Headers["Authorization"] = string.Format("Bearer {0}", accessToken);
            qboApiRequest.ContentType = "application/json;charset=UTF-8";
            qboApiRequest.Accept = "application/json";
            try
            {
                // get the response
                var response = await qboApiRequest.GetResponseAsync();
                HttpWebResponse qboApiResponse = (HttpWebResponse)response;
                //read qbo api response
                using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                {
                    var result = qboApiReader.ReadToEnd();
                    var rootObj = JsonConvert.DeserializeObject<Models.QuickBooksOnline.RootObject>(result);
                    Customers = rootObj.QueryResponse.Customer.ToList();
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    await PerformRefreshToken();
                }
                else
                {
                }
            }
        }

        public async Task PerformRefreshToken()
        {
            var clientId = "Q06sbTNwSHXscqjxqo2WxLLOYIquJCPDK5EyeosFZTL5okp2vz";
            var clientSecret = "megMDCbbrMAk4Kp41je1YJbMdTXNwtlsTqTSHPtF";

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "QuickBooks", "refresh_token");

            //_openIdConnectionConfiguration.

            string cred = string.Format("{0}:{1}", clientId, clientSecret);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request
            string refreshtokenRequestBody = string.Format("grant_type=refresh_token&refresh_token={0}",refreshToken);

            // send the Refresh Token request
            HttpWebRequest refreshtokenRequest = (HttpWebRequest)WebRequest.Create(this._discoveryData.Token_endpoint);
            refreshtokenRequest.Method = "POST";
            refreshtokenRequest.ContentType = "application/x-www-form-urlencoded";
            refreshtokenRequest.Accept = "application/json";
            //Adding Authorization header
            refreshtokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;

            byte[] _byteVersion = Encoding.ASCII.GetBytes(refreshtokenRequestBody);
            refreshtokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = refreshtokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                //get response
                HttpWebResponse refreshtokenResponse = (HttpWebResponse)refreshtokenRequest.GetResponse();
                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = refreshTokenReader.ReadToEnd();

                    // decode response
                    Dictionary<string, string> refreshtokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    if (refreshtokenEndpointDecoded.ContainsKey("error"))
                    {
                        // Check for errors.
                        if (refreshtokenEndpointDecoded["error"] != null)
                        {
                            _logger.LogError(String.Format("OAuth token refresh error: {0}.", refreshtokenEndpointDecoded["error"]));
                            return new Tuple<string, string>("", "");
                        }
                    }
                    else
                    {
                        //if no error
                        if (refreshtokenEndpointDecoded.ContainsKey("refresh_token"))
                        {
                            _logger.LogInformation("Access token refreshed.");
                            updateFunc(realmId, refreshtokenEndpointDecoded["access_token"], refreshtokenEndpointDecoded["refresh_token"]);
                            return new Tuple<string, string>(refreshtokenEndpointDecoded["access_token"], refreshtokenEndpointDecoded["refresh_token"]);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {

                        _logger.LogInformation("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            _logger.LogInformation(exceptionDetail);
                            _logger.LogError(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                _logger.LogInformation(responseText);
                                _logger.LogError(responseText);
                            }
                        }
                    }

                }
            }
            return new Tuple<string, string>("", "");
        }

        public async Task UpdateUserTokens(string realmId, string accessToken, string refreshToken)
        {
            //todo: update users tokens
            HttpContext context = _httpContextAccessor.HttpContext;
            var userResult = await context.AuthenticateAsync();
            var user = userResult.Principal;
            var props = userResult.Properties;
            //todo: confirm new tokens persisted
            props.UpdateTokenValue("access_token", accessToken);
            props.UpdateTokenValue("refresh_token", refreshToken);
        }

        private DiscoveryData getDiscoveryData()
        {
            _logger.LogInformation("Fetching Discovery Data.");
            var discoveryURI = _configuration["QuickBooksDiscoveryUrl"];
            DiscoveryData discoveryDataDecoded;

            // build the request    
            HttpWebRequest discoveryRequest = (HttpWebRequest)WebRequest.Create(discoveryURI);
            discoveryRequest.Method = "GET";
            discoveryRequest.Accept = "application/json";

            try
            {
                //call Discovery endpoint
                HttpWebResponse discoveryResponse = (HttpWebResponse)discoveryRequest.GetResponse();
                using (var discoveryDataReader = new StreamReader(discoveryResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = discoveryDataReader.ReadToEnd();

                    // converts to dictionary
                    discoveryDataDecoded = JsonConvert.DeserializeObject<DiscoveryData>(responseText);
                }
                _logger.LogInformation("Discovery Data obtained.");
                return discoveryDataDecoded;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        _logger.LogInformation("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            _logger.LogInformation(exceptionDetail);
                            _logger.LogError(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                _logger.LogInformation(responseText);
                                _logger.LogError(responseText);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogInformation(ex.Message);
                    _logger.LogError(ex.Message);
                }
                return null;
            }
        }

    }
}