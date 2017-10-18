using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickBooksASPNetCore2OpenIDConnect.Data;
using QuickBooksASPNetCore2OpenIDConnect.Services;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace QuickBooksASPNetCore2OpenIDConnect
{
    public class Startup 
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(sharedOptions => { })
                .AddCookie()
                .AddOpenIdConnect("QuickBooks", "QuickBooks", openIdConnectOptions =>
                {
                    openIdConnectOptions.Authority = "QuickBooks";
                    openIdConnectOptions.UseTokenLifetime = true;
                    openIdConnectOptions.ClientId = "Q06sbTNwSHXscqjxqo2WxLLOYIquJCPDK5EyeosFZTL5okp2vz"; //client id & client secret need to be set w/ your app keys
                    openIdConnectOptions.ClientSecret = "megMDCbbrMAk4Kp41je1YJbMdTXNwtlsTqTSHPtF";
                    openIdConnectOptions.ResponseType = OpenIdConnectResponseType.Code;
                    openIdConnectOptions.MetadataAddress = "https://developer.api.intuit.com/.well-known/openid_sandbox_configuration/";    //development path
                    openIdConnectOptions.ProtocolValidator.RequireNonce = false;
                    openIdConnectOptions.SaveTokens = true;
                    openIdConnectOptions.GetClaimsFromUserInfoEndpoint = true;
                    openIdConnectOptions.Scope.Add("openid");
                    openIdConnectOptions.Scope.Add("phone");
                    openIdConnectOptions.Scope.Add("email");
                    openIdConnectOptions.Scope.Add("address");
                    openIdConnectOptions.Scope.Add("com.intuit.quickbooks.accounting");
                    openIdConnectOptions.Scope.Add("com.intuit.quickbooks.payment");
                });

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
