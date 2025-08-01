using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using MealMentor.API.Helper;
using MealMentor.API.Repositories.AccountRepository;
using MealMentor.API.Repositories.IngredientRepository;
using MealMentor.API.Repositories.PaymentRepository;
using MealMentor.API.Repositories.PlanDateRepository;
using MealMentor.API.Repositories.RecipeRepository;
using MealMentor.API.Repositories.TokenRepository;
using MealMentor.API.Repositories.UserRepository;
using MealMentor.API.Services.AccountService;
using MealMentor.API.Services.IngredientService;
using MealMentor.API.Services.PayOSService;
using MealMentor.API.Services.PlanDateService;
using MealMentor.API.Services.RecipeService;
using MealMentor.API.Services.Sercurity;
using MealMentor.API.Services.TokenService;
using MealMentor.API.Services.UserService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Net.payOS;
using System.Text;
using System.Text.Json.Serialization;

namespace MealMentor.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add Repository
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPlanDateRepository, PlanDateRepository>();
            #endregion

            #region Add Services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<PasswordHasher>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IPayOSService, PayOSService>();
            builder.Services.AddScoped<IPlanDateService, PlanDateService>();
            builder.Services.AddAutoMapper(typeof(MapperConfigure).Assembly);

            builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOSSettings"));
            builder.Services.AddSingleton<PayOS>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<PayOSSettings>>().Value;
            return new PayOS(settings.PAYOS_CLIENT_ID, settings.PAYOS_API_KEY, settings.PAYOS_CHECKSUM_KEY);
        });

            builder.Services.AddHttpContextAccessor();

            #endregion

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
                .SetApplicationName("MealMentor.API");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            //Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                //options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
                options.CallbackPath = new PathString("/signin-google"); //match uri on google console, not controller route
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            //Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {new OpenApiSecurityScheme{
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
            //Firebase
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase.json")
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI();


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors("AllowAll");
            app.MapControllers();

            app.Run();
        }
    }
}
