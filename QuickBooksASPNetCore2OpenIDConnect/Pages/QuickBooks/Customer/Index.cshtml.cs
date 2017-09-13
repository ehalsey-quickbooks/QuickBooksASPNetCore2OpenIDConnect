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

namespace QuickBooksASPNetCore2OpenIDConnect.Pages.QuickBooks.Customer
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IList<Models.QuickBooksOnline.Customer> Customers { get; private set; }

        public IndexModel(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
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
                }
                else
                {
                }
            }
        }
    }
}