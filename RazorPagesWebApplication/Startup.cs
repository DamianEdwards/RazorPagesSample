using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesWebApplication.Data;
using RazorPagesWebApplication.Services;

namespace RazorPagesWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddGoogleAuthentication(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddMvc(options =>
            {
                options.SslPort = 44335;
                options.Filters.Add(new RequireHttpsAttribute());
            })
            .AddRazorPagesOptions(options =>
            {
                options.AuthorizeFolder("/Account/Manage");
                options.AuthorizePage("/Account/Logout");
            });

            // Add application services.
            services.Configure<SendGridOptions>(Configuration.GetSection("SendGridOptions"));
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            return services.BuildServiceProvider(validateScopes: HostingEnvironment.IsDevelopment());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
