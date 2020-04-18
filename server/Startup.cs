using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;

using server.Models;

namespace server
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
            services.AddOData();

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddDbContext<OutcomeReportingContext>(options => options.UseSqlServer("Name=OutcomeReportingContext"));
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<Department>("Department");
            builder.EntitySet<CourseInfo>("CourseInfo");
            builder.EntitySet<Student>("Student");
            builder.EntitySet<Course>("Course");
            builder.EntitySet<Assignment>("Assignment");
            builder.EntitySet<AssignmentTask>("AssignmentTask");
            return builder.GetEdmModel();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
        
                routeBuilder.Expand().Select().OrderBy().Filter().MaxTop(null).Count();

                routeBuilder.MapODataServiceRoute("odataroute", "api", GetEdmModel());
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
        }
    }
}
