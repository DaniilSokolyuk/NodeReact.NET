using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NodeReact.Sample.Webpack.AspNetCore
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
            services.AddMvc();

            services.AddNodeReact(
                config =>
                {
                    config.ConfigureOutOfProcessNodeJSServiceOptions = o =>
                    {
                        o.NumRetries = 0;
                        o.InvocationTimeoutMS = 2000;
                    };
                    config.AddScriptWithoutTransform("~/server.bundle.js");
                    config.UseDebugReact = true;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(c =>
            {
                c.MapControllers(); 
            });
        }
    }
}
