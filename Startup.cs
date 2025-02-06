using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using DesafioAPISiumulacao.Data;
using DesafioAPISimulacao.Application;
using FluentMigrator.Runner;
using System.Data;
using System.Data.SqlClient;
using DesafioAPISimulacao.MigrateDataBase;

namespace DesafioAPISimulacao.API
{
    public class Startup
    {
        private readonly string AllowAllOrigins = "allowAllOrigins";
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOrigins, builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
           
            services.Configure<RequestLocalizationOptions>(
              options =>
              {
                  var supportedCultures = new List<CultureInfo>
                  {
                      new CultureInfo("pt-BR"),
                  };
                  options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: "pt-BR", uiCulture: "pt-BR");
                  options.SupportedCultures = supportedCultures;
                  options.SupportedUICultures = supportedCultures;
              });

            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
            services.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
            
           services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Desafio Simulação V1", Version = "v1" });
            });

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                //paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                //paramsValidation.ValidAudience = tokenConfigurations.Audience;
                //paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                // Valida a assinatura de um token recebido
                paramsValidation.ValidateIssuerSigningKey = true;

                // Verifica se um token recebido ainda é válido
                paramsValidation.ValidateLifetime = true;

                // Tempo de tolerancia para a expira��o de um token (utilizado
                // caso haja problemas de sincronismo de horário entre diferentes
                // computadores envolvidos no processo de comunicação)
                paramsValidation.ClockSkew = TimeSpan.Zero;
            });

            //Ativa o uso do token como forma de autorizar o acesso
            // a recursos deste projeto
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddInfrastructureDI();
            Configuration.AddInfrastructureMapper();
            services.AddApplicationDI();


            var builder = WebApplication.CreateBuilder();

            builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("Connection")));
            builder.Services.AddControllers();
            builder.Services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(builder.Configuration.GetConnectionString("Connection"))
                    .ScanIn(typeof(CreateDataBaseDesafioAPI).Assembly).For.Migrations());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP Request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            //IOptions<RequestLocalizationOptions> localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(localizeOptions.Value);

            string endpointSwagger = Configuration.GetSection("Swagger")["Endpoint"];
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Desafio Simulação V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // global cors policy
            app.UseCors(AllowAllOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async (context) =>
            {
                string message = "No endpoint found - try /swagger";
                await context.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(message), 0, message.Length);
            });

            

        }
    }
}
