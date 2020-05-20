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

            services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<OutcomeReportingContext>(options => options.UseSqlServer("Name=OutcomeReportingContext"));
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Department>("Departments");
            builder.EntitySet<CourseInfo>("CourseInfos");
            builder.EntitySet<Student>("Students");
            builder.EntitySet<Course>("Courses");
            builder.EntitySet<Assignment>("Assignments");
            builder.EntitySet<AssignmentTask>("AssignmentTasks");
            builder.EntitySet<AssignmentTaskOutcome>("AssignmentTaskOutcomes").EntityType.HasKey(x => new { x.AssignmentTaskId, x.OutcomeId });
            builder.EntitySet<Outcome>("Outcomes");
            builder.EntitySet<LearningOutcome>("LearningOutcomes");
            builder.EntitySet<ProgramOutcome>("ProgramOutcomes");

            return builder.GetEdmModel();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Expand().Select().OrderBy().Filter().MaxTop(null).Count();
                endpoints.EnableDependencyInjection();
                endpoints.MapODataRoute("odataroute", "api", GetEdmModel());
            });
        }
    }
}
