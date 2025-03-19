using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Hospital.Infrastructure;
using Hospital.Domain.Shared;
using Hospital.Domain.Users.SystemUser;
using Hospital.Infrastructure.Users;
using Hospital.Services;
using Hospital.Infrastructure.Patients;
using Hospital.Domain.Patients;
using Hospital.Infrastructure.Logs;
using Hospital.Domain.OperationRequest;
using Hospital.Infrastructure.operationrequestmanagement;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Hospital
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
            services.AddSingleton<IConfiguration>(Configuration);

            // Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Error);
            });

            // Caching and Session
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // CORS for React client; change between the localhost and the deployed client
            // http://localhost:3000
            // http://vs606.dei.isep.ipp.pt
            services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", builder =>
                    builder.WithOrigins("http://localhost:3000", "http://vs606.dei.isep.ipp.pt", "http://localhost:4000")
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials());
            });

            // JWT Authentication
            ConfigureJwtAuthentication(services);

            // Database context configuration
            ConfigureDatabase(services);

            // Register application services and repositories
            RegisterServices(services);

            // Add controllers with JSON support
            services.AddControllers().AddNewtonsoftJson();
        }

        private void ConfigureJwtAuthentication(IServiceCollection services)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<HospitalDbContext>(opt =>
                opt.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    MySqlServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))),
                ServiceLifetime.Scoped);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ISystemUserRepository, SystemUserRepository>();
            services.AddTransient<SystemUserService>();
            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<PatientRegistrationService>();
            services.AddTransient<PatientService>();
            services.AddTransient<IOperationRequestRepository, OperationRequestRepository>();
            services.AddTransient<OperationRequestService>();
            services.AddTransient<ILogRepository, LogRepository>();
            services.AddTransient<ILoggingService, LoggingService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPasswordService, PasswordService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowReactApp");

            app.UseRouting();
            app.UseSession();

            // Authentication middleware and Authorization middleware
            app.UseAuthentication(); // Using JWT Bearer Authentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseFallbackForReact();
        }
    }

    public static class ApplicationBuilderExtensions
    {
        // Adds a fallback path for handling client-side routing in React
        public static void UseFallbackForReact(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });
        }
    }
}
