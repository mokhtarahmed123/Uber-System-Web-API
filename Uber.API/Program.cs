using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Uber.Uber;
using Uber.Uber.Application;
using Uber.Uber.Application.Interfaces;
using Uber.Uber.Application.Interfaces.Repository.DriverProfiles;
using Uber.Uber.Application.Services;
using Uber.Uber.Domain.Entities;
using Uber.Uber.Infrastructure.Services;



namespace Uber.Uber
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();


            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            #region Password Edit
            builder.Services.AddIdentity<User, RoleApp>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
           .AddEntityFrameworkStores<UberContext>()
           .AddDefaultTokenProviders();
            #endregion
            #region DB
            builder.Services.AddDbContext<UberContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("UberSystem")));

            #endregion
            #region Caching
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = "localhost:6379,abortConnect=false"; 
                return ConnectionMultiplexer.Connect(configuration);
            });

            builder.Services.AddSingleton<RedisCacheService>();
            #endregion
            #region Auto Mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #endregion
            #region Swagger Setting
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Uber System",
                    Description = " Uber",
                    Contact = new OpenApiContact
                    {
                        Email = "Uber@Gmail.com",
                        Name = "Uber",
                        
                    }
                });
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "Uber System",
                    Description = " Uber",
                    Contact = new OpenApiContact
                    {
                        Email = "Uber@Gmail.com",
                        Name = "Uber",

                    }
                     
                });
               swagger.EnableAnnotations();


                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });

            #endregion
            #region Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                  .AddJwtBearer(options =>
                  {
                      var secretKey = builder.Configuration["JWT:SecretKey"];
                      if (string.IsNullOrEmpty(secretKey))
                          throw new ArgumentNullException("JWT:SecretKey", "JWT SecretKey is missing in appsettings.json");

                      options.SaveToken = true;
                      options.RequireHttpsMetadata = false;
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidIssuer = builder.Configuration["JWT:IssuerIP"],

                          ValidateAudience = true,
                          ValidAudience = builder.Configuration["JWT:AudienceIP"],

                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                          RoleClaimType = ClaimTypes.Role,
                          ValidateLifetime = true,
                          ClockSkew = TimeSpan.Zero
                      };
                  });

            #endregion
            #region CORS
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policyBuilder =>
                    policyBuilder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader());
            });
            #endregion
            #region Versions
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // v1.0
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc(); // ??? ????: ?? ??????? ????? ?? MVC ?? Controllers
            #endregion
            #region Real Time
            builder.Services.AddSignalR();
            #endregion
         
            builder.Services.AddHttpContextAccessor();

            #region Repositories & Services
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICacheService , RedisCacheService>();
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<IDeliveryService, DeliveryService>();
            builder.Services.AddScoped<ITripService, TripService>();
            builder.Services.AddScoped<IRideRequestsService, RideRequestsService>();
            builder.Services.AddScoped<IDriverProfileService, DriverProfileService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IComplaintsService, ComplaintService>();
            builder.Services.AddScoped<IComplaintsRepo, ComplaintsRepo>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ICategoryService, CategoryServices>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<IDeliveryRepo, DeliveryRepo>();
            builder.Services.AddScoped<IDriverProfilesRepo, DriverProfilesRepo>();
            builder.Services.AddScoped<IItemRepo, ItemRepo>();
            builder.Services.AddScoped<IMerchantRepo, MerchantRepo>();
            builder.Services.AddScoped<IOrderRepo, OrderRepo>();
            builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
            builder.Services.AddScoped<IReviewsRepo, ReviewsRepo>();
            builder.Services.AddScoped<IRideRequestRepo, RideRequestRepo>();
            builder.Services.AddScoped<ITripRepo, TripRepo>();




            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("CorsPolicy");
            app.MapHub<TripHub>("/tripHub");
            app.MapHub<DriverHub>("/driverHub");
            app.MapHub<PaymentHub>("/paymentHub");
            app.MapHub<DeliveryHub>("/deliveryHub");

            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleApp>>();
                await Roles.SeedRolesAsync(roleManager);
            }
            app.Run();
        }
    }
}
