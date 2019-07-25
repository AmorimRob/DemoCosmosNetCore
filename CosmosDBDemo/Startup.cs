using System.Threading.Tasks;
using CosmosDBDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace CosmosDBDemo
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options => options.AddPolicy("AllowMyOrigin",
               builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "DEMO Cosmos DB",
                        Version = "v1",
                        Description = "-",
                        Contact = new Contact
                        {
                            Name = "Robson Amorim",
                            Url = "https://github.com/amorimrob"
                        }
                    });
            });

            services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection.GetSection("DatabaseName").Value;
            var containerName = configurationSection.GetSection("ContainerName").Value;
            var account = configurationSection.GetSection("Account").Value;
            var key = configurationSection.GetSection("Key").Value;

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);

            CosmosClient client = clientBuilder
                                .WithConnectionModeDirect()
                                .Build();

            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);

            Database database = await client.CreateDatabaseIfNotExistsAsync(databaseName);

            await database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Cosmos v1");
            });
            app.UseHttpsRedirection();
            app.UseCors("AllowMyOrigin");
            app.UseMvc();
        }
    }
}
