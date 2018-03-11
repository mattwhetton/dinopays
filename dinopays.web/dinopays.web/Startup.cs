using System;
using dinopays.web.ApplicationServices;
using dinopays.web.Data;
using dinopays.web.Models;
using dinopays.web.Options;
using dinopays.web.Starling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Newtonsoft.Json.Converters;

namespace dinopays.web
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
            ConfigureMongo();

            services.Configure<StarlingOptions>(Configuration);

            services.AddSingleton<IStarlingClient, StarlingClient>();
            services.AddSingleton<ISummaryBuilder, SummaryBuilder>();
            services.AddSingleton<IGoalRepository, GoalRepository>();
            services.AddSingleton(p =>
            {
                var url = MongoUrl.Create("mongodb://localhost:27017/dinopays");
                var client = new MongoClient(url);
                return client.GetDatabase(url.DatabaseName);
            });
            services.AddSingleton(p => p.GetService<IMongoDatabase>().GetCollection<User>("user"));

            services.AddMvc().AddJsonOptions(o => o.SerializerSettings.Converters.Add(new StringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private static void ConfigureMongo()
        {
            var conventionPack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String)
            };
            BsonSerializer.RegisterIdGenerator(typeof(Guid), GuidGenerator.Instance);

            ConventionRegistry.Register("MongoConventions",
                                        conventionPack,
                                        t => t.Namespace != null && t.Namespace.StartsWith("dinopays.web"));
        }
    }
}
