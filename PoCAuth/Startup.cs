using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoCAuth.Data;
using PoCAuth.Models;
using PoCAuth.Services;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;

namespace PoCAuth
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
          
            
          
            services.AddMvc();
            var connectionString = @"server=.;database=PoCAuth;trusted_connection=yes";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            var context = new ApplicationDbContext(optionsBuilder.Options);
            context.Database.Migrate();
            

            // configure identity server with in-memory users, but EF stores for clients and resources
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            
            // ASP.NET Identity Registrations
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddIdentityServer()
                .AddConfigurationStore(options => options.ConfigureDbContext(new DbContextOptionsBuilder().UseSqlServer(connectionString)))
                .AddOperationalStore(options => options.ConfigureDbContext(new DbContextOptionsBuilder().UseSqlServer(connectionString)))
                .AddAspNetIdentity<ApplicationUser>();
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

//            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseIdentityServer();
        }
    }
}
