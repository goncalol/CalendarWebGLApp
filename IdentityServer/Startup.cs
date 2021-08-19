using IdentityServer.Data;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("IdentityContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>();

            services.AddTransient<IProfileService, IdentityWithAdditionalClaimsProfileService>();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients(Configuration))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<IdentityWithAdditionalClaimsProfileService>();

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication();
              //.AddGoogle("Google", options =>
              //{
              //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

              //    options.ClientId = "<insert here>";
              //    options.ClientSecret = "<insert here>";
              //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

    }
}
