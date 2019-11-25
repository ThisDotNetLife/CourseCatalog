using CourseCatalog.api.Services;
using DataLayerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace CourseCatalog.api {
    public class Startup {

        private static IConfigurationRoot config;

        // ENABLE CORS STEP 1 0f 3: Set policy name , which happens to be arbitrary.
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            #region Parse appsettings.json and retrieve default connection string.

            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = builder.Build();

            services.AddScoped<IWebcastRepository,  WebcastRepository>();
            services.AddScoped<IAuthorRepository,   AuthorRepository>();
            services.AddScoped<IVendorRepository,   VendorRepository>();
            services.AddScoped<ITagRepository,      TagRepository>();

            #endregion

            // ENABLE CORS STEP 2 of 3: Call AddCors with a lambda expression. The lambda takes a CorsPolicyBuilder object. 
            // Configuration options, such as WithOrigins, are described later in these articles:
            // https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            // https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core

            services.AddCors(options => {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder => {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowAnyOrigin()
                           .AllowCredentials();
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // ENABLE CORS STEP 3 of 3: Calls UseCors extension method, which enables CORS.
            // By enabling CORS as shown here, the policy is applied to all controller and actions in the API.
            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
